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
    abstract public Response tick();
    protected List<Node> children = new List<Node>();
}

// Selector node
public class SelectorNode : Node
{
    public override Response tick()
    {
        // Iterate through all children to try and find one that succeeds or is running
        foreach (Node n in children)
        {
            Response childResponse;
            childResponse = n.tick();

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
    public override Response tick()
    {
        // Iterate through all children to try and find one that succeeds or is running
        foreach (Node n in children)
        {
            Response childResponse;
            childResponse = n.tick();

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
    public override Response tick()
    {
        throw new NotImplementedException();
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
	}
	
	// Update each frame by sending a signal through the root-node.
	void Update () {
        root.tick();
	}
}
