using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehaviour : MonoBehaviour {

    // Publics
    public float speed = 1.0f;

    // Privates
    private Rigidbody2D body;

	// Use this for initialization
	void Awake ()
    {
        body = GetComponent<Rigidbody2D>();
	}

    public void moveForward()
    {
        body.velocity = transform.up * speed;
    }
    public void moveBackwards()
    {
        body.velocity = transform.up * speed * -1;
    }

    // FOR TESTING PURPOSES
    void Update()
    {
        moveForward();
        print(transform.up);
    }

}
