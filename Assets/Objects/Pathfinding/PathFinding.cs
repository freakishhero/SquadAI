using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class PathFinding : MonoBehaviour {

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public void FindPath(PathRequest _request, Action<PathResult> _callback)
    {
        Vector3[] waypoints = new Vector3[0];
        bool path_success = false;

        Node start_node = grid.GetNodeFromWorldPosition(_request.path_start);
        Node end_node = grid.GetNodeFromWorldPosition(_request.path_end);

        if (end_node.Walkable)
        {
            Heap<Node> open_nodes = new Heap<Node>(grid.Max_Size);
            HashSet<Node> closed_nodes = new HashSet<Node>();

            open_nodes.Add(start_node);

            while (open_nodes.Count > 0)
            {
                Node current_node = open_nodes.RemoveFirst();
                closed_nodes.Add(current_node);

                if (current_node == end_node)
                {
                    path_success = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(current_node))
                {
                    if (!neighbour.Walkable || closed_nodes.Contains(neighbour))
                    {
                        continue;
                    }

                    int movement_cost_to_neighbour = current_node.G_Cost + GetDistance(current_node, neighbour);

                    if (movement_cost_to_neighbour < neighbour.G_Cost || !open_nodes.Contains(neighbour))
                    {
                        neighbour.G_Cost = movement_cost_to_neighbour;
                        neighbour.H_Cost = GetDistance(neighbour, end_node);
                        neighbour.Parent = current_node;

                        if (!open_nodes.Contains(neighbour))
                        {
                            open_nodes.Add(neighbour);
                        }
                        else
                        {
                            open_nodes.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
  
        if(path_success)
        {
            waypoints = RetracePath(start_node, end_node);
            path_success = waypoints.Length > 0;
        }

        _callback(new PathResult(waypoints, path_success, _request.callback));
    }

    Vector3[] RetracePath(Node _start_node, Node _end_node)
    {
        List<Node> path = new List<Node>();
        Node current_node = _end_node;

        while(current_node != _start_node)
        {
            path.Add(current_node);
            current_node = current_node.Parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> _path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 old_direction = Vector2.zero;

        for(int i = 1; i < _path.Count; i++)
        {
            Vector2 new_direction = new Vector2(_path[i - 1].X_Index - _path[i].X_Index, _path[i - 1].Y_Index - _path[i].Y_Index);
            if(new_direction != old_direction)
            {
                waypoints.Add(_path[i].World_Pos);
            }
            old_direction = new_direction;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node _node_1, Node _node_2)
    {
        int x_dinsance = Mathf.Abs(_node_1.X_Index - _node_2.X_Index);
        int y_dinsance = Mathf.Abs(_node_1.Y_Index - _node_2.Y_Index);

        if (x_dinsance > y_dinsance)
            return 14 * y_dinsance + 10 * (x_dinsance - y_dinsance);
        else
            return 14 * x_dinsance + 10 * (y_dinsance - x_dinsance);
    }
}
