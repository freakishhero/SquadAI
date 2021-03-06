﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    const float path_update_timer = 0.2f;
    float path_update_threshold = 0.5f;

    public float movement_speed = 20f;
    public float turn_speed = 3f;
    public float turn_distance = 5f;

    public List<Path> waypoints;

    public GameObject Target { get; set; }

    Path path;
    Rigidbody rb;

    void Start () {
        StartCoroutine(UpdatePath());
        Target = new GameObject();
        this.name = "Unit";
        Target.name = "Target ";
        rb = this.GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY 
            | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | 
            RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }

    public void OnPathFound(Vector3[] _waypoints, bool path_successful)
    {
        if(path_successful)
        {
            path = new Path(_waypoints, transform.position, turn_distance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    void FixedUpdate()
    {
        foreach(Selection unit in Selection.all_units)
        {
            if(Vector3.Distance(this.transform.position, unit.gameObject.transform.position) < 0.7f)
            {
              this.GetComponent<Rigidbody>().velocity -= unit.GetComponent<Rigidbody>().velocity;
            }
        }
    }

    protected void LateUpdate()
    {
        this.transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
    }

    IEnumerator FollowPath()
    {
        bool following_path = true;
        int path_index = 0;
        transform.LookAt(path.look_points[0]);

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX 
            | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;

        while (following_path)
        {
            Vector2 position_2D = new Vector2(transform.position.x, transform.position.z);
            while(path.turn_boundaries[path_index].HasCrossedLine(position_2D))
            {
                if(path_index == path.finish_line_index)
                {
                    following_path = false;
                    break;
                }
                    else
                {
                    path_index++;
                }
            }

            if(following_path)
            {
                Quaternion target_rotation = Quaternion.LookRotation(path.look_points[path_index] - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, turn_speed * Time.deltaTime);
                transform.Translate(Vector3.forward * movement_speed * Time.deltaTime, Space.Self);
            }
            yield return null;
        }
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }


    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
            yield return new WaitForSeconds(0.3f);

    

        float sqaure_move_threshhold = path_update_threshold * path_update_threshold;
        Vector3 old_target_position = Target.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(path_update_timer);

            if((Target.transform.position - old_target_position).sqrMagnitude > sqaure_move_threshhold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, Target.transform.position, OnPathFound));
                old_target_position = Target.transform.position;
            }
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            path.DrawWithGizmos();
        }

        Gizmos.DrawWireSphere(transform.position, 1.0f);
    }
}
