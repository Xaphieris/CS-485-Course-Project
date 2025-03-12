using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileProp : MonoBehaviour
{
    //Tile traversal/selection properties
    public bool hasPlayerUnit = false;
    public bool hasEnemyUnit = false;
    public bool isTraversable = true;

    //Tile Array Coordinates
    public int tileNumX;
    public int tileNumZ;


    //Pathfinding
    public List<TileProp> Neighbors;
    public TileProp Connection { get; private set; }
    public float G { get; private set; }
    public float H { get; private set; }
    public float F { get; private set; }

    //Container for game object residing on tile
    public GameObject unit;

    public void Update()
    {
        if(!Traverability())
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    //Set connection to another tile
    public void SetConnection(TileProp tileprop) 
    {
        Connection = tileprop;
    }

    //Set distance from start tile to this tile
    public void SetG(float g)
    {
        G = g;
        SetF();
    }

    //Set distance to end tile
    public void SetH(float h)
    {
        H = h;
        SetF();
    }

    //Set the F value (combination of distance from start to this tile, and distance from this tile to end tile)
    public void SetF()
    {
        F = G + H;
    }

    //Get distance between this tile and a target tile
    public float GetDistance(TileProp endTile)
    {
        //return Mathf.Abs(endTile.tileNumX - this.tileNumX) + Mathf.Abs(endTile.tileNumZ - this.tileNumZ);
        return Mathf.Round(Vector3.Distance(endTile.transform.position, this.transform.position));
    }

    //Check is the tile is taversable
    public bool Traverability()
    {
        if(!isTraversable || hasEnemyUnit || hasPlayerUnit)
        {
            return false;
        }

        return true;
    }
}
