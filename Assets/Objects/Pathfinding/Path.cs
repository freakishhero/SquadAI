using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly Vector3[] look_points;
    public readonly Line[] turn_boundaries;
    public readonly int finish_line_index;

    public Path(Vector3[] _waypoints, Vector3 _start_pos, float _turn_distance)
    {
        look_points = _waypoints;
        turn_boundaries = new Line[look_points.Length];
        finish_line_index = turn_boundaries.Length - 1;

        Vector2 previous_point = Vector3ToVector2(_start_pos);
        for (int i = 0; i < look_points.Length; i++)
        {
            Vector2 current_point = Vector3ToVector2(look_points[i]);
            Vector2 direction_to_current_point = (current_point - previous_point).normalized;
            Vector2 turn_boundary_point = (i == finish_line_index)? current_point : current_point - direction_to_current_point * _turn_distance;
            turn_boundaries[i] = new Line(turn_boundary_point, previous_point - direction_to_current_point * _turn_distance);
            previous_point = turn_boundary_point;
        }

    }

    Vector2 Vector3ToVector2(Vector3 _vector3)
    {
        return new Vector2(_vector3.x, _vector3.z);
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 point in look_points)
        {
            Gizmos.DrawCube(point + Vector3.up, Vector3.one);
        }

        Gizmos.color = Color.white;
        foreach (Line line in turn_boundaries)
        {
            line.DrawWithGizmos(10);
        }
    }
        
}
