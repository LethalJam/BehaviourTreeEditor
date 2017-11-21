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
    protected List<Node> children = new List<Node>();
    protected Node parent = null;
}

// Selector node
[Serializable]
public class SelectorNode : Node
{
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
public class checkForwardNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        if (tank.checkObstructed(tank.transform.up))
            return Response.success;
        else
            return Response.failure;
    }
}
[Serializable]
public class checkBackwardsNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        if (tank.checkObstructed(-tank.transform.up))
            return Response.success;
        else
            return Response.failure;
    }
}
[Serializable]
public class checkRightNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        if (tank.checkObstructed(tank.transform.right))
            return Response.success;
        else
            return Response.failure;
    }
}
[Serializable]
public class checkLeftNode : Node
{
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
public class moveForwardNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.moveForward();
        return Response.running;
    }
}
[Serializable]
public class moveBackwardsNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.moveBackwards();
        return Response.running;
    }
}
[Serializable]
public class rotateLeftNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.rotateLeft();
        return Response.running;
    }
}
[Serializable]
public class rotateRightNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.rotateRight();
        return Response.running;
    }
}


public class BehaviourTree : MonoBehaviour {

    public Button saveButton;
    public Button loadButton;

    private TankBehaviour tank;
    private Node root = new SelectorNode();

	// Initialize relevant variables for tree.
	void Awake () {

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
        if (!File.Exists(Application.persistentDataPath + "/behaviourTreeData.dat"))
        {
            Debug.Log("No file exists, creating file.");
            //BinaryFormatter bf = new BinaryFormatter();
            FileStream newFile = File.Create(Application.persistentDataPath + "/behaviourTreeData.dat");
            newFile.Close();
        }

        Debug.Log("Saving file...");
        // EXEMPELKOD LÅNGT FRÅN FÄRDIG
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/behaviourTreeData.dat", FileMode.Open);

        TreeData data = new TreeData();
        data.rootNode = root;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("File saved!");
    }
    // Load function to deserialize and load data from file
    public void loadTree()
    {
        if (File.Exists(Application.persistentDataPath + "/behaviourTreeData.dat"))
        {
            Debug.Log("Loading file...");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/behaviourTreeData.dat", FileMode.Open);
            TreeData data = (TreeData)bf.Deserialize(file);
            root = data.rootNode;
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