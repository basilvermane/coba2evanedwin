using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour {

	public float distance = 30;

	private static AIManager _instance;
	public static AIManager instance{
		get{
			if (_instance == null) {
				_instance = FindObjectOfType<AIManager> ();
			}
			return _instance;
		}
	}

	private IMovable[] movables;

	// Use this for initialization
	void Start () {
		movables = GetComponentsInChildren <IMovable> ();
		foreach (IMovable movable in movables) {
			movable.Destination = RandomWalk (distance);
		}
	}

	public Vector3 RandomWalk (float distance, int layermask = -1)
	{
		Vector3 pos = Random.insideUnitSphere * distance;
		NavMeshHit hit;

		NavMesh.SamplePosition (pos, out hit, distance, layermask);
		return hit.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
