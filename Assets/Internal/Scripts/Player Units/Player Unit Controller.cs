using System.Collections.Generic;
using System.Collections;

using UnityEngine;

public class PlayerUnitController : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Traverse(GameObject lastSelection, GameObject currentSelection)
    {
        GameObject unit = GameObject.Find("Player Unit 1");
        Vector3 tilePos;

        int count;
        float xDiff, zDiff;
        float moveSpeed = 0.001f;

        TileProp lastSelTileProp = lastSelection.GetComponent<TileProp>();
        TileProp currentSelTileProp = currentSelection.GetComponent<TileProp>();
        
        List<TileProp> path = PathFinding.FindPath(lastSelTileProp, currentSelTileProp);

        //unit.transform.position = currentSelection.transform.position;
        path.Reverse();

        //This runs all within one frame, that is a problem
        //Need to move a little bit each frame, not all at once on a stack
        //Need to send to unit script?
        foreach (var t in path)
        {
            count = 0;

            Debug.Log("Tile Traversed: " + t.tileNumX + ", " + t.tileNumZ);
            tilePos = t.transform.position;
            unit.transform.LookAt(t.transform);
            
            xDiff = tilePos.x - unit.transform.position.x;
            zDiff = tilePos.z - unit.transform.position.z;

            while(unit.transform.position.x < tilePos.x)
            {
                unit.transform.position += new Vector3(xDiff * moveSpeed * Time.deltaTime, 0, zDiff * moveSpeed  * Time.deltaTime);

                Debug.Log("Unit Position: " + unit.transform.position.x + ", " + unit.transform.position.z);

                count++;
            }
        }

        lastSelection.GetComponent<TileProp>().hasPlayerUnit = false;
        currentSelection.GetComponent<TileProp>().hasPlayerUnit = true;

        Debug.Log("Traversed");
    }
}
