using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] Waypoint startPoint, endPoint;
    Dictionary<Vector2Int,Waypoint> grid = new Dictionary<Vector2Int, Waypoint>();
    Queue<Waypoint> queue = new Queue<Waypoint>();
    bool isRunning = true;
    Waypoint searchPoint;
    [SerializeField] List<Waypoint> path = new List<Waypoint>();

    Vector2Int[] directions = {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public List<Waypoint> GetPath()
    {
        if(path.Count == 0)
        {
            CalculatePath();
        }
            return path;
        
    }

    private void CalculatePath()
    {
        LoadBlocks();
        SetColorStartAndEnd(); // Move out
        PathfindAlgorithm();
        CreatePath();
    }
    private void CreatePath()
    {
        AddPointToPath(endPoint);

        Waypoint prevPoint = endPoint.exploredFrom;
        while(prevPoint != startPoint)
        {
            prevPoint.SetTopColor(Color.gray);
            AddPointToPath(prevPoint);
            prevPoint = prevPoint.exploredFrom;
        }
        AddPointToPath(startPoint);
        path.Reverse();
    }

    private void AddPointToPath(Waypoint waypoint)
    {
        path.Add(waypoint);
        waypoint.isPlaceable = false;
    }

    private void PathfindAlgorithm()
    {
        queue.Enqueue(startPoint);
        while(queue.Count > 0 && isRunning == true)
        {
              searchPoint = queue.Dequeue();
              searchPoint.isExplored = true;
              CheckForEndpoint();
              ExploreNearPoints();
        }
    }

    private void CheckForEndpoint()
    {
        if(searchPoint == endPoint)
        {
            isRunning = false;
        }
    }

    private void ExploreNearPoints()
    {
        if(!isRunning) { return; }
        
        foreach(Vector2Int direction in directions)
        {
            Vector2Int nearPointCoordinates = searchPoint.GetGridPos() + direction;

            if(grid.ContainsKey(nearPointCoordinates))
            {
                Waypoint nearPoint = grid[nearPointCoordinates];
                AddPointToQueue(nearPoint);
            }
        }
    }

    private void AddPointToQueue(Waypoint nearPoint)
    {
        if(nearPoint.isExplored || queue.Contains(nearPoint))
        {
            return;
        }
        else
        {
            queue.Enqueue(nearPoint);
            nearPoint.exploredFrom = searchPoint;
        }
    }
    private void LoadBlocks()
    {
        var waypoints = FindObjectsOfType<Waypoint>();
        foreach(Waypoint waypoint in waypoints)
        {
            Vector2Int gridPos = waypoint.GetGridPos();
            if(grid.ContainsKey(gridPos))
            {
                Debug.LogWarning("Повтор блока : " + waypoint);
            }
            else
            {
                grid.Add(gridPos,waypoint);
            }
        }
    }

    void SetColorStartAndEnd()
    {
        startPoint.SetTopColor(Color.green);
        endPoint.SetTopColor(Color.red);
    }
}
