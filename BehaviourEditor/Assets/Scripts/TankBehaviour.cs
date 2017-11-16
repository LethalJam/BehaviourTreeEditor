using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBehaviour : MonoBehaviour {

    // Publics
    public float speed = 1.0f;
    public float rotationspeed = 1.0f;
    public float projectileSpeed = 10.0f;

    [Tooltip("Raycasting settings")]
    public float raylength = 10.0f;
    [SerializeField]
    public LayerMask layerMask;

    public GameObject bullet;

    // Privates
    private Rigidbody2D body;

	// Use this for initialization
	void Awake ()
    {
        body = GetComponent<Rigidbody2D>();
	}

    // Movement commands
    public void moveForward()
    {
        body.velocity = transform.up * speed;
    }
    public void moveBackwards()
    {
        body.velocity = transform.up * speed * -1;
    }
    // Rotation commands
    public void rotateLeft()
    {
        transform.Rotate(transform.forward, rotationspeed);
    }
    public void rotateRight()
    {
        transform.Rotate(transform.forward, -rotationspeed);
    }
    // Shoot command
    public void shoot()
    {
        // Instantiate new bullet
       GameObject newBullet = GameObject.Instantiate(bullet);
       Rigidbody2D newBody = newBullet.GetComponent<Rigidbody2D>();
        newBullet.transform.position = transform.position;
        newBody.velocity = transform.up * projectileSpeed;
    }

    // Function for checking obstruction in an angle
    public bool checkObstructed(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raylength, layerMask);
        // Check if the chosen direction is obstructed
        if (hit)
        {
            Debug.DrawLine(transform.position, hit.point);
            return true;
        }
        return false;
    }

    // FOR TESTING PURPOSES
    void Update()
    {
        moveForward();
        rotateLeft();
        checkObstructed(transform.up);

        if (Input.GetButtonDown("Fire1"))
            shoot();
    }

}
