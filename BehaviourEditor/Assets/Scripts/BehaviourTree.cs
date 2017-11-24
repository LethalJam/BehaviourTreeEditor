using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Save system related
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;


public enum Response
{
    success, failure, running
}

[Serializable]
public class SerializedVector3
{
    public float x, y, z;
}

// Base class for a node
[Serializable]
public abstract class Node
{
    abstract public Response tick(ref TankBehaviour tank);

    public void addChild(Node node)
    {
        node.parent = this;
        children.Add(node);
    }
    public void removeChild(Node node)
    {
        children.Remove(node);
    }
    public int childAmount()
    {
        return children.Count;
    }
    public void destroy()
    {
        // Remove node from parents children.
        if (parent != null)
            parent.removeChild(this);

        // "Destroy" all children
        for (int i = 0; i < children.Count; i++)
        {
            children[i].destroy();
        }
        children.Clear();
    }
    public bool isEndnode()
    {
        return endNode;
    }
    public void setVisualPos(Vector3 pos)
    {
        serializedPos.x = pos.x;
        serializedPos.y = pos.y;
        serializedPos.z = pos.z;
    }
    public Vector3 getVisualPos()
    {
        Vector3 returnVec = new Vector3(serializedPos.x, serializedPos.y, serializedPos.z);
        return returnVec;
    }
    public bool hasParent()
    {
        return (parent != null);
    }
    public List<Node> children = new List<Node>();
    public string tipText = "Root";
    protected Node parent = null;
    protected bool endNode = false;
    // Vector used for serializing the position of the nodes button when saving and loading
    protected SerializedVector3 serializedPos = new SerializedVector3();
}
[Serializable]
public abstract class EndNode : Node
{
    protected EndNode()
    {
        endNode = true;
    }
}


// Selector node
[Serializable]
public class SelectorNode : Node
{
    public SelectorNode()
    {
        tipText = "Selector node executes its children from left to right. When a child returns success or running, it stops.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        // Iterate through all children to try and find one that succeeds or is running
        foreach (Node n in children)
        {
            Response childResponse;
            childResponse = n.tick(ref tank);

            if (childResponse == Response.running
                || childResponse == Response.success)
            {
                return childResponse;
            }
        }

        // If no such child was found, return failure.
        return Response.failure;
    }
    
}
// Sequence node
[Serializable]
public class SequenceNode : Node
{
    public SequenceNode()
    {
        tipText = "Sequence node executes its children from left to right. When a child returns failure or running, it stops.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        // Iterate through all children to try and find one that succeeds or is running
        foreach (Node n in children)
        {
            Response childResponse;
            childResponse = n.tick(ref tank);

            if (childResponse == Response.running
                || childResponse == Response.failure)
            {
                return childResponse;
            }
        }

        // If no such child was found, return failure.
        return Response.success;
    }
}
// Node for checking if there is anything in front of the tank
[Serializable]
public class checkForwardNode : EndNode
{
    public checkForwardNode()
    {
        tipText = "This node checks if the tank is obsctructed in the front. If so, return success. If not, return failure.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        if (tank.checkObstructed(tank.transform.up))
            return Response.success;
        else
            return Response.failure;
    }
}
[Serializable]
public class checkBackwardsNode : EndNode
{
    public checkBackwardsNode()
    {
        tipText = "This node checks if the tank is obsctructed in the back. If so, return success. If not, return failure.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        if (tank.checkObstructed(-tank.transform.up))
            return Response.success;
        else
            return Response.failure;
    }
}
[Serializable]
public class checkRightNode : EndNode
{
    public checkRightNode()
    {
        tipText = "This node checks if the tank is obsctructed to the right. If so, return success. If not, return failure.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        if (tank.checkObstructed(tank.transform.right))
            return Response.success;
        else
            return Response.failure;
    }
}
[Serializable]
public class checkLeftNode : EndNode
{
    public checkLeftNode()
    {
        tipText = "This node checks if the tank is obsctructed to the left. If so, return success. If not, return failure.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        if (tank.checkObstructed(-tank.transform.right))
            return Response.success;
        else
            return Response.failure;
    }
}

// Tank movements
[Serializable]
public class moveForwardNode : EndNode
{
    public moveForwardNode()
    {
        tipText = "Moves the tank forward. Always returns success.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        tank.moveForward();
        return Response.success;
    }
}
[Serializable]
public class moveBackwardsNode : EndNode
{
    public moveBackwardsNode()
    {
        tipText = "Moves the tank backwards. Always returns success.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        tank.moveBackwards();
        return Response.success;
    }
}
[Serializable]
public class rotateLeftNode : EndNode
{
    public rotateLeftNode()
    {
        tipText = "Rotates the tank to the left. Always returns success.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        tank.rotateLeft();
        return Response.success;
    }
}
[Serializable]
public class rotateRightNode : EndNode
{
    public rotateRightNode()
    {
        tipText = "Rotates the tank to the right. Always returns success.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        tank.rotateRight();
        return Response.success;
    }
}

// Other Tank actions
[Serializable]
public class shootNode : EndNode
{
    public shootNode()
    {
        tipText = "Makes the tank shoot. Always returns success.";
    }
    public override Response tick(ref TankBehaviour tank)
    {
        tank.shoot();
        return Response.success;
    }
}

public class BehaviourTree : MonoBehaviour {

    public Button saveButton;
    public Button loadButton;

    private TankBehaviour tank;
    private Node root = new SelectorNode();

    string directoryPath;
    string savingPath;

	// Initialize relevant variables for tree.
	void Awake ()
    {
        directoryPath = "./SavedTrees";
        savingPath = directoryPath + "/behaviourData.tree";
        // Attempt to get the tank behaviour script of the gameobject
        // If no such script exists, disable the component
        tank = gameObject.GetComponent<TankBehaviour>();
        if (tank == null)
            this.enabled = false;

        // Set correct button functions
        if (saveButton == null || loadButton == null)
        {
            Debug.LogError("Error: Save and/or load buttons not assigned to behaviourtree");
        }
        else
        {
            saveButton.onClick.AddListener(saveTree);
            loadButton.onClick.AddListener(loadTree);
        }
	}
	
    public Node getRoot()
    {
        return root; 
    }

	// Update each frame by sending a signal through the root-node.
	void Update () {
        root.tick(ref tank);
	}

    // Save function to serialize and save down treedata in file
    public void saveTree()
    {
        if (!File.Exists(savingPath))
        {
            Debug.Log("No file exists, creating file.");
            Directory.CreateDirectory(directoryPath);
            FileStream newFile = File.Create(savingPath);
            newFile.Close();
        }

        Debug.Log("Saving file...");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(savingPath, FileMode.Open);

        TreeData data = new TreeData();
        data.rootNode = root;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("File saved!");
    }
    // Load function to deserialize and load data from file
    public void loadTree()
    {
        if (File.Exists(savingPath))
        {
            BehaviourEditor editor = GameObject.FindGameObjectWithTag("Editor").GetComponent<BehaviourEditor>();
            Debug.Log("Loading file...");

            
            editor.deleteTreeButtons();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savingPath, FileMode.Open);
            TreeData data = (TreeData)bf.Deserialize(file);
            root = data.rootNode;
            editor.reconstructInterface(root);
            file.Close();

            Debug.Log("File loaded!");
        }
        else
        {
            Debug.Log("No savefile found.");
        }
    }
}

[Serializable]
public class TreeData
{
    public Node rootNode;
}