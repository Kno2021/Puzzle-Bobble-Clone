using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    private NodeIndex _nodeIndex;
    private Vector3 _nodePosition;
    public Vector3 NodePosition { get { return _nodePosition; } }
    private int _type;
    public int NodeType { get { return _type; } }

    private Ball _ball;
    public Ball Ball { get { return _ball; } }

    private List<Node> _neighbors = new();

    public Node(NodeIndex index, Vector3 nodePosition)
    {
        _nodeIndex = index;
        _nodePosition = nodePosition;
        _type = -1;
    }
   
    public void SetNodeType(Ball ball)
    {
        _ball = ball;
        if (ball)
        {
            _type = _ball.BallType;
        }
        else
            _type = -1;

    }

    public Ball ResetNodeType()
    {
        _type = -1;
        return _ball;
    }

    public NodeIndex GetNodeIndex()
    {
        return _nodeIndex;
    }

    public void UpdateNodeConnections(List<Node> neighbors)
    {
        _neighbors = neighbors;
    }

    public List<Node> GetNeighbors()
    {
        return _neighbors;
    }

}


