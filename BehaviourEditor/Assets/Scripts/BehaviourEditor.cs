using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourEditor : MonoBehaviour {
    
    // Public variables to be assigned in Unity editor.
    public BehaviourTree tree;
    public GameObject nodeButton;

    private Node targetNode;
    private Vector3 rootPosition;
    private List<GameObject> nodeButtons;
    private GameObject canvas;
    private GameObject buttons;
    private object newNodeObject;

    // Use this for initialization
    void Awake()
    {
        // Find gameobject and save the dropdown object as well if there is one.
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
            Debug.LogError("No canvas object found.");
        buttons = GameObject.FindGameObjectWithTag("Buttons");
        if (buttons == null)
            Debug.LogError("No buttons gameobject found.");


        nodeButtons = new List<GameObject>();
        rootPosition = new Vector3(Screen.width / 2, Screen.height / 2 + Screen.height / 4, 0);
        if (tree == null)
        {
            this.enabled = false;
        }
        // Create initial node button from the root of the tree.
        GameObject nodeObj = GameObject.Instantiate(nodeButton);
        nodeObj.transform.SetParent(buttons.transform);
        nodeObj.transform.SetPositionAndRotation(rootPosition, nodeObj.transform.rotation);
        nodeObj.GetComponent<NodeManipulator>().setNode(tree.getRoot());
        nodeObj.GetComponent<NodeManipulator>().setIndestructable(true);
        nodeObj.transform.GetChild(0).GetComponent<Text>().text = "RootSelecter";
        nodeObj.GetComponent<NodeManipulator>().setTipText(tree.getRoot().tipText);
        tree.getRoot().setVisualPos(nodeObj.transform.position);
        nodeButtons.Add(nodeObj);
    }

    // Delete the list of buttons
    public void deleteTreeButtons()
    {
        for (int i = 0; i < nodeButtons.Count; i++)
        {
            Destroy(nodeButtons[i]);
        }
        nodeButtons.Clear();
    }

    // Setup new node and initialize its values.
    public GameObject createNewNode(Node node, Vector2 position)
    {
        GameObject nodeObj = GameObject.Instantiate(nodeButton);
        nodeObj.transform.SetParent(buttons.transform);
        nodeObj.transform.SetPositionAndRotation(new Vector3(position.x, position.y, 0), nodeObj.transform.rotation);
        nodeObj.GetComponent<NodeManipulator>().setNode(node);
        nodeObj.GetComponent<NodeManipulator>().setTipText(node.tipText);
        nodeObj.transform.GetChild(0).GetComponent<Text>().text = node.GetType().Name;
        node.setVisualPos(nodeObj.transform.position);
        nodeButtons.Add(nodeObj);
        return nodeObj;
    }

    public GameObject reconstructInterface(Node rootNode)
    {
        
        GameObject nodeObj = createNewNode(rootNode, rootNode.getVisualPos());
        if (!rootNode.hasParent())
        {
            nodeObj.GetComponent<NodeManipulator>().setIndestructable(true);
            nodeObj.transform.GetChild(0).GetComponent<Text>().text = "Root";
        }

        List<Node> nodeChildren = rootNode.children;
        for (int i = 0; i < nodeChildren.Count; i++)
        {
            // Create line renderer to visually represent the tree structure.
            UILineRenderer line = GameObject.Instantiate(Resources.Load("UILine", typeof(GameObject)) as GameObject).GetComponent<UILineRenderer>();
            line.transform.SetParent(canvas.transform);
            line.setAttachmentOffsets(new Vector3(-40 + 10 * i, -10, 0), new Vector3(0, 10, 0));
            line.pointA = nodeObj.transform;
            line.pointB = reconstructInterface(nodeChildren[i]).transform;
        }
        return nodeObj;
    }
}
