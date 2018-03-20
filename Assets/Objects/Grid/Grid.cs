using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    [SerializeField]
    bool DisplayGridGizmos;

    [SerializeField]
    private LayerMask obstacle_mask;

    [SerializeField]
    private Vector2 grid_size;

    [SerializeField]
    float node_radius;

    private float node_diameter;

    [SerializeField]
    private int grid_spaces_x;

    [SerializeField]
    private int grid_spaces_y;

    private Node[,] grid;

    public int Max_Size
    {
        get
        {
            return grid_spaces_x * grid_spaces_y;
        }
    }

    void Awake()
    {
        node_diameter = node_radius * 2;
        grid_spaces_x = Mathf.RoundToInt(grid_size.x / node_diameter);
        grid_spaces_y = Mathf.RoundToInt(grid_size.y / node_diameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[grid_spaces_x, grid_spaces_y];
        Vector3 bottom_left_point = transform.position - Vector3.right * grid_size.x / 2 - Vector3.forward * grid_size.y / 2;

        for(int x = 0; x < grid_spaces_x; x++)
        {
            for (int y = 0; y < grid_spaces_y; y++)
            {
                Vector3 world_point = bottom_left_point + Vector3.right * (x * node_diameter + node_radius) + Vector3.forward * (y * node_diameter + node_radius);
                bool walkable = !(Physics.CheckSphere(world_point, node_radius, obstacle_mask));
                grid[x, y] = new Node(walkable, world_point, x, y);
            }
        }
    }

    void FixedUpdate()
    {
        foreach (Node node in grid)
        {
            node.Walkable = !(Physics.CheckSphere(node.World_Pos, node_radius, obstacle_mask));
        }
        
    }

    public Node GetNodeFromWorldPosition(Vector3 _world_pos) 
    {
        float x_scale = (_world_pos.x + grid_size.x / 2) / grid_size.x;
        float y_scale = (_world_pos.z + grid_size.y / 2) / grid_size.y;

        x_scale = Mathf.Clamp01(x_scale);
        y_scale = Mathf.Clamp01(y_scale);

        int x = Mathf.RoundToInt((grid_size.x - 1) * x_scale);
        int y = Mathf.RoundToInt((grid_size.y - 1) * y_scale);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node _node)
    {
        List<Node> neighbours = new List<Node>();
        
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int check_x = _node.X_Index + x;
                int check_y = _node.Y_Index + y;

                if(check_x >= 0 && check_x < grid_spaces_x && check_y >= 0 && check_y < grid_spaces_y)
                {
                    neighbours.Add(grid[check_x,check_y]);
                }
            }
        }
        return neighbours;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(grid_size.x, 1, grid_size.y));

            if (grid != null && DisplayGridGizmos)
            {
                foreach (Node node in grid)
                {
                    Gizmos.color = (node.Walkable) ? Color.green : Color.red;
                    Gizmos.DrawCube(node.World_Pos, Vector3.one * (node_diameter - 0.01f));
                }
            }
        }
}
