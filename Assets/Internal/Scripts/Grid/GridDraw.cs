using System.Collections.Generic;
using System.Net.WebSockets;
using Mono.Cecil;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    public GameObject mountain;
    public int nonTraversablePercent;
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

    public List<TileProp> traversable;
  
    //Need to add something to get a selection pool of tiles
    void Start()
    {
        if(regenOnStart)
        {
            DestroyTiles();
            DestroyUnits();
            CreateTiles();
            SetAllConnectedTiles();
            CheckTiles();
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
        CheckTiles();
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
        GameObject template;
        tiles = new GameObject[width,height];
        for (int z = 0; z < height; ++z)
        {
            for (int x = 0; x < width; ++x)
            {
                if(UnityEngine.Random.Range(0, 101) > nonTraversablePercent)
                {
                    template = block;
                }
                else
                {
                    template = mountain;
                }

                GameObject clone;
                if(z % 2 == 0)//Even
                {
                    clone = Instantiate(template, new Vector3(x,0,0) + new Vector3 (0, 0, z) * zOffset, Quaternion.identity, this.transform);
                }
                else
                {
                    clone = Instantiate(template, new Vector3(x,0,0) + new Vector3 (0, 0, z) * zOffset + new Vector3(1, 0, 0) * xOffset, Quaternion.identity, this.transform);
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

                if(clone.GetComponent<TileProp>().isTraversable)
                {
                    traversable.Add(clone.GetComponent<TileProp>());
                }


                // //Hard coded player position
                // if(x == 0 && z == 0)
                // {
                //     clone.GetComponent<TileProp>().hasPlayerUnit = true;
                // }

                if(clone.transform.GetChild(1).GetComponent<TextMeshPro>() != null)
                {
                    clone.transform.GetChild(1).GetComponent<TextMeshPro>().text = clone.name;
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
            playerUnit.GetComponent<PlayerProp>().tile = tile;

            //Set Unit Properties

            //Debug.Log("Placed player unit at: " + tile.name + " with entry: " + (0) + ", " + playerUnitXBound);
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
            enemyUnit.GetComponent<EnemyProp>().tile = tile;

            //Set Unit Properties

            //Debug.Log("Placed enemy unit at: " + tile.name + " with entry: " + (width - enemyUnitXBound) + ", " + width);
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
                    //Debug.Log("Checking Neighbor: " + neighbor.tileNumX + ", " + neighbor.tileNumZ);
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

        //Debug.Log("Found Traversable Tile Immediately");

        return tile; 
    }

    private void CheckTiles()
    {
        //Get a list of all traversable tiles (can be made in the draw function)
        //Get a count of all traversable tiles (length of list before manipulation)
        //Start at the first tile of the list and ensure all other tiles in the list are reachable
        //  Check neighbors, if neighbor is traversable, add it to reachable list
        //  Add reachable tiles to a new connected list, add the processed tile to a processed list, remove tile from traversable list
        //  Only check tiles that have not been processed
        //  If the connected list is less than the traversable count, then there are some tiles that are not reachable

        List<TileProp> localTraversable = new List<TileProp>(traversable);


        TileProp currentTile;
        TileProp newCurrentTile;

        List<TileProp> connected = new List<TileProp>();
        List<TileProp> processed = new List<TileProp>();
        List<TileProp> toProcess = new List<TileProp>();
        
        toProcess.Add(localTraversable[0]);
        connected.Add(localTraversable[0]);

        //Debug.Log("LocalTraversable Start: " + localTraversable.Count);
        
        while(toProcess.Count > 0)
        {
            currentTile = toProcess[0];
            toProcess.RemoveAt(0);
            localTraversable.Remove(currentTile);
            //Debug.Log("LocalTraversable: " + localTraversable.Count);

            foreach(var neighbor in currentTile.Neighbors)
            {
                if(neighbor.isTraversable && !connected.Contains(neighbor))
                {
                    connected.Add(neighbor);
                }
                if(neighbor.isTraversable && !processed.Contains(neighbor))
                {
                    toProcess.Add(neighbor);
                }
            }

            processed.Add(currentTile);
        }


        //If some tiles are not reachable, need to make them reachable
        //  Foreach tile that is not reachable get its neighbors and check if any are not traversable
        //  Foreach non traversable neighbor, check if it is neighbors with a connected tile. At the first connected tile found, change this tile to traversable
        //      Add the flipped tile to connected, add current tile to connected. Check all neighbors of current tile. Add any traversable neighbors to connected and remove them from localTraversable
        //  If no neighbors of the traversable tile neighbor a connected tile, flip the first nontraversable tile and add it to the list to be checked

        while(localTraversable.Count > 0)
        {
            //Debug.Log("Finding flippable tiles");

            currentTile = localTraversable[0];
            localTraversable.RemoveAt(0);

            bool flipTileFound = false;

            foreach(var neighbor in currentTile.Neighbors)
            {
                if(!neighbor.isTraversable)
                {
                    foreach(var subNeighbor in neighbor.Neighbors)
                    {
                        if(connected.Contains(subNeighbor))
                        {
                            Debug.Log("Flip tile: " + neighbor.transform.name);
                            flipTileFound = true;

                            // Flip tile
                            TileProp flippedTile = FlipTile(neighbor);

                            connected.Add(flippedTile);
                            localTraversable.Add(flippedTile);
                            toProcess.Add(flippedTile);


                            // //Find all new connected tiles
                            while(toProcess.Count > 0)
                            {
                                newCurrentTile = toProcess[0];
                                toProcess.RemoveAt(0);
                                localTraversable.Remove(newCurrentTile);
                                //Debug.Log("LocalTraversable: " + localTraversable.Count);

                                foreach(var newNeighbor in newCurrentTile.Neighbors)
                                {
                                    if(newNeighbor.isTraversable && !connected.Contains(newNeighbor))
                                    {
                                        connected.Add(newNeighbor);
                                    }
                                    if(newNeighbor.isTraversable && !processed.Contains(newNeighbor))
                                    {
                                        toProcess.Add(newNeighbor);
                                    }
                                }

                                processed.Add(newCurrentTile);
                            }
                            
                            break;
                        }
                    }
                }

                if(flipTileFound)
                {
                    break;
                }
            }

            //If no neighbors are next to a connected tile, flip one neighbor and add it to the list to be checked
            if(!flipTileFound)
            {
                foreach(var neighbor in currentTile.Neighbors)
                {
                    if(!neighbor.isTraversable)
                    {
                        TileProp flippedTile = FlipTile(neighbor);
                        localTraversable.Add(flippedTile);
                        break;
                    }
                }


            }

            flipTileFound = false;
        }
        

        if(connected.Count == traversable.Count)
        {
            Debug.Log("All traversable tiles are connected");
        }
        else
        {
            Debug.Log("Not all tiles are reachable: " + connected.Count + "/" + traversable.Count);
            foreach (var item in localTraversable)
            {
                Debug.Log(item.transform.name);
            }
        }
    }

    private TileProp FlipTile(TileProp tile)
    {
        // Get tile information
        // Get current tile neighbors
        // Remove the neighbor connection to this tile from all current neighbors
        // Destroy current tile
        // Instanciate new tile
        // Update information
        // Update neighbors
        // Set new neighbor connection to this tile from neighbors
        // Add tile to traversable list

        int tileX = tile.tileNumX;
        int tileZ = tile.tileNumZ;

        List<TileProp> tileNeighbors = tile.Neighbors;

        foreach(var neighbor in tileNeighbors)
        {
            neighbor.Neighbors.Remove(tile);
        }

        Destroy(tile.transform.gameObject);

        // Create the new tile and set properties
        GameObject clone;
        if(tileZ % 2 == 0)//Even
        {
            clone = Instantiate(block, new Vector3(tileX,0,0) + new Vector3 (0, 0, tileZ) * zOffset, Quaternion.identity, this.transform);
        }
        else
        {
            clone = Instantiate(block, new Vector3(tileX,0,0) + new Vector3 (0, 0, tileZ) * zOffset + new Vector3(1, 0, 0) * xOffset, Quaternion.identity, this.transform);
        }
        //Debug.Log("Created Clone");

        tiles[tileX,tileZ] = clone;
        //Debug.Log("Added Clone to Tile List");
        
        //**************************************************
        //Set Clone Properties
        //**************************************************
        clone.name = "Tile " + tileX + ", " + tileZ;
        
        clone.GetComponent<TileProp>().tileNumX = tileX;
        clone.GetComponent<TileProp>().tileNumZ = tileZ;
        //Debug.Log("Modified Clone Properties");

        clone.GetComponent<TileProp>().Neighbors = tileNeighbors;

        foreach(var neighbor in tileNeighbors)
        {
            neighbor.Neighbors.Add(clone.GetComponent<TileProp>());
        }

        traversable.Add(clone.GetComponent<TileProp>());

        return clone.GetComponent<TileProp>();
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
                //Debug.Log("Setting Neighbors for: " + tile.name);

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
                //Debug.Log("Set relatives for: " + tile.name);

            }
        }
    }
}
