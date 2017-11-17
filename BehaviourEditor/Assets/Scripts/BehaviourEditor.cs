using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourEditor : MonoBehaviour {
    
    // Public variables to be assigned in Unity editor.
    public BehaviourTree tree;
    public GameObject nodeButton;
    public Canvas editorCanvas;

    private Node targetNode;

    // Use this for initialization
    void Awake()
    {
        if (tree == null)
        {
            this.enabled = false;
        }
        // Create initial node button from the root of the tree.
        GameObject nodeObj = GameObject.Instantiate(nodeButton);
        nodeObj.transform.parent = editorCanvas.transform;
        nodeObj.transform.SetPositionAndRotation(new Vector3(Screen.height/2, Screen.width/2, 0), nodeObj.transform.rotation);
        nodeObj.GetComponent<NodeManipulator>().setNode(tree.getRoot());
        nodeObj.transform.GetChild(0).GetComponent<Text>().text = tree.getRoot().GetType().Name;
	}

    // Setup new node and initialize its values.
    void createNewNode(Node node, Vector2 position)
    {
        GameObject nodeObj = GameObject.Instantiate(nodeButton);
        nodeObj.transform.parent = editorCanvas.transform;
        nodeObj.transform.SetPositionAndRotation(new Vector3(position.x, position.y, 0), nodeObj.transform.rotation);
        nodeObj.GetComponent<NodeManipulator>().setNode(node);
        nodeObj.transform.GetChild(0).GetComponent<Text>().text = tree.getRoot().GetType().Name;
    }

	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
