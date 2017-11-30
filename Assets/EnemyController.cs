using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class EnemyController : MonoBehaviour, IDamageable, IMovable {

	NavMeshAgent agent;
	enum EnemyState{Wander, Chasing, Suicide, Length}

	private EnemyState state;
	private int currentHealth;
	private Vector3 destination;

	[SerializeField]
	private float speed;

	public int maxHealth;
	public int Health{
		get{
			return currentHealth;
		}
		private set{
			currentHealth = value;
		}
	}

	public int MaxHealth{
		get{
			return maxHealth;
		}
	}

	public bool IsAlive{
		get{
			return
				currentHealth > 0;
		}
	}

	public Vector3 Position{
		get{
			return transform.position;
		}
	}

	public Vector3 Destination{
		get{
			return destination;
		}

		set{
			destination = value;
			agent.SetDestination (destination);
		}
	}

	public Vector3 Direction{
		get{
			return destination - transform.position;
		}
	}

	public float Speed{
		get{
			return speed;
		}
	}

	public Quaternion TargetRotation{
		get{
			return
				Quaternion.LookRotation (Direction, transform.up);
		}
	}

	public void Awake(){
		agent = GetComponent<NavMeshAgent> ();
		ResetHealth();
		state = EnemyState.Wander;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {    
		switch (state) {
		case EnemyState.Wander:
			if (IsClosedBy (Destination)) {
                    Destination = AIManager.instance.RandomWalk(
                            AIManager.instance.distance
                        );
			}
			break;
		case EnemyState.Chasing:
                Destination = FPSController.instance.Position;
			break;
		case EnemyState.Suicide:
                Destroy(this.gameObject);
                ExplodingService.instance.CreateExplosion(transform.position);
			break;
		default:
			throw new Exception ("Enemy state isn't yet implemented!");
			
		}
		ValidateState ();
	}

	private void ValidateState ()
	{
		EnemyState nextState = EnemyState.Wander;
		Vector3 playerPos = FPSController.instance.Position;
		Vector3 direction = playerPos - transform.position;

		float angle = Vector3.Angle (direction, transform.forward);

		float fovAngle = 160.0f;

		if (angle < fovAngle * 0.05f) {
			nextState = EnemyState.Chasing;
			if(IsClosedBy(playerPos)){
				nextState = EnemyState.Suicide;
			}
		}
		state = nextState;
	}

	public bool IsClosedBy (Vector3 target)
	{
		float closeDistance = 4.0f;
		float currentDistance = Vector3.Distance (target, transform.position);
		if (currentDistance <= closeDistance) {
			return true;
		}
		return false;
	}

	public void TakeDamage(int damageAmount){
		Health -= damageAmount;
		if (IsAlive) {
			return;
		}
		Destroy (this.gameObject);
	}

	public void ResetHealth ()
	{
		Health = MaxHealth;
	}
}
