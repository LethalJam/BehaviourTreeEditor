using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    private SightCollider sight;

    // Timer related
    private float shootingCooldown = 0.5f;
    private float timer = 0.0f;

	// Use this for initialization
	void Awake ()
    {
        body = GetComponent<Rigidbody2D>();
        sight = transform.GetChild(0).GetComponent<SightCollider>();
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
        timer += Time.deltaTime;
        if (timer >= shootingCooldown)
        {
            // Instantiate new bullet
            GameObject newBullet = GameObject.Instantiate(bullet);
            Rigidbody2D newBody = newBullet.GetComponent<Rigidbody2D>();
            newBullet.transform.position = transform.position;
            newBody.velocity = transform.up * projectileSpeed;
            // Reset timer
            timer = 0.0f;
        }
    }

    // Function for checking obstruction in an angle
    public bool checkObstructed(Vector2 direction)
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Vector3 perpendicular = Vector3.Cross(new Vector3(0, 0, 1), new Vector3(direction.x, direction.y, 0));
        Vector3.Normalize(perpendicular);
        perpendicular *= 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raylength, layerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + perpendicular, direction, raylength, layerMask);
        RaycastHit2D hit3 = Physics2D.Raycast(transform.position - perpendicular, direction, raylength, layerMask);
        // Check if the chosen direction is obstructed
        if (hit || hit2 || hit3)
        {
            if (hit)
                Debug.DrawLine(transform.position, hit.point);
            if (hit2)
                Debug.DrawLine(transform.position + perpendicular, hit2.point);
            if (hit3)
                Debug.DrawLine(transform.position - perpendicular, hit3.point);
            return true;
        }
        return false;
    }

    public bool foundEnemy()
    {
        return sight.enemyInSight();
    }

}
