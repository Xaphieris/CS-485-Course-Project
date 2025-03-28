using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using System;

public class EnemyUnitController : MonoBehaviour
{

    public GameObject enemyContainer;
    List<GameObject> enemies = new List<GameObject>();

    private IEnumerator moveU;

    //Proportional speed, max of 1
    public float moveSpeed = 1.5f;

    public GameObject playerUnit;

    public GameObject enemyToAttack;
    public bool scheduleAttack;

    private bool enemyIsMoving;
    private bool enemyBusy;
    private int enemiesMoved;

    public bool turnActive;

    public GameObject pointAndClickController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        Debug.Log("Enemy Units Active");
        
        //GetActiveChildren();
        //MoveEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemies();
        //Shedule attack, wait until enemy is done moving then attack
        //ScheduleAttack_1();
        EndTurn();
    }

    //Stary enemy turn
    public void StartTurn()
    {
        turnActive = true;
        enemiesMoved = 0;
        GetActiveChildren();
    }

    //End enemy turn
    private void EndTurn()
    {

        //Revise this for attacks
        if(enemiesMoved == enemies.Count && !enemyIsMoving)
        {
            turnActive = false;
        }
        if(!turnActive)
        {
            //Reset player armor and end turn
            playerUnit.GetComponent<PlayerProp>().armor = playerUnit.GetComponent<PlayerProp>().baseArmor;
            GameObject.Find("TurnController").GetComponent<TurnController>().EndEnemyTurn();
        }
    }
    
    //Get all active enemies
    private void GetActiveChildren()
    {
        enemies.RemoveAll(item=>true);

        //RemoveEnemies();
        int children;
        
        children = enemyContainer.transform.childCount;
        Debug.Log("Number of children: " + children);

        for(int i = 0; i < children; i++)
        {
            if(enemyContainer.transform.GetChild(i).gameObject.activeSelf)
            {
                enemies.Add(enemyContainer.transform.GetChild(i).gameObject);
                Debug.Log(enemyContainer.transform.GetChild(i).gameObject.name);
            }
        }
        Debug.Log("Number of enemies: " + enemies.Count);
    }


    //Function to move the enemy units, sets new occupying tiles
    private void MoveEnemies()
    {
        //Might need to do flags to determine if a coroutine is running to do one enemy at a time
        GameObject enemy;
        playerUnit = GameObject.Find("Player Units").transform.GetChild(1).gameObject;

        if(enemiesMoved < enemies.Count && !enemyIsMoving && !enemyBusy)
        {
            enemy = enemies[enemiesMoved];

            //Get path from enemy tile and player tile
            //List<TileProp> path = PathFinding.FindPath(enemy.GetComponent<EnemyProp>().tile.GetComponent<TileProp>(), 
            //        playerUnit.GetComponent<PlayerProp>().tile.GetComponent<TileProp>());

            List<TileProp> path = GetAttackPosition(enemy);


            Debug.Log("Path size: " + path.Count);

            

            enemyIsMoving = true;
            Traverse(enemy, path);

            enemiesMoved++;
            
            Debug.Log("Moving enemy: " + enemy.name);
        }
    }

        //Funtion to call the Coroutine to move a unit
        //Returns true if unit reached end of path
    public bool Traverse(GameObject enemy, List<TileProp> path)
    {
        // Make sure there is actually a path to traverse
        if(path.Count != 0)
        {

            //unit.transform.position = currentSelection.transform.position;
            Debug.Log("Traversing with unit: " + enemy.name);

            path.Reverse();
            moveU = moveUnit(enemy, path);
            StartCoroutine(moveU);

            enemy.transform.GetComponent<EnemyProp>().tile.GetComponent<TileProp>().hasEnemyUnit = false;

            if(path.Count > enemy.transform.GetComponent<EnemyProp>().moveRange)
            {
                path[enemy.transform.GetComponent<EnemyProp>().moveRange - 1].hasEnemyUnit = true;
                path[enemy.transform.GetComponent<EnemyProp>().moveRange - 1].unit = enemy;
                enemy.GetComponent<EnemyProp>().tile = path[enemy.transform.GetComponent<EnemyProp>().moveRange - 1].transform.gameObject;
                
                //Dont attack on false
                return false;
            }
            // If it gets to end of path
            else
            {
                path[path.Count - 1].hasEnemyUnit = true;
                path[path.Count - 1].unit = enemy;
                enemy.transform.GetComponent<EnemyProp>().tile = path[path.Count - 1].transform.gameObject;

                //Attack_1(enemy);
                //enemyToAttack = enemy;
                scheduleAttack = true;
                enemyBusy = true;
                return true;
            }
        }
        else
        {
            if(scheduleAttack)
            {
                Attack_1(enemy);
            }
            enemyIsMoving = false;
        }

        //Dont attack on false
        return false;
    }

    //Move player unit according to MoveSpeed. Look at next tile, move 1 movespeed towards target, repeat each frame
    //When tile is reached, move unit the rest of the way to the target to avoid overshoot. Get next tile, repeat process
    private IEnumerator moveUnit(GameObject enemy, List<TileProp> path)
    {
        Vector3 tilePos;

        float xDiff, zDiff, distance;

        int tilesCrossed = 0;
        Debug.Log("Started Coroutine");
        //Debug.Log("Started Path finding (Start, Target): " + path[0].transform.name + ", " + path[path.Count-1].transform.name);

        foreach (var t in path)
        {
            
            Debug.Log("Tile Traversed: " + t.tileNumX + ", " + t.tileNumZ);

            tilePos = t.transform.position;
            enemy.transform.LookAt(t.transform);
            
            xDiff = tilePos.x - enemy.transform.position.x;
            zDiff = tilePos.z - enemy.transform.position.z;

            distance = Vector3.Distance(enemy.transform.position, tilePos);

            while(distance >= moveSpeed * Time.deltaTime)
            {
                distance = Vector3.Distance(enemy.transform.position, tilePos);
                if(distance <= moveSpeed * Time.deltaTime)
                {
                    enemy.transform.position = tilePos;
                }
                else
                {
                    enemy.transform.position += new Vector3(xDiff * moveSpeed * Time.deltaTime, 0, zDiff * moveSpeed * Time.deltaTime);
                }


                //Debug.Log("Unit Position: " + this.transform.position.x + ", " + this.transform.position.z + " Speed: " + moveSpeed);

                yield return null;
            }

            tilesCrossed++;

            if(tilesCrossed >= enemy.transform.GetComponent<EnemyProp>().moveRange)
            {
                break;
            }
        }

        if(scheduleAttack)
        {
            AttackAnimation(enemy);
            Attack_1(enemy);
        }

        //enemiesMoved++;
        Debug.Log("Finished Corountine");
        enemyIsMoving = false;
        yield return null;
    }

    //Attacking
    //  Looking at player position
    //      Get cost of each tile within 1 tile to player unit
    //          Store tiles and direction from player unit
    //          Track minimum path cost and the tile of the MPC
    //          For any tiles still in the list
    //              Check next tile in direction, see path cost and obstructions
    //              Do amount of times within attack range, keep shortest path

    public List<TileProp> GetAttackPosition(GameObject enemy)
    {
        PointAndClickController pointAndClickController_C = pointAndClickController.GetComponent<PointAndClickController>();
        int attackRange = enemy.GetComponent<EnemyProp>().attack_1range;
        TileProp playerTile = playerUnit.GetComponent<PlayerProp>().tile.GetComponent<TileProp>();
        TileProp enemyTile = enemy.GetComponent<EnemyProp>().tile.GetComponent<TileProp>();

        GridDraw grid = GameObject.Find("Grid").GetComponent<GridDraw>();

        TileProp[,] tilesInPattern = new TileProp [6,attackRange];
        int tileX;
        int tileZ;

        TileProp tile;
        TileProp bestTile;
        List<TileProp> bestPath = new List<TileProp>();
        List<TileProp> currentPath = new List<TileProp>();

        Vector2Int tileV;


        //Looks at player position
        //Gets each tile within attack pattern around player unit
        //  Effectively ray traces around player unit outward
        //Checks the path cost of each tile in pattern
        //Return the shortest path
        for(int i = 0; i < 6; i++)
        {
            int currentX = playerTile.tileNumX;
            int currentZ = playerTile.tileNumZ;

            for(int j = 0; j < attackRange; j++)
            {
                tileV = pointAndClickController_C.GetNeighborTile(currentX, currentZ,i);
                tileX = tileV.x;
                tileZ = tileV.y;

                if(!(tileX >= grid.width || tileZ >= grid.height || tileX < 0 || tileZ < 0))
                {
                    tile = grid.tiles[tileX,tileZ].transform.GetComponent<TileProp>();

                    if(tile.tileNumX == enemyTile.tileNumX && tile.tileNumZ == enemyTile.tileNumZ)
                    {
                        //Attack_1(enemy);
                        //enemyToAttack = enemy;

                        scheduleAttack = true;
                        enemyBusy = true;
                        return new List<TileProp>();
                    }

                    if(tile.Traversability() && !tile.isObstructing)
                    {
                        tilesInPattern[i,j] = tile;

                        //Find path
                        currentPath = PathFinding.FindPath(enemyTile, tile);

                        if(currentPath == null)
                        {
                            break;
                        }

                        //Keep shortest path
                        if(currentPath.Count < bestPath.Count || bestPath.Count == 0)
                        {
                            Debug.Log("Best Path ATP, currentPath: " + bestPath.Count + ", " + currentPath.Count);

                            bestPath = new List<TileProp>(currentPath);
                            bestTile = tile;
                        }

                        currentX = tile.tileNumX;
                        currentZ = tile.tileNumZ;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        Debug.Log("Best path: " + bestPath.Count);

        return bestPath;
    }


    //Attack function against player
    public void Attack_1(GameObject enemy)
    {
        Debug.Log(enemy.name + " attacking player for: " + enemy.GetComponent<EnemyProp>().attack_1dmg);

        int enemyDmg = enemy.GetComponent<EnemyProp>().attack_1dmg;
        int playerArmor = playerUnit.GetComponent<PlayerProp>().armor;
        
        playerUnit.GetComponent<PlayerProp>().armor = (int)MathF.Max(0, playerUnit.GetComponent<PlayerProp>().armor-enemyDmg);
        

        playerUnit.GetComponent<PlayerProp>().health -= (int)Mathf.Max(0, enemyDmg-playerArmor);
    }

    public void AttackAnimation(GameObject enemy)
    {
        enemy.transform.LookAt(new Vector3(playerUnit.transform.position.x, 0, playerUnit.transform.position.z));

        

        enemyBusy = false;
    }

    //Wait to attack until after movement
    public void ScheduleAttack_1()
    {
        if(!enemyIsMoving && scheduleAttack && enemyToAttack != null)
        {
            Attack_1(enemyToAttack);
            scheduleAttack = false;
            enemyToAttack = null;
        }
    }
}
