using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public Vector3 World_Pos { get; set; }

    public bool Walkable { get; set; }
    public int G_Cost { get; set; }
    public int H_Cost { get; set; }
    public int X_Index { get; set; }
    public int Y_Index { get; set; }

    public int Heap_Index { get; set; }

    public Node Parent { get; set; }

    public Node(bool _walkable, Vector3 _world_pos, int _x, int _y)
    {
        Walkable = _walkable;
        World_Pos = _world_pos;
        X_Index = _x;
        Y_Index = _y; 
    }

    public int F_Cost
    {
        get
        {
            return G_Cost + H_Cost;
        }
    }

    public int CompareTo(Node _node)
    {
        int compare = F_Cost.CompareTo(_node.F_Cost);

        if (compare == 0)
        {
            compare = H_Cost.CompareTo(_node.H_Cost);
        }
        return -compare;
    }
}
