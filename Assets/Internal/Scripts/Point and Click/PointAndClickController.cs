using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using System.Numerics;
using TMPro;

public class PointAndClickController : MonoBehaviour
{

    //Set up mouse click buttons
    public enum mouseButtonCode : ushort
    {
        leftMouse = 0,
        rightMouse = 1,
        middleMouse = 2
    }

    public bool playerUnitSelected = false;

    LayerMask layerMask;

    public mouseButtonCode mouseClick = mouseButtonCode.leftMouse;
    public Camera cam = null;
    public Transform marker = null;
    private GameObject lastSelectedObject;
    private GameObject currentSelectedObject;
    public GameObject selectionBoxRed;
    public GameObject selectionBoxWhite;
    private GameObject selectionBoxCloneRed;
    private GameObject selectionBoxCloneWhite;

    public int playerAttacks = 1;
    public int playerMoves = 1;
    public bool playerMoving;
    public bool playerAttacking;
    public TextMeshProUGUI playerAttackText;
    public TextMeshProUGUI playerMoveText;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        layerMask = LayerMask.GetMask("Tile");
        
        if(this.cam == null)
        {
            this.cam = Camera.main;
        }
        if(this.selectionBoxRed == null)
        {
            //I don't think this works
            selectionBoxRed = (GameObject)Resources.Load("Assets/External/Low Poly Hexagons/Assets/Prefabs/Objects/SelectionRed.prefab");
        }
        if(this.selectionBoxWhite == null)
        {
            //I don't think this works
            selectionBoxWhite = (GameObject)Resources.Load("Assets/External/Low Poly Hexagons/Assets/Prefabs/Objects/SelectionWhite.prefab");
        }
    }

    //Need to DO:
    //  Create a Get Selection Method
    //      Determine Actions from selection
    //          Traversal Script, in UNIT, pass path from pathfinding?

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown((int) this.mouseClick))
        {
            //Get mouse cursor screen position
            UnityEngine.Vector3 mouseCurrentPos = Input.mousePosition;
            //Debug.Log("Mouse pressed at: " + mouseCurrentPos.x + ", " + mouseCurrentPos.y);

            //Convert mouse cursor position into 3D mouse-ray
            Ray mouseRay = cam.ScreenPointToRay(mouseCurrentPos);
            //Debug.DrawRay(mouseRay.origin, mouseRay.direction * 10, Color.yellow);

            RaycastHit hitInfo;
            
            //Check if mouse-ray hits anything
            if(Physics.Raycast(mouseRay, out hitInfo, 100.0F, layerMask))
            {
                Debug.Log("Ray hit: " + hitInfo.collider.name);
                
                //Establish current selected tile
                Selected(hitInfo.collider.gameObject);


                //currentSelectedObject = hitInfo.collider.gameObject;

                //Check is the last selection is null
                // if(lastSelectedObject != null)
                // {
                //     // Debug.Log("Last Selected Object:" + lastSelectedObject.name);
                //     // Debug.Log("Current Selected Object:" + currentSelectedObject.name);
                //     // Debug.Log("Last Selected Has Player Unit: " + lastSelectedObject.GetComponent<TileProp>().hasPlayerUnit);

                //     //Move player unit if player tile was selected last, and new tile does not have player
                //     if(lastSelectedObject.GetComponent<TileProp>().hasPlayerUnit)
                //     {
                //         TileProp lastSelTileProp = lastSelectedObject.GetComponent<TileProp>();
                //         TileProp currentSelTileProp = currentSelectedObject.GetComponent<TileProp>();

                //         List<TileProp> path = PathFinding.FindPath(lastSelTileProp, currentSelTileProp);

                //         GameObject unit = getPlayerUnit();

                //         unit.GetComponent<PlayerUnitController>().Traverse(lastSelectedObject, currentSelectedObject, path);
                //         currentSelectedObject = null;
                //     }
                // }


                if(this.marker != null)
                {
                    this.marker.position = hitInfo.point;
                }

                lastSelectedObject = currentSelectedObject;
                currentSelectedObject = null;
            }

            //Did not hit
            else
            {
                clearSelectionBoxes();
            }
        }

        playerAttackText.text = "Attacks: " + playerAttacks + "/" + 1;
        playerMoveText.text = "Movement: " + playerMoves + "/" + 1;
    }


    //Selection method
    //  Determine what was selected
    // 

    public void Selected(GameObject selection)
    {
        currentSelectedObject = selection;

        //Destroy Old selection marker
        if(selectionBoxCloneRed != null)
        {
            Destroy(selectionBoxCloneRed);
        }
        
        //Check selection
        if(selection.GetComponent<TileProp>().hasPlayerUnit)
        {
            //Select player unit
            playerUnitSelected = true;
            selectionBoxCloneWhite = Instantiate(selectionBoxWhite, selection.transform.position, UnityEngine.Quaternion.identity);
            return;
        }
        else
        {
            selectionBoxCloneRed = Instantiate(selectionBoxRed, selection.transform.position, UnityEngine.Quaternion.identity);
        }

        //If player is selected, do action
        if(playerUnitSelected)
        {
            if(lastSelectedObject != null)
            {
                //Player Actions
                TileProp lastSelTileProp = lastSelectedObject.GetComponent<TileProp>();
                TileProp currentSelTileProp = currentSelectedObject.GetComponent<TileProp>();

                if(currentSelectedObject.GetComponent<TileProp>().Traversability() && playerMoves > 0 && !playerAttacking)
                {
                    playerMoving = true;
                    List<TileProp> path = PathFinding.FindPath(lastSelTileProp, currentSelTileProp);

                    GameObject unit = getPlayerUnit();
                    unit.GetComponent<PlayerUnitController>().Traverse(lastSelectedObject, currentSelectedObject, path);
                    currentSelectedObject = null;
                    playerMoves = 0;
                }
                else if(currentSelectedObject.GetComponent<TileProp>().unit != null && playerAttacks > 0 && !playerMoving)
                {
                    GameObject unit = getPlayerUnit();

                    //Need to check if player can hit

                    if(Attack_1_pattern(unit))
                    {
                        playerAttacking = true;
                        unit.GetComponent<PlayerUnitController>().Attack_1(currentSelectedObject);
                        playerAttacks = 0;
                    }
                    

                }
 
                clearSelectionBoxes();
                playerUnitSelected = false;
 
                lastSelectedObject = null;
                currentSelectedObject = null;
            }
        }

        lastSelectedObject = currentSelectedObject;
    }

    //Get the player unit object from the last selected tile
    public GameObject getPlayerUnit()
    {
        //Change this later to get the player unit from the hextile prop
        GameObject unit = lastSelectedObject.GetComponent<TileProp>().unit;
        return unit;
    }

    //Clear all selection boxes
    public void clearSelectionBoxes()
    {
        if(selectionBoxCloneWhite != null)
        {
            Destroy(selectionBoxCloneWhite);
        } 
        if(selectionBoxCloneRed != null)
        {
            Destroy(selectionBoxCloneRed);
        }
    }

    public bool Attack_1_pattern(GameObject unit)
    {
        int attackRange = unit.GetComponent<PlayerProp>().attack_1range;
        int targetX = currentSelectedObject.GetComponent<TileProp>().tileNumX;
        int targetZ = currentSelectedObject.GetComponent<TileProp>().tileNumZ;

        int tileX = lastSelectedObject.GetComponent<TileProp>().tileNumX;
        int tileZ = lastSelectedObject.GetComponent<TileProp>().tileNumZ;

        GridDraw grid = GameObject.Find("Grid").GetComponent<GridDraw>();

        if(grid == null)
        {
            Debug.Log("Failed to find grid");
        }
        Debug.Log("Grid size" + grid.tiles.Length);

        //Check distance
        if(lastSelectedObject.GetComponent<TileProp>().GetDistance(currentSelectedObject.GetComponent<TileProp>()) > attackRange)
        {
            //Debug.Log("Attack outside of range");
            return false;
        }

        int xdif = targetX - tileX;
        int zdif = targetZ - tileZ;

        //Debug.Log("Attack dif: " + xdif + ", " + zdif);

        // ** ** Directions
        // Find direction
        // Trace along direction until enemy found or out of range
        // If obstruction, no attack

        int direction = GetDirection(xdif, zdif, tileX, tileZ);

        //Debug.Log("Found Direction: " + direction);

        UnityEngine.Vector2 currentTile = new UnityEngine.Vector2(tileX, tileZ);
        TileProp tile;
        bool foundEnemy = false;

        for(int i = 0; i < attackRange; i++)
        {
            currentTile = GetNeighborTile((int)currentTile.x, (int)currentTile.y, direction);

            //Ensure check is within bounds
            if(currentTile.x >= grid.width || currentTile.y >= grid.height || currentTile.x < 0 || currentTile.y < 0)
            {
                //Debug.Log("Failed to find enemy within bounds");
                return false;
            }

            //Get the neighbor tile and check it
            tile = grid.tiles[(int)currentTile.x, (int)currentTile.y].GetComponent<TileProp>();
            if(tile.isObstructing)
            {
                //Debug.Log("Attack obstructed");
                return false;
            }
            if(tile.hasEnemyUnit && tile.unit != null)
            {
                if(tile.transform != currentSelectedObject.transform)
                {
                    //Debug.Log("Attack blocked by another enemy");
                    //Debug.Log("Tile Obstructing, Selection: " + tile.transform.name + ", " + currentSelectedObject.name);
                    return false;
                }

                foundEnemy = true;
                break;
            }
        }

        if(!foundEnemy)
        {
            //Debug.Log("Attack outside of pattern or range");
        }

        return foundEnemy;
        

        // Check if tile is within pattern

        // Debug.Log("Pattern Check Results: " + ((zdif+1) / 2 + (zdif + 1) % 2) + ", " + (Mathf.Abs(zdif - 1) / 2) + ", " + (zdif));

        // if((zdif+1) / 2 + (zdif + 1) % 2 == xdif)
        // {
        //     // Check all tiles between player and target
         
        //     if(zdif > 0)
        //     {
        //         for(int i = 1; i <= zdif; i++)
        //         {
        //             Debug.Log("Checking tile: " + ((i +1) / 2 + (i + 1) % 2 + tileX) + ", " + (i + tileZ));
        //             if(grid.tiles[(i +1) / 2 + (i + 1) % 2 + tileX, i + tileZ].GetComponent<TileProp>().isObstructing)
        //             {
        //                 Debug.Log("Attack obstructed");
        //                 return false;
        //             }
        //         }
        //     }
        //     else if(zdif < 0)
        //     {
        //         for(int i = -1; i >= zdif; i--)
        //         {
        //             Debug.Log("Checking tile: " + ((i +1) / 2 + (i + 1) % 2 + tileX) + ", " + (i + tileZ));
        //             if(grid.tiles[((i +1) / 2 + (i + 1) % 2 + tileX), (i + tileZ)].GetComponent<TileProp>().isObstructing)
        //             {
        //                 Debug.Log("Attack obstructed");
        //                 return false;
        //             }
        //         }
        //     }

        //     Debug.Log("Attacking with type 1");
        //     return true;
        // }
        // else if(Mathf.Abs(zdif - 1) / 2 == xdif)
        // {
        //     if(zdif > 0)
        //     {
        //         for(int i = 1; i <= zdif; i++)
        //         {
        //             Debug.Log("Checking tile: " + (Mathf.Abs(i - 1) / 2 + tileX) + ", " + (i + tileZ));
        //             if(grid.tiles[Mathf.Abs(i - 1) / 2 + tileX, i + tileZ].GetComponent<TileProp>().isObstructing)
        //             {
        //                 Debug.Log("Attack obstructed");
        //                 return false;
        //             }
        //         }
        //     }
        //     else if(zdif < 0)
        //     {
        //         for(int i = -1; i >= zdif; i--)
        //         {
        //             Debug.Log("Checking tile: " + (Mathf.Abs(i - 1) / 2 + tileX) + ", " + (i + tileZ));
        //             if(grid.tiles[Mathf.Abs(i - 1) / 2 + tileX, i + tileZ].GetComponent<TileProp>().isObstructing)
        //             {
        //                 Debug.Log("Attack obstructed");
        //                 return false;
        //             }
        //         }
        //     }

        //     Debug.Log("Attacking with type 2");
        //     return true;
        // }
        // else if(zdif == 0)
        // {
        //     if(xdif > 0)
        //     {
        //         for(int i = 1; i <= xdif; i++)
        //         {
        //             Debug.Log("Checking tile: " + (i + tileX) + ", " + (tileZ));
        //             if(grid.tiles[i + tileX, tileZ].GetComponent<TileProp>().isObstructing)
        //             {
        //                 Debug.Log("Attack obstructed");
        //                 return false;
        //             }
        //         }
        //     }
        //     else
        //     {
        //         for(int i = -1; i >= xdif; i--)
        //         {
        //             Debug.Log("Checking tile: " + (i + tileX) + ", " + (tileZ));
        //             if(grid.tiles[i + tileX, tileZ].GetComponent<TileProp>().isObstructing)
        //             {
        //                 Debug.Log("Attack obstructed");
        //                 return false;
        //             }
        //         }
        //     }

        //     Debug.Log("Attacking with type 3");
        //     return true;
        // }
    }

    public Vector2Int GetNeighborTile(int x, int z, int direction)
    {
        // Direction logic for hexagonal grid (6 possible directions)
        // Direction 0 = Up Right, 1 = Right, 2 = Down-Right, 3 = Down Left, 4 = Left, 5 = Up-Left
        
        switch (direction)
        {
            case 0: return new Vector2Int(x + (z % 2 == 0 ? 0 : 1), z + 1);  // Up-Right
            case 1: return new Vector2Int(x + 1, z);                  // Right
            case 3: return new Vector2Int(x + (z % 2 == 0 ? -1 : 0), z - 1); // Down-Left
            case 2: return new Vector2Int(x + (z % 2 == 0 ? 0 : 1), z - 1);     // Down-Right
            case 4: return new Vector2Int(x - 1, z);                  // Left
            case 5: return new Vector2Int(x + (z % 2 == 0 ? -1 : 0), z + 1);    // Up-Left
            default: return new Vector2Int(x, z);
        }
    }

    public int GetDirection(int xdif, int zdif, int tileX, int tileZ)
    {
        //Debug.Log("Finding Direction");

        if(zdif > 0)
        {
            if(xdif > 0)
            {
                //Return upper right
                return 0;
                
            }
            else if(xdif < 0)
            {
                //Return upper left
                return 5;
            }
            else
            {
                if(tileZ % 2 == 0)
                {
                    //Return upper right
                    return 0;
                }
                else
                {
                    //Return upper left
                    return 5;
                }
            }

        }
        else if(zdif < 0)
        {
            if(xdif > 0)
            {
                //Return lower right
                return 2;
            }
            else if(xdif < 0)
            {
                //Return lower left
                return 3;
            }
            else
            {
                if(tileZ % 2 == 0)
                {
                    //Return lower right
                    return 2;
                }
                else
                {
                    //Return lower left
                    return 3;
                }
            }
        }
        else
        {
            if(xdif > 0)
            {
                //Return right
                return 1;
            }
            else
            {
                //Return left
                return 4;
            }
        }
    }
}
