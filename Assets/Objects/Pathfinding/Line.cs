using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{

    const float vertical_line_gradient = 1e5f;

    float gradient;
    float y_intercept;
    Vector2 first_line_point;
    Vector2 second_line_point;

    bool approach_side;

    float perpendicular_gradient;

    public Line(Vector2 _line_point, Vector2 point_perpendicular_to_line)
    {
        float delta_x = _line_point.x - point_perpendicular_to_line.x;
        float delta_y = _line_point.y - point_perpendicular_to_line.y;

        approach_side = false;

        if (delta_x == 0)
            perpendicular_gradient = vertical_line_gradient;
        else
            perpendicular_gradient = delta_y / delta_x;


        if (perpendicular_gradient == 0)
            gradient = vertical_line_gradient;
        else
            gradient = -1 / perpendicular_gradient;

        y_intercept = _line_point.y - gradient * _line_point.x;

        first_line_point = _line_point;
        second_line_point = _line_point + new Vector2(1, gradient);

        approach_side = GetSide(point_perpendicular_to_line);
    }

    bool GetSide(Vector2 _point)
    {
        return (_point.x - first_line_point.x) * (second_line_point.y - first_line_point.y) > (_point.y - first_line_point.y) * (second_line_point.x - first_line_point.x);
    }

    public bool HasCrossedLine(Vector2 _point)
    {
        return GetSide(_point) != approach_side;
    }

    public void DrawWithGizmos(float _length)
    {
        Vector3 line_direction = new Vector3(1, 0, gradient).normalized;
        Vector3 line_centre = new Vector3(first_line_point.x, 0, first_line_point.y) + Vector3.up;
        Gizmos.DrawLine(line_centre - line_direction * _length / 2f, line_centre + line_direction * _length / 2f);
    }
}
