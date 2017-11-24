using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeManipulator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    private Node node;
    private Button button;
    private GameObject canvas;
    private Dropdown nodeDropDown;
    private GameObject tooltip;
    private Text tipText;
    private BehaviourEditor editorInstance;
    private bool indestructable = false;

    // Moving variables
    private string tipString;
    private bool isSelected = false;
    private bool mouseHovering = false;
    private Vector3 tooltipHidePos = new Vector3(10000, 0, 0);

    public void setIndestructable(bool indestructable)
    {
        this.indestructable = indestructable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseHovering = true;
        tipText.text = tipString;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.transform.position = tooltipHidePos;
        mouseHovering = false;        
    }

    public void OnPointerClick (PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Middle && !indestructable)
        {
            node.destroy();
            tooltip.transform.position = tooltipHidePos;
            Destroy(this.gameObject);
        }
    }

    // Check right mousebutton on press and release
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            isSelected = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            isSelected = false;
            node.setVisualPos(transform.position);
        }

    }

    public void Update()
    {
        if (isSelected)
        {
            transform.position = Input.mousePosition;
        }
        if (mouseHovering)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

    // Set initial values
    void Awake ()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(chooseNode);

        tooltip = GameObject.FindGameObjectWithTag("Tooltip");
        if (tooltip == null)
            Debug.LogError("No tooltip gameobject was found");
        else
        {
            tipText = tooltip.transform.GetChild(0).GetComponent<Text>();
            if (tipText == null)
                Debug.Log("No text was found on tooltip gameobject.");
        }

        editorInstance = GameObject.FindGameObjectWithTag("Editor").GetComponent<BehaviourEditor>();
        if (editorInstance == null)
            Debug.LogError("No editor gameobject was found!");

        // Find gameobject and save the dropdown object as well if there is one.
        canvas = GameObject.Find("Canvas");
        if (canvas == null)
            Debug.LogError("No canvas object found.");

        nodeDropDown = canvas.GetComponentInChildren<Dropdown>(true);
        if (nodeDropDown == null)
            Debug.LogError("NO DROPDOWN FOUND: ABORT MISSION.");
    }

    // Start process of selecting and creating new node in the behaviourtree.
    public void chooseNode()
    {
        // Check to see if the editorCanvas is active.
        if (canvas != null && editorInstance != null && nodeDropDown != null && !node.isEndnode())
        {
            if (canvas.activeSelf)
            {
                // Create new node object
                Node newNode = getNodeType(nodeDropDown.value);
                Vector3 offsetVector = new Vector3(0, -50, 0);
                GameObject newNodeObject = editorInstance.createNewNode(newNode, gameObject.transform.position + offsetVector);
                

                // Create line renderer to visually represent the tree structure.
                UILineRenderer line = GameObject.Instantiate(Resources.Load("UILine", typeof(GameObject)) as GameObject).GetComponent<UILineRenderer>();
                line.transform.SetParent(canvas.transform);
                line.setAttachmentOffsets(new Vector3(-40 + 10*node.childAmount(), -10, 0), new Vector3(0, 10, 0));
                line.pointA = transform;
                line.pointB = newNodeObject.transform;

                // Add the node as a child to the current node
                addChild(newNode);
            }
        }
    }

    public void setTipText(string text)
    {
        tipString = text;
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
            case 6:
                return new checkForwardNode();
            case 7:
                return new checkBackwardsNode();
            case 8:
                return new checkRightNode();
            case 9:
                return new checkLeftNode();
            case 10:
                return new shootNode();
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
        this.node.addChild(node);
    }
    void removeChild(Node node)
    {

    }
}
