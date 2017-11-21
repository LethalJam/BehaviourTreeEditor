using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Response
{
    success, failure, running
}

// Base class for a node
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
public class moveForwardNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.moveForward();
        return Response.running;
    }
}
public class moveBackwardsNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.moveBackwards();
        return Response.running;
    }
}
public class rotateLeftNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.rotateLeft();
        return Response.running;
    }
}
public class rotateRightNode : Node
{
    public override Response tick(ref TankBehaviour tank)
    {
        tank.rotateRight();
        return Response.running;
    }
}

public class BehaviourTree : MonoBehaviour {

    private TankBehaviour tank;
    private Node root = new SelectorNode();

	// Initialize relevant variables for tree.
	void Awake () {

        // Attempt to get the tank behaviour script of the gameobject
        // If no such script exists, disable the component
        tank = gameObject.GetComponent<TankBehaviour>();
        if (tank == null)
            this.enabled = false;
        //testConstructor();
	}
	
    public Node getRoot()
    {
        return root; 
    }

	// Update each frame by sending a signal through the root-node.
	void Update () {
        root.tick(ref tank);
	}

    // Simple behaviour for testing purposes
    void testConstructor()
    {
        SequenceNode origin = new SequenceNode();

        SelectorNode newSelect = new SelectorNode();
        newSelect.addChild(new checkForwardNode());
        newSelect.addChild(new moveForwardNode());
        origin.addChild(newSelect);

        origin.addChild(new rotateRightNode());

        root.addChild(origin);
    }
}
