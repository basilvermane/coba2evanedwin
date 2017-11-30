using UnityEngine;
using System.Collections;

public class enemyHealth : MonoBehaviour {
	public int enemyHP = 100;
	public GameObject go;

	// Use this for initialization
	void Start () {
		go = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//GameObject musuh = GameObject.FindGameObjectWithTag ("Enemy");
	}

//	public static void TakeDamage(int sisaDarah) {
//		if (sisaDarah <= 0 ) {
//			Destroy(this.gameObject);
//		}
//
//	}
}
