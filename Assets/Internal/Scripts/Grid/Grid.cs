
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject[,] tiles;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public int GetDistance(GameObject startTile, GameObject endTile)
    {
        int startX = startTile.GetComponent<TileProp>().tileNumX;
        int startZ = startTile.GetComponent<TileProp>().tileNumZ;

        int endX = endTile.GetComponent<TileProp>().tileNumX;
        int endZ = endTile.GetComponent<TileProp>().tileNumZ;

        return Mathf.Abs(endX - startX) + Mathf.Abs(endZ - startZ);
    }
}
