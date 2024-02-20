using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node : IComparable<Node>
{

    public bool isWalkable;
    public Vector3 position;
    public int gridX;
    public int gridY;

    public int gCost; //A 알고리즘의 경로값 G(n) 
    public int hCost; //A 알고리즘의 휴리스틱 H(n) 
    public Node parent;

    public Node(bool walkable, Vector3 position, int gridX, int gridY)
    {
        this.isWalkable = walkable;
        this.position = position;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int CompareTo(Node otherNode)
    {
        if (this.fCost < otherNode.fCost) return -1;
        else if (this.fCost > otherNode.fCost) return 1;
        else return 0;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
