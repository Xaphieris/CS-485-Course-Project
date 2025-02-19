using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<TileProp> FindPath(TileProp startNode, TileProp targetNode) 
    {
        Debug.Log("Started Path finding");
        var toSearch = new List<TileProp>() { startNode };
        var processed = new List<TileProp>();

        while (toSearch.Any()) 
        {
            var current = toSearch[0];
            foreach (var t in toSearch) 
                if (t.F < current.F || t.F == current.F && t.H < current.H) current = t;

            processed.Add(current);
            toSearch.Remove(current);

            if (current == targetNode) 
            {
                var currentPathTile = targetNode;
                var path = new List<TileProp>();
                var count = 100;
                while (currentPathTile != startNode) 
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                    count--;
                    if (count < 0) throw new Exception();
                    Debug.Log("sdfsdf");
                }
                
                Debug.Log(path.Count);
                return path;
            }

            Debug.Log("Checking Neighbors");
            foreach (var neighbor in current.Neighbors.Where(t => t.isTraversable && !processed.Contains(t))) {
                var inSearch = toSearch.Contains(neighbor);

                var costToNeighbor = current.G + current.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.G) 
                {
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(current);

                    if (!inSearch) 
                    {
                        neighbor.SetH(neighbor.GetDistance(targetNode));
                        toSearch.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }
}
