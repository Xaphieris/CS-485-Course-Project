using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridDraw : MonoBehaviour
{
    public int playerUnitXBound = 2;
    public int enemyUnitXBound = 2;
    public int numberOfPlayerUnits = 1;
    public int numberOfEnemyUnits = 2;
    public bool regenOnStart = false;

    public GameObject enemy;
    public GameObject player;

    //Set Game Object to spawn
    public GameObject block;
    public GameObject[,] tiles;
    public GameObject[] playerUnits;
    public GameObject[] enemyUnits;

    //Set number of tiles wide
    [Range(2,15)] public int width;

    //Set number of tiles high
    [Range(2,15)] public int height;

    //Offsets for hexagons
    private float xOffset = .5f;

    //Height offset ((Tilesize)/2sin(60))*(3/2);
    private float zOffset = .866f;
  
    //Need to add something to get a selection pool of tiles
    void Start()
    {
        if(regenOnStart)
        {
            DestroyTiles();
            DestroyUnits();
            CreateTiles();
            SetAllConnectedTiles();
            PlaceEnemyUnits();
            PlacePlayerUnits();
        }
        else
        {
            if(this.transform.childCount < 1)
            {
                CreateTiles();
                SetAllConnectedTiles();
                PlaceEnemyUnits();
                PlacePlayerUnits();
            }
        }
    }

    void Update()
    {
        
    }

    [ContextMenu("Reset Tiles")]

    void ResetTiles()
    {
        DestroyTilesIm();
        DestroyUnitsIm();
        CreateTiles();
        SetAllConnectedTiles();
        PlaceEnemyUnits();
        PlacePlayerUnits();
    }

    [ContextMenu("Destroy")]
    void DetroyAll()
    {
        DestroyTilesIm();
        DestroyUnitsIm();
    }


    //Destroy old tiles
    private void DestroyTiles()
    {
        if(this.tiles != null)
        {
            for (int z = 0; z < this.tiles.GetLength(0); ++z)
            {
                for (int x = 0; x < this.tiles.GetLength(1); ++x)
                {
                    Destroy(this.tiles[x, z].gameObject);
                }
            }
        }
        else if(this.transform.childCount > 0)
        {
            while(this.transform.childCount != 0)
            {
                Destroy(this.transform.GetChild(0));
            }
        }
    }

    private void DestroyTilesIm()
    {
        if(this.tiles != null)
        {
            for (int z = 0; z < this.tiles.GetLength(0); ++z)
            {
                for (int x = 0; x < this.tiles.GetLength(1); ++x)
                {
                    DestroyImmediate(this.tiles[x, z].gameObject);
                }
            }
        }
        else if(this.transform.childCount > 0)
        {
            while(this.transform.childCount != 0)
            {
                DestroyImmediate(this.transform.GetChild(0));
            }
        }

    }

    private void DestroyUnits()
    {
        if(this.playerUnits != null)
        {
            for (int z = 0; z < this.playerUnits.GetLength(0); ++z)
            {
                Destroy(this.playerUnits[z].gameObject);
            }
        }

        if(this.enemyUnits != null)
        {
            for (int z = 0; z < this.enemyUnits.GetLength(0); ++z)
            {
                Destroy(this.enemyUnits[z].gameObject);
            }
        }
    }

    private void DestroyUnitsIm()
    {
        if(this.playerUnits != null)
        {
            for (int z = 0; z < this.playerUnits.GetLength(0); ++z)
            {
                DestroyImmediate(this.playerUnits[z].gameObject);
            }
        }

        if(this.enemyUnits != null)
        {
            for (int z = 0; z < this.enemyUnits.GetLength(0); ++z)
            {
                DestroyImmediate(this.enemyUnits[z].gameObject);
            }
        }
    }

    //Instantiate all tiles of the grid
    private void CreateTiles()
    {
        tiles = new GameObject[width,height];
        for (int z = 0; z < height; ++z)
        {
            for (int x = 0; x < width; ++x)
            {
                GameObject clone;
                if(z % 2 == 0)//Even
                {
                    clone = Instantiate(block, new Vector3(x,0,0) + new Vector3 (0, 0, z) * zOffset, Quaternion.identity, this.transform);
                }
                else
                {
                    clone = Instantiate(block, new Vector3(x,0,0) + new Vector3 (0, 0, z) * zOffset + new Vector3(1, 0, 0) * xOffset, Quaternion.identity, this.transform);
                }
                //Debug.Log("Created Clone");

                tiles[x,z] = clone;
                //Debug.Log("Added Clone to Tile List");
                
                //**************************************************
                //Set Clone Properties
                //**************************************************
                clone.name = "Tile " + x + ", " + z;
                
                clone.GetComponent<TileProp>().tileNumX = (int)x;
                clone.GetComponent<TileProp>().tileNumZ = (int)z;
                //Debug.Log("Modified Clone Properties");


                //Hard coded player position
                if(x == 0 && z == 0)
                {
                    clone.GetComponent<TileProp>().hasPlayerUnit = true;
                }
            }
        }
    }

    private void PlacePlayerUnits()
    {
        GameObject tile;
        GameObject playerUnit;
        playerUnits = new GameObject[numberOfPlayerUnits];
     
        for(int i = 1; i <= numberOfPlayerUnits; i++)
        {
            tile = GetTraversableTile(0,playerUnitXBound);

            //Set object type, physical position, rotation, parent
            playerUnit = Instantiate(player, tile.transform.position, Quaternion.identity, GameObject.Find("Player Units").transform);

            playerUnits[i-1] = playerUnit;

            //Set enemy unit name
            playerUnit.name = "PlayerUnit: " + i;
            
            //Set properties so that tile knows it has enemy unit
            tile.GetComponent<TileProp>().hasPlayerUnit = true;
            tile.GetComponent<TileProp>().unit = playerUnit;

            //Set Unit Properties

            Debug.Log("Placed player unit at: " + tile.name + " with entry: " + (0) + ", " + playerUnitXBound);
        }        
    }

    private void PlaceEnemyUnits()
    {
        GameObject tile;
        GameObject enemyUnit;
        enemyUnits = new GameObject[numberOfEnemyUnits];

        for(int i = 1; i <= numberOfEnemyUnits; i++)
        {
            tile = GetTraversableTile(width - enemyUnitXBound, width);

            //Set object type, physical position, rotation, parent
            enemyUnit = Instantiate(enemy, tile.transform.position, Quaternion.identity, GameObject.Find("Enemy Units").transform);

            enemyUnits[i-1] = enemyUnit;

            //Set enemy unit name
            enemyUnit.name = "EnemyUnit: " + i;
            
            //Set properties so that tile knows it has enemy unit
            tile.GetComponent<TileProp>().hasEnemyUnit = true;
            tile.GetComponent<TileProp>().unit = enemyUnit;

            //Set Unit Properties

            Debug.Log("Placed enemy unit at: " + tile.name + " with entry: " + (width - enemyUnitXBound) + ", " + width);
        }
    }


    //Need to ensure that there is at least one accessable tile within the range
    private GameObject GetTraversableTile(int xBoundLow, int xBoundHigh)
    {
        GameObject tile;
        bool foundTile = false;
        List<GameObject> viableNeighbors = new List<GameObject>();
        //Check tile to ensure it is traversable

        tile = tiles[UnityEngine.Random.Range(xBoundLow, xBoundHigh), UnityEngine.Random.Range(0, height)];

        //If tile cannot hold unit, get neighbor tile in range
        if(tile.GetComponent<TileProp>().isTraversable == false || tile.GetComponent<TileProp>().hasEnemyUnit || tile.GetComponent<TileProp>().hasPlayerUnit)
        {
            while(!foundTile)
            {
                foreach (var neighbor in tile.GetComponent<TileProp>().Neighbors)
                {
                    Debug.Log("Checking Neighbor: " + neighbor.tileNumX + ", " + neighbor.tileNumZ);
                    //Check if it can hold a unit, and is within range
                    if(neighbor.isTraversable && !neighbor.hasEnemyUnit && !neighbor.hasPlayerUnit && neighbor.tileNumX >= xBoundLow && neighbor.tileNumX <= xBoundHigh)
                    {
                        return neighbor.transform.gameObject;
                    }
                    else if(neighbor.tileNumX >= xBoundLow && neighbor.tileNumX <= xBoundHigh)
                    {
                        viableNeighbors.Add(neighbor.transform.gameObject);
                    }
                }
                
                tile = viableNeighbors[0];
                viableNeighbors.RemoveAt(0);
            }
        }

        Debug.Log("Found Traversable Tile Immediately");

        return tile; 
    }

    //Get neighboring tiles
    private void SetAllConnectedTiles()
    {
        GameObject tile;

        for (int z = 0; z < height; ++z)
        {
            for (int x = 0; x < width; ++x)
            {
                tile = tiles[x, z];
                Debug.Log("Setting Neighbors for: " + tile.name);

                /*
                    Coordinates For Even Rows
                     [x-1,z+1][x,z+1]
                    [x-1,z][x,z][x+1,z]
                     [x-1,z-1][x,z-1]

                    Coordinates For Odd Rows
                     [x,z+1][x+1,z+1]
                    [x-1,z][x,z][x+1,z]
                     [x,z-1][x+1,z-1]

                    If statements move ccw from top left

                    Checks to determine if a set of coordinates is within the grid, then add that neighbor
                */

                if(z % 2 == 0)//Even
                {
                    if(x-1 < 0 || z+1 == height)
                    {
                        //tile.GetComponent<TileProp>().leftTile1 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().leftTile1 = tiles[x-1,z+1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x-1,z+1].GetComponent<TileProp>());
                    }

                    if(z+1 == height)
                    {
                        //tile.GetComponent<TileProp>().rightTile1 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().rightTile1 = tiles[x, z+1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x,z+1].GetComponent<TileProp>());
                    }

                    if(x+1 == width)
                    {
                        //tile.GetComponent<TileProp>().rightTile2 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().rightTile2 = tiles[x+1, z];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x+1,z].GetComponent<TileProp>());
                    }

                    if(z-1 < 0)
                    {
                        //tile.GetComponent<TileProp>().rightTile3 = null; 
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().rightTile3 = tiles[x, z-1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x,z-1].GetComponent<TileProp>());
                    }

                    if(x-1 < 0 || z-1 < 0)
                    {
                        //tile.GetComponent<TileProp>().leftTile3 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().leftTile3 = tiles[x-1, z-1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x-1,z-1].GetComponent<TileProp>());
                    }

                    if(x-1 < 0)
                    {
                        //tile.GetComponent<TileProp>().leftTile2 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().leftTile2 = tiles[x-1,z];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x-1,z].GetComponent<TileProp>());
                    } 
                }
                else
                {
                    if(z+1 == height)
                    {
                        //tile.GetComponent<TileProp>().leftTile1 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().leftTile1 = tiles[x,z+1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x,z+1].GetComponent<TileProp>());
                    }

                    if(x+1 == width || z+1 == height)
                    {
                        //tile.GetComponent<TileProp>().rightTile1 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().rightTile1 = tiles[x+1, z+1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x+1,z+1].GetComponent<TileProp>());
                    }

                    if(x+1 == width)
                    {
                        //tile.GetComponent<TileProp>().rightTile2 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().rightTile2 = tiles[x+1, z];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x+1,z].GetComponent<TileProp>());
                    }

                    if(x+1 == width || z-1 < 0)
                    {
                        //tile.GetComponent<TileProp>().rightTile3 = null; 
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().rightTile3 = tiles[x+1, z-1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x+1,z-1].GetComponent<TileProp>());
                    }

                    if(z-1 < 0)
                    {
                        //tile.GetComponent<TileProp>().leftTile3 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().leftTile3 = tiles[x, z-1];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x,z-1].GetComponent<TileProp>());
                    }

                    if(x-1 < 0)
                    {
                        //tile.GetComponent<TileProp>().leftTile2 = null;
                    }
                    else
                    {
                        //tile.GetComponent<TileProp>().leftTile2 = tiles[x-1,z];
                        tile.GetComponent<TileProp>().Neighbors.Add(tiles[x-1,z].GetComponent<TileProp>());
                    }
                }
                Debug.Log("Set relatives for: " + tile.name);

            }
        }
    }
}
