using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionTimer : MonoBehaviour {

    public float lifeTime = 2.0f;
    private float timer = 0.0f;
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Wall")
            Destroy(gameObject);
    }
}


