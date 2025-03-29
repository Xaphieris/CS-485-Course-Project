using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.Tilemaps;

public class PlayerUnitController : MonoBehaviour
{
    Vector3 tilePos;

    float xDiff, zDiff, distance;

    //Proportional speed, max of 1
    public float moveSpeed = 1.5f;

    private IEnumerator moveU;
    private IEnumerator Attack_1AN;

    private int tilesCrossed;
    public GameObject bullet;
    public GameObject PACC;
    private PointAndClickController pointAndClick;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PACC == null)
        {
            PACC = GameObject.Find("PointAndClickController");
        }
        if(pointAndClick == null)
        {
            pointAndClick = PACC.GetComponent<PointAndClickController>();
        }
    }


    //Funtion to call the Coroutine to move a unit
    public void Traverse(GameObject lastSelection, GameObject currentSelection, List<TileProp> path)
    {
        // Make sure there is actually a path to traverse
        if(path != null)
        {
            //unit.transform.position = currentSelection.transform.position;
            path.Reverse();
            moveU = moveUnit(path);
            StartCoroutine(moveU);

            lastSelection.GetComponent<TileProp>().hasPlayerUnit = false;

            if(path.Count >= this.transform.GetComponent<PlayerProp>().moveRange)
            {
                path[this.transform.GetComponent<PlayerProp>().moveRange - 1].hasPlayerUnit = true;
                path[this.transform.GetComponent<PlayerProp>().moveRange - 1].unit = gameObject;
                gameObject.GetComponent<PlayerProp>().tile = path[this.transform.GetComponent<PlayerProp>().moveRange - 1].transform.gameObject;
            }
            else
            {
                path[path.Count - 1].hasPlayerUnit = true;
                path[path.Count - 1].unit = gameObject;
                gameObject.GetComponent<PlayerProp>().tile = path[path.Count - 1].transform.gameObject;
            }
        }



        //Assumes inf move speed
        //currentSelection.GetComponent<TileProp>().hasPlayerUnit = true;
        //currentSelection.GetComponent<TileProp>().unit = gameObject;
    }

    //Move player unit according to MoveSpeed. Look at next tile, move 1 movespeed towards target, repeat each frame
    //When tile is reached, move unit the rest of the way to the target to avoid overshoot. Get next tile, repeat process
    private IEnumerator moveUnit(List<TileProp> path)
    {
        tilesCrossed = 0;
        //Debug.Log("Started Coroutine");
        foreach (var t in path)
        {
           // Debug.Log("Tile Traversed: " + t.tileNumX + ", " + t.tileNumZ);

            tilePos = t.transform.position;
            this.transform.LookAt(t.transform);
            
            xDiff = tilePos.x - this.transform.position.x;
            zDiff = tilePos.z - this.transform.position.z;

            distance = Vector3.Distance(this.transform.position, tilePos);

            while(distance >= moveSpeed * Time.deltaTime)
            {
                distance = Vector3.Distance(this.transform.position, tilePos);
                if(distance <= moveSpeed * Time.deltaTime)
                {
                    this.transform.position = tilePos;
                }
                else
                {
                    this.transform.position += new Vector3(xDiff * moveSpeed * Time.deltaTime, 0, zDiff * moveSpeed * Time.deltaTime);
                }


                //Debug.Log("Unit Position: " + this.transform.position.x + ", " + this.transform.position.z + " Speed: " + moveSpeed);

                yield return null;
            }

            tilesCrossed++;

            if(tilesCrossed >= this.transform.GetComponent<PlayerProp>().moveRange)
            {
                break;
            }
        }

        //Debug.Log("Finished Corountine");
        pointAndClick.playerMoving = false;
        yield return null;
    }

    private IEnumerator Attack_1Ani(GameObject enemyTile)
    {   
        //Spawn bullet
        GameObject bulletClone = Instantiate(bullet, new Vector3(this.transform.position.x, this.transform.position.y + .5f , this.transform.position.z), Quaternion.identity);

        tilePos = enemyTile.GetComponent<TileProp>().unit.transform.position;
        
        xDiff = tilePos.x - bulletClone.transform.position.x;
        zDiff = tilePos.z - bulletClone.transform.position.z;

        distance = Vector3.Distance(bulletClone.transform.position, tilePos + new Vector3(0, .5f, 0));
        float lastdistance = distance;

        while(distance >= moveSpeed * Time.deltaTime)
        {
            distance = Vector3.Distance(bulletClone.transform.position, tilePos + new Vector3(0, .5f, 0));
            //Debug.Log("Bullet Distance: " + distance);

            if(distance <= moveSpeed * Time.deltaTime)
            {
                bulletClone.transform.position = tilePos;
            }
            else if(distance > lastdistance)
            {
                break;
            }
            else
            {
                bulletClone.transform.position += new Vector3(xDiff * moveSpeed * Time.deltaTime, 0, zDiff * moveSpeed * Time.deltaTime);
                //break;
            }

            yield return null;

        }

        bulletClone.gameObject.SetActive(false);
        Destroy(bulletClone);

        pointAndClick.playerAttacking = false;
        yield return null; 
    }

    //Attack 1
    public void Attack_1(GameObject enemyTile)
    {
        if(enemyTile.GetComponent<TileProp>().unit == null)
        {
            Debug.Log("Error: Enemy unit not found");
        }
        else
        {
            //Debug.Log("Attacking Enemy: " + enemyTile.GetComponent<TileProp>().unit.name + " for " + this.transform.GetComponent<PlayerProp>().attack_1dmg);
            this.transform.LookAt(new Vector3(enemyTile.transform.position.x, 0, enemyTile.transform.position.z));
         
            Attack_1AN = Attack_1Ani(enemyTile);
            StartCoroutine(Attack_1AN);

            enemyTile.GetComponent<TileProp>().unit.GetComponent<EnemyProp>().health -= this.transform.GetComponent<PlayerProp>().attack_1dmg;
        }
    }
}
