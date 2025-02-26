using System.Collections.Generic;
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
    public int G { get; private set; }
    public int H { get; private set; }
    public int F { get; private set; }

    //Container for game object residing on tile
    public GameObject unit;

    public void SetConnection(TileProp tileprop) {
        Connection = tileprop;
    }

    public void SetG(int g)
    {
        G = g;
        SetF();
    }

    public void SetH(int h)
    {
        H = h;
        SetF();
    }

    public void SetF()
    {
        F = G + H;
    }

    public int GetDistance(TileProp endTile)
    {
        return Mathf.Abs(endTile.tileNumX - this.tileNumX) + Mathf.Abs(endTile.tileNumZ - this.tileNumZ);
    }
}
