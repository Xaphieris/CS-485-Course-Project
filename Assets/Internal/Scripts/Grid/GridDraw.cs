using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridDraw : MonoBehaviour
{
    public int playerUnitXBound = 2;
    public int enemyUnitXBound = 8;
    public int numberOfPlayerUnits = 1;
    public int numberOfEnemyUnits = 2;

    public GameObject enemy;
    public GameObject player;

    //Set Game Object to spawn
    public GameObject block;
    public GameObject[,] tiles;

    //Set number of tiles wide
    public int width;

    //Set number of tiles high
    public int height;

    //Offsets for hexagons
    private float xOffset = .5f;

    //Height offset ((Tilesize)/2sin(60))*(3/2);
    private float zOffset = .866f;
  
    //Need to add something to get a selection pool of tiles
    void Start()
    {
        CreateTiles();
        SetAllConnectedTiles();
        PlaceEnemyUnits();
        PlacePlayerUnits();
    }

    void Update()
    {
        
    }

    //Instantiate all tiles of the grid
    private void CreateTiles()
    {
        tiles = new GameObject[width,height];
        for (float z = 0; z < height; ++z)
        {
            for (float x = 0; x < width; ++x)
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

                tiles[(int)x,(int)z] = clone;
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
        for(int i = 1; i <= numberOfPlayerUnits; i++)
        {
            tile = GetTraversableTile(0,playerUnitXBound);

            //Set object type, physical position, rotation, parent
            playerUnit = Instantiate(player, tile.transform.position, Quaternion.identity, GameObject.Find("Player Units").transform);

            //Set enemy unit name
            playerUnit.name = "PlayerUnit: " + i;
            
            //Set properties so that tile knows it has enemy unit
            tile.GetComponent<TileProp>().hasPlayerUnit = true;
            tile.GetComponent<TileProp>().unit = playerUnit;

            //Set Unit Properties
        }        
    }

    private void PlaceEnemyUnits()
    {
        GameObject tile;
        GameObject enemyUnit;
        for(int i = 1; i <= numberOfEnemyUnits; i++)
        {
            tile = GetTraversableTile(enemyUnitXBound, width);

            //Set object type, physical position, rotation, parent
            enemyUnit = Instantiate(enemy, tile.transform.position, Quaternion.identity, GameObject.Find("Enemy Units").transform);

            //Set enemy unit name
            enemyUnit.name = "EnemyUnit: " + i;
            
            //Set properties so that tile knows it has enemy unit
            tile.GetComponent<TileProp>().hasEnemyUnit = true;
            tile.GetComponent<TileProp>().unit = enemyUnit;

            //Set Unit Properties
        }
    }

    private GameObject GetTraversableTile(int xBoundLow, int xBoundHigh)
    {
        GameObject tile;
        //Check tile to ensure it is traversable

        tile = tiles[UnityEngine.Random.Range(xBoundLow, xBoundHigh), UnityEngine.Random.Range(0, height)];

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
