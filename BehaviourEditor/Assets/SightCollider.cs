using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCollider : MonoBehaviour {

	private bool foundEnemy = false;
    private Color originalColor;

    public string enemyTag;
    public LayerMask rayMask;

    void Awake()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Material mat = renderer.material;
        originalColor = mat.color;
    }

    public bool enemyInSight()
    {
        return foundEnemy;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Check if enemy in sight
        if (other.tag == enemyTag)
        {
            Vector3 enemyDir = Vector3.Normalize(other.transform.position - transform.parent.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.parent.transform.position, enemyDir, Mathf.Infinity, rayMask);
            Debug.DrawLine(transform.parent.transform.position, hit.point, Color.red);
            if (hit.collider == other)
            {
                foundEnemy = true;
                setFoundAlpha(foundEnemy);
            }
            else
            {
                foundEnemy = false;
                setFoundAlpha(foundEnemy);
            }
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (other.tag == enemyTag)
        {
            foundEnemy = false;
            setFoundAlpha(foundEnemy);
        }
    }

    void setFoundAlpha(bool found)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        Material mat = renderer.material;

        if (found)
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 0.8f);
        else
            mat.color = originalColor;

    }

    // FOR TESTING PURPOSES
    void Update()
    {
        print("Found enemy: " + foundEnemy);
    }
}
