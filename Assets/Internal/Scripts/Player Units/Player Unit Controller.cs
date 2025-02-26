using System.Collections.Generic;
using System.Collections;

using UnityEngine;

public class PlayerUnitController : MonoBehaviour
{
    Vector3 tilePos;

    float xDiff, zDiff, distance;
    public float moveSpeed = 0.1f;

    private IEnumerator moveU;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Funtion to call the Coroutine to move a unit
    public void Traverse(GameObject lastSelection, GameObject currentSelection, List<TileProp> path)
    {
        //unit.transform.position = currentSelection.transform.position;
        path.Reverse();
        moveU = moveUnit(path);
        StartCoroutine(moveU);

        lastSelection.GetComponent<TileProp>().hasPlayerUnit = false;
        currentSelection.GetComponent<TileProp>().hasPlayerUnit = true;
        currentSelection.GetComponent<TileProp>().unit = gameObject;
    }

    //Move player unit according to MoveSpeed. Look at next tile, move 1 movespeed towards target, repeat each frame
    //When tile is reached, move unit the rest of the way to the target to avoid overshoot. Get next tile, repeat process
    private IEnumerator moveUnit(List<TileProp> path)
    {
        Debug.Log("Started Coroutine");
        foreach (var t in path)
        {
            Debug.Log("Tile Traversed: " + t.tileNumX + ", " + t.tileNumZ);

            tilePos = t.transform.position;
            this.transform.LookAt(t.transform);
            
            xDiff = tilePos.x - this.transform.position.x;
            zDiff = tilePos.z - this.transform.position.z;

            distance = Vector3.Distance(this.transform.position, tilePos);

            while(distance >= moveSpeed)
            {
                distance = Vector3.Distance(this.transform.position, tilePos);
                if(distance <= moveSpeed)
                {
                    this.transform.position = tilePos;
                }
                else
                {
                    this.transform.position += new Vector3(xDiff * moveSpeed, 0, zDiff * moveSpeed);
                }


                Debug.Log("Unit Position: " + this.transform.position.x + ", " + this.transform.position.z);

                yield return null;
            }
        }

        Debug.Log("Finished Corountine");
        yield return null;
    }
}
