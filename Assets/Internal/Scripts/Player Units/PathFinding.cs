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
        Debug.Log("Started Path finding (Start, Target): " + startNode.transform.name + ", " + targetNode.transform.name);

        //Establish lists to track what is to be processed and what has been processed
        var toSearch = new List<TileProp>() { startNode };
        var processed = new List<TileProp>();

        //Reset G value in case of carryover
        startNode.SetG(0);

        //While there are any nodes to search
        while (toSearch.Any()) 
        {
            //Set the current node to the first entry of the toSearch list
            var current = toSearch[0];
            //Debug.Log("Current Node in path: " + current.transform.name);
            
            //Check all in toSearch against the current node. If the F(combination of range from unit and range from target) 
            //      ..value is less than the current, switch to that node. If F is equal, then check the H (range from target) value
            //
            // At the start there is only one node to check, so this is effectively skiped
            foreach (var t in toSearch) 
                if (t.F < current.F || t.F == current.F && t.H < current.H)
                {
                    current = t;
                    //Debug.Log("Changed to Node: " + current.transform.name + "(" + current.G + ", " + current.H + ", " + current.F + ")");
                }

            // Add the current node to the list of nodes processed
            // Remove the current node from the list to be searched
            processed.Add(current);
            toSearch.Remove(current);

            // If the current node is the target (end node) trace back from the target node, along Connections, to the start node
            // Add each tile to the path list
            // If the path would continue forever, throw error
            // Return the path
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
                    //Debug.Log("sdfsdf");
                }
                
                Debug.Log("Finished Path finding: " + path.Count);
                return path;
            }

            //Debug.Log("Checking Neighbors");
            // Check each neighbor
            // 
            foreach (var neighbor in current.Neighbors.Where(t => t.Traversability() && !processed.Contains(t))) {
                
                //Check if neighbor is within the search list
                var inSearch = toSearch.Contains(neighbor);

                //Compute cost to neighbor (g)
                var costToNeighbor = current.G + current.GetDistance(neighbor);
                //Debug.Log("Distance to neighbor: " + current.GetDistance(neighbor));

                //If the neighbor is not in the search list or the current cost to that neighbor is less than the g value stored in that neighbor
                // Change the G cost and link it to the current node
                if (!inSearch || costToNeighbor < neighbor.G) 
                {
                    neighbor.SetG(costToNeighbor);

                    //Tracks the current path
                    neighbor.SetConnection(current);

                    //If the neighbor is not in the search list
                    //Set H value, distance from the target node and add it to the toSearch list
                    if (!inSearch) 
                    {
                        neighbor.SetH(neighbor.GetDistance(targetNode));
                        toSearch.Add(neighbor);
                    }
                }

                //Debug.Log("Neighbor: " + neighbor.transform.name + "(" + neighbor.G + ", " + neighbor.H + ", " + neighbor.F + ")");
            }
        }

        //If path fails
        return null;
    }
}
