using System;
using UnityEngine;

public class FPSController : MonoBehaviour, IDamageable {
	//length ditambahkan sebagai element enum terakhir
	//agar lebih mudah dalam mengubah enum PlayerState
	public  enum PlayerState {
		Standing,
		Crouching,
		Crawling,
		Length
	}

	//state awal player.
	private PlayerState state = PlayerState.Standing;

	private bool grounded = false;
	private Rigidbody rb;

	//pointer head melakukan reference terhadap MainCamera
	private Transform head;

	//class yang digunakan untuk menampung seluruh hal terkait dengan
	//input yang dapat dilakukan oleh "User"
	//tidak bersifat "must to have", bersifat "nice to have"
	private UserInput userInput = new UserInput();

	public class UserInput {
		[HideInInspector]
		public float horizontal, vertical;
		[HideInInspector]
		public bool running, jumping, changeState;
		[HideInInspector]
		public float mouseX, mouseY;

		public void CheckInput() {
			//pergerakan mouse terhadap sumbu "X"
			mouseX = Input.GetAxis("Mouse X");
			//pergerakan  mouse terhadap sumbu "Y"
			mouseY = Input.GetAxis("Mouse Y");
			//horizontal axis = 'A','D', '->', dan '<-'
			horizontal = Input.GetAxis("Horizontal");
			//vertical axis = 'W', 'S', 'up', dan 'down'
			vertical = Input.GetAxis("Vertical");
			//saat button left-shift ditekan
			running = /*Input.GetKey(KeyCode.LeftShift) ||*/ (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.3f);
            //saat button jump ditekan
            jumping = /*Input.GetButtonDown("Jump") ||*/ OVRInput.GetDown(OVRInput.Button.One);
			//saat button leftControl ditekan nilai bool akan berubah
			//potongan kode if sengaja dibuat tidak langsung
			//mengubah playerState agar class tetap hanya mengatur
			//input player saja
			if (/*Input.GetKeyDown(KeyCode.LeftControl) ||*/ OVRInput.GetDown(OVRInput.Button.Two)) {
				changeState = true;
			}
		}        
	}

	//grouping terhadap variabel speed saat
	//player melakukan pergerakan "Run", "Walk", ...
	//agar terlihat lebih mudah di Editor yang kita miliki
	[System.Serializable]
	public struct Speed {
		public float run, walk, crouch, crawl;
		//mengembalikan nilai speed sesuai dengan kondisi
		//yang dimiliki oleh "Player"
		public float currentSpeed(
			PlayerState state, 
			bool isRunning, 
			bool isGrounded) {
			switch (state) {
			case PlayerState.Standing:
				return (isRunning && isGrounded)? run: walk;
			case PlayerState.Crouching:
                    AIManager.instance.RandomWalk(0, -1);
				return crouch;
			case PlayerState.Crawling:
                    AIManager.instance.RandomWalk(0, -1);
                    return crawl;
			default:
				return walk;
			}
		}
	}
	//digunakan untuk grouping terhadap parameter
	//yang dimiliki oleh objek "Player"
	[Header("Horizontal Movement")]
	public Speed speed;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool isDiagonalMovementFaster = true;
    public float yRotationOffset = 20.0f;

	[Header("Vertical Movement")]
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool allowOnAirMovement = false;

	[Header("Rotation")]
	public float rotXSpeed = 90.0f;
	public float rotYSpeed = 90.0f;
	public float minRotX = -60.0f;
	public float maxRotX = 40.0f;


	//Singleton, agar memudahkan akses terhadap "Player"
	//mengingat nanti akan tepat ada satu "Player" di dalam
	//permainan yang akan kita kembangkan, SSoT
	private static FPSController _instance;
	public static FPSController instance {
		get {
			if(_instance == null) {
				_instance = FindObjectOfType<FPSController>();
			}
			return _instance;
		}
	}

	private void Awake() {
		head = Camera.main.transform;

		rb = GetComponent<Rigidbody>();
		//memastikan agar kapsul Player tidak bisa jatuh
		//ke arah kiri, kanan, depan, dan belakang
		rb.freezeRotation = true;
		//mematikan use gravity milik player
		//nanti gravity akan kita set secara manual 
		//menggunakan fungsi AddForce
		rb.useGravity = false;

	}

