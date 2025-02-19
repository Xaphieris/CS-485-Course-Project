using System.Collections.Generic;
using UnityEngine;

public class TileProp : MonoBehaviour
{

    public bool canTraverse = true;

    public bool hasPlayerUnit = false;

    public bool isTraversable = true;

    public int tileNumX;
    public int tileNumZ;

    public List<TileProp> Neighbors;
    public TileProp Connection { get; private set; }
    public int G { get; private set; }
    public int H { get; private set; }
    public int F { get; private set; }

    public GameObject unit;

    //public GameObject leftTile1, leftTile2, leftTile3, rightTile1, rightTile2, rightTile3;
    //public List<TileProp> neighbors;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
