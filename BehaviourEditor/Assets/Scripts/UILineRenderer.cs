using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MonoBehaviour {

    [Header("Used for dynamic updating of linepositions.")]
    public Transform pointA;
    public Transform pointB;
    [Header("Determines width of the given line.")]
    public float lineWidth = 1.0f;

    // Privates
    private RectTransform rectTransform;
    private Vector3 pointAOffset;
    private Vector3 pointBOffset;

    void Awake()
    {
        rectTransform = GetComponent<Image>().rectTransform;
    }

    public void setAttachmentOffsets(Vector3 offset, Vector3 offset2)
    {
        pointAOffset = offset;
        pointBOffset = offset2;
    }

	// Update is called once per frame
	void Update ()
    {
        // If any point is removed, delete the line and the detached gameobjects
        if (pointA == null || pointB == null)
        {
            if (pointB != null)
                Destroy(pointB.gameObject);

            Destroy(gameObject);
        }
        else
        {
            Vector3 deltaVec = (pointB.position + pointBOffset) - (pointA.position + pointAOffset);
            rectTransform.sizeDelta = new Vector2(deltaVec.magnitude, lineWidth);
            rectTransform.pivot = new Vector2(0, 0.5f);
            rectTransform.position = pointA.position + pointAOffset;
            float angle = Mathf.Atan2(deltaVec.y, deltaVec.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
	}
}
