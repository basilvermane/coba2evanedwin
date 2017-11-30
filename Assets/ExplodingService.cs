using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingService : MonoBehaviour {
    [SerializeField]
    private GameObject explodingEffect;
    [SerializeField]
    private int explosionDamage = 50;

    

    private static ExplodingService _instance;
    public static ExplodingService instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<ExplodingService>();
            }
            return _instance;
        }
    }

    public void CreateExplosion(Vector3 location)
    {
        Instantiate(explodingEffect, location , Quaternion.identity);
        Invoke("DamagePlayer", 0.3f);
    }

    public void DamagePlayer()
    {
        FPSController.instance.TakeDamage(explosionDamage);
    }
}
