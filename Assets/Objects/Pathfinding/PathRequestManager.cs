using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PathRequestManager : MonoBehaviour {

    Queue<PathResult> results = new Queue<PathResult>();
    PathFinding path_finding;
    static PathRequestManager instance;

    void Awake()
    {
        instance = this;
        path_finding = GetComponent<PathFinding>();
    }

    void Update()
    {
        if(results.Count > 0)
        {
            int items_in_queue = results.Count;
            lock(results)
            {
                for (int i = 0; i < items_in_queue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }

	public static void RequestPath(PathRequest _request)
    {
        ThreadStart thread_start = delegate
        {
            instance.path_finding.FindPath(_request, instance.FinishedProcessingPath);
        };
        thread_start.Invoke();
    }

    public void FinishedProcessingPath(PathResult _result)
    {
        lock (results)
        {
            results.Enqueue(_result);
        }
        
    }
}

public struct PathResult
{
    public Vector3[] path;
    public bool success;
    public Action<Vector3[], bool> callback;

    public PathResult(Vector3[] _path, bool _success, Action<Vector3[], bool> _callback)
    {
        path = _path;
        success = _success;
        callback = _callback;
    }
}

public struct PathRequest
{
    public Vector3 path_start;
    public Vector3 path_end;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 _path_start, Vector3 _path_end, Action<Vector3[], bool> _callback)
    {
        path_start = _path_start;
        path_end = _path_end;
        callback = _callback;
    }
}