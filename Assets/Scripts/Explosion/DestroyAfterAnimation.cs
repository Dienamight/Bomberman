using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour {

     public ParticleSystem explosionParticle;

    float timer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (explosionParticle)
        {
            if (timer >= explosionParticle.main.duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
