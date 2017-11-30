using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {
	public GameObject bullet;

	public Transform projectilePos;

	public int maxTotalBullet = 150;
	public int ammo = 30;
	public int ammoSisa = 0;
	public bool emptyMagazine = false;
	public bool reloading = false;

	public float bulletPerSecond = 30;
	public int damage = 55;
	public float lastFiringTime;
	public float firingDelay;

	private Transform firingPos;
	private ParticleSystem firingEffect;

	// Use this for initialization
	private void Start () {
		GameObject go = (GameObject) Instantiate (
			bullet,
			projectilePos.position,
			projectilePos.rotation
		);

		go.transform.parent = projectilePos;
		firingPos = Camera.main.transform;

		firingEffect = go.GetComponent<ParticleSystem> ();
		lastFiringTime = 0.0f;
		firingDelay = 1 / bulletPerSecond;
	}

	private bool DelayAfterFiringCompleted(){
		if (Time.time >= lastFiringTime + firingDelay) {
			lastFiringTime = Time.time;
			return true;
		}
		return false;
	}

	private void Fire(){
		firingEffect.time = 0;
		firingEffect.Play ();
		RaycastHit hit;
		if (Physics.Raycast (
			firingPos.position,
			firingPos.forward,
			out hit)) {
			if(hit.transform.CompareTag("Enemy")){
				enemyHealth e = hit.transform.GetComponent<enemyHealth> ();
				Debug.Log ("Kenaa dehh");
				e.enemyHP -= damage;
				if (e.enemyHP <= 0) {
					Destroy (e.go);
				}
			}
		}
	}

	// Update is called once per frame
	void Update () {

		if ((/*Input.GetButton ("Fire1") ||*/ OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.3f) && DelayAfterFiringCompleted () && !emptyMagazine && !reloading) {
            FindObjectOfType<SoundManager>().Play("Firing");
			Fire ();
			ammo--;

			if (ammo <= 0) {
				emptyMagazine = true;
				Debug.Log ("abis pelor bang");
			}
		}

		if (/*Input.GetKeyDown(KeyCode.R) ||*/ OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.3f) {
			Debug.Log ("R kepencet");
			Reload ();
		}
	}

	public void Reload(){
		reloading = true;
		Debug.Log ("reload dulu aah");
		ammoSisa = 30 - ammo;
		ammo = ammo + ammoSisa;
		emptyMagazine = false;
		reloading = false;
	}
}
