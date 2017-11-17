using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeManipulator : MonoBehaviour {

    private Node node;
    private Button button;
    private GameObject canvas;
    private Dropdown nodeDropDown;

    // Set initial values
    void Awake ()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(chooseNode);

        // Find gameobject and save the dropdown object as well if there is one.
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
            Debug.LogError("No canvas object found.");
        else
            print("Found canvas object.");

        nodeDropDown = canvas.GetComponentInChildren<Dropdown>(true);
        if (nodeDropDown == null)
            Debug.LogError("NO DROPDOWN FOUND: ABORT MISSION.");
    }

    // Start process of selecting and creating new node in the behaviourtree.
    public void chooseNode()
    {
        // Check to see if the editorCanvas is active.
        if (canvas != null)
        {
            if (canvas.activeSelf)
            {
                Node newNode = getNodeType(nodeDropDown.value);
                addChild(newNode);
            }
        }
    }

    // Return kind of node according to the given index.
    private Node getNodeType (int index)
    {
        switch (index)
        {
            case 0:
                return new SelectorNode();
            case 1:
                return new SequenceNode();
            case 2:
                return new moveForwardNode();
            case 3:
                return new moveBackwardsNode();
            case 4:
                return new rotateLeftNode();
            case 5:
                return new rotateRightNode();
            default:
                return null;
        }
    }

	public void setNode(Node node)
    {
        this.node = node;
    }

    void addChild(Node node)
    {
        node.addChild(node);
    }
}