	private void Start() {
		//menghilangkan panah cursor pada Layar permainan
		Cursor.lockState = CursorLockMode.Locked;
        FindObjectOfType<SoundManager>().Play("Theme");
	}

	//seluruh fungsi terkait dengan input dipanggil di
	//dalam fungsi Update()
	private void Update() {
		//memanggil fungsi untuk mengecek
		//input yang ditekan oleh pemain
		userInput.CheckInput();
		if (userInput.changeState) {
			ChangeState();
			userInput.changeState = false;
		}
	}

	//seluruh fungsi terkait dengan pergerakan dan physics
	private void FixedUpdate() {
		Vector3 velocity = rb.velocity;

		Movement(ref velocity);
		Jump(ref velocity);

		//secara manual mensimulasikan gravitasi..
		rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

		grounded = false;
	}
	//fungsi LateUpdate yang dipanggil setelah seluruh
	//fungsi Update selesai dijalankan
	private void LateUpdate() {
		Rotate();
	}

	private void Rotate() {
		//        //mengambil nilai rotasi "Player"
		//        //menggunakan sistem euler
		//        Vector3 euler = transform.eulerAngles;
		//        //menambahkan sudut nilai rotasi terhadap sumbu "Y" 
		//        //pergerakan kiri kanan.
		//        euler.y += userInput.mouseX * rotYSpeed * Time.deltaTime;
		//        transform.eulerAngles = euler;
		//
		//        //quaternion merupakan langkah lain untuk merepresentasikan
		//        //nilai rotasi. Lebih baik dikarenakan mencegah kondisi bernama 
		//        //"Gimbal Lock", namun sulit dimengerti oleh manusia.
		//        Quaternion headRotation = head.localRotation;
		//
		//        //menambahkan rotasi kamera dengan melakukan perkalian antar
		//        //rotasi, operasi *= pada Quaternion sama dengan operasi +=
		//        //pada sistem rotasi EulerAngle
		//        headRotation *= Quaternion.Euler(
		//            -userInput.mouseY * rotXSpeed * Time.deltaTime, 
		//            0, 
		//            0
		//        );
		//        //besaran sudut X menggunakan sistem EulerAngle
		//        float angleX = 
		//            2.0f * Mathf.Rad2Deg * Mathf.Atan(headRotation.x);
		//        //membatasi pergerakan sumbu X agar kamera tidak dapat melihat
		//        //ke atas dan ke bawah sampai kepala terbalik
		//        angleX = Mathf.Clamp(angleX, minRotX, maxRotX);
		//
		//        //melakukan konversi rotasi terhadap sumbu "X" dari sistem
		//        //euler ke Quaternion
		//        headRotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
		//        head.localRotation = headRotation; 
		//
		//
		//======== CODINGAN SENDIRI =======

		// mengambil nilai rotasi player
		Vector3 euler = transform.eulerAngles;

		//menambahkan sudut nilai rotasi terhadap sumbu "Y"
		//pergerakan kiri kanan
		euler.y += userInput.mouseX * rotYSpeed * Time.deltaTime;
		transform.eulerAngles = euler;

		Quaternion headRotation = head.localRotation;

		headRotation *= Quaternion.Euler (
			-userInput.mouseY * rotXSpeed * Time.deltaTime,
			0,
			0
		);

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (headRotation.x);

		angleX = Mathf.Clamp (angleX, minRotX, maxRotX);

		headRotation.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		head.localRotation = headRotation;
	}  

	private void Movement(ref Vector3 velocity) {

		//        if (grounded || allowOnAirMovement) {
		//            Vector3 targetVelocity = new Vector3(
		//                userInput.horizontal, 
		//                0, 
		//                userInput.vertical
		//            );
		//            //penjelasan dari bagian ini dapat dilihat pada kotak 
		//            //dengan tulisan biru pada modul
		//            //setelah fungsi selesai dituliskan
		//            if(!isDiagonalMovementFaster)
		//                targetVelocity = targetVelocity.normalized;
		//
		//            //mengubah nilai targetVelocity agar sesuai dengan
		//            //arah transformasi milik "Player", ingat arah 
		//            //selalu bersifat relatif terhadap tiap-tiap objek
		//            targetVelocity = transform.TransformDirection(targetVelocity);
		//
		//            //melakukan perkalian Input dengan speed saat ini untuk 
		//            //melakukan perhitungan kecepatan
		//            targetVelocity *= 
		//                speed.currentSpeed(state, userInput.running, grounded);
		//
		//            //membatasi maksimal dari pergerakan yang dilakukan player
		//            Vector3 velocityChange = (targetVelocity - velocity);
		//            velocityChange.x = Mathf.Clamp(
		//                velocityChange.x, 
		//                -maxVelocityChange, 
		//                maxVelocityChange
		//            );
		//            velocityChange.z = Mathf.Clamp(
		//                velocityChange.z, 
		//                -maxVelocityChange, 
		//                maxVelocityChange
		//            );
		//            velocityChange.y = 0;
		//
		//            rb.AddForce(velocityChange, ForceMode.VelocityChange);
		//        }

		//======== CODINGAN SENDIRI =======
		if (grounded || allowOnAirMovement) {
			Vector3 targetVelocity = new Vector3 (
				userInput.horizontal, 
				0, 
				userInput.vertical
			);

            float cameraRotY = Camera.main.transform.rotation.eulerAngles.y;

            print(cameraRotY);

            targetVelocity = Quaternion.AngleAxis(cameraRotY + yRotationOffset, Vector3.up) * targetVelocity;

			if(!isDiagonalMovementFaster){
				targetVelocity = targetVelocity.normalized;
			}

			targetVelocity = transform.TransformDirection (targetVelocity);

			targetVelocity *= speed.currentSpeed (state, userInput.running, grounded);

			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp (
				velocityChange.x,
				-maxVelocityChange,
				maxVelocityChange
			);
			velocityChange.z = Mathf.Clamp (
				velocityChange.z,
				-maxVelocityChange,
				maxVelocityChange
			);
			velocityChange.y = 0;

			rb.AddForce (velocityChange, ForceMode.VelocityChange);
		}
	}

	private void Jump(ref Vector3 velocity) {
		//        if (state == PlayerState.Standing && //selama "Player" dalam posisi berdiri
		//            canJump && //dalam permainan "Player" di-izinkan untuk meloncat
		//            userInput.jumping && //menekan tombol untuk loncat
		//            grounded) /*berada di atas tanah*/ {
		//
		//            rb.velocity = new Vector3(
		//                velocity.x, 
		//                CalculateJumpVerticalSpeed(), 
		//                velocity.z
		//            );
		//        }

		// ========= CODINGAN SENDIRI =======
		if (state == PlayerState.Standing && canJump && userInput.jumping && grounded) {
			rb.velocity = new Vector3 (
				velocity.x,
				CalculateJumpVerticalSpeed (),
				velocity.z
			);
		}
	}

	private void ChangeState() {
		//        int nextState = (int)state + 1;
		//        nextState %= (int)PlayerState.Length;
		//        state = (PlayerState)nextState;
		//        Debug.Log(state);

		// ==== CODINGAN SENDIRI =====
		int nextState = (int) state + 1;
		nextState %= (int)PlayerState.Length;
		state = (PlayerState)nextState;
		Debug.Log (state);

	}

	private void OnCollisionStay() {
		grounded = true;
	}

	private float CalculateJumpVerticalSpeed() {
		//penjelasan dari fungsi ini akan ada di kotak biru setelah 
		//potongan kode selesai dituliskan
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

    public void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
    }

    public void ResetHealth()
    {
        health -= maxHealth;
    }

    public Vector3 Position {
		get {
			return transform.position;
		}
	}

    private int health;
    public int Health {
        get
        {
            return health;
        }
        private set
        {
            health = value;
            if (health <= 0)
            {
                Debug.Log(health);
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private int maxHealth;
    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
    }

    private bool isAlive;
    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        private set
        {
            if (!isAlive)
            {
                Time.timeScale = 0.0f;
            }

        }
    }
}
