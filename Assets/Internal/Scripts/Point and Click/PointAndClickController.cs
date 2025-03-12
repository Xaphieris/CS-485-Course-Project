using System.Collections.Generic;
using System.Collections;

using UnityEngine;
using UnityEngine.Tilemaps;

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
            Vector3 mouseCurrentPos = Input.mousePosition;
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
            selectionBoxCloneWhite = Instantiate(selectionBoxWhite, selection.transform.position, Quaternion.identity);
            return;
        }
        else
        {
            selectionBoxCloneRed = Instantiate(selectionBoxRed, selection.transform.position, Quaternion.identity);
        }

        //If player is selected, do action
        if(playerUnitSelected)
        {
            if(lastSelectedObject != null)
            {
                //Player Actions
                TileProp lastSelTileProp = lastSelectedObject.GetComponent<TileProp>();
                TileProp currentSelTileProp = currentSelectedObject.GetComponent<TileProp>();

                if(currentSelectedObject.GetComponent<TileProp>().Traverability())
                {
                    List<TileProp> path = PathFinding.FindPath(lastSelTileProp, currentSelTileProp);

                    GameObject unit = getPlayerUnit();
                    unit.GetComponent<PlayerUnitController>().Traverse(lastSelectedObject, currentSelectedObject, path);
                    currentSelectedObject = null;
                }
 
                clearSelectionBoxes();
                playerUnitSelected = false;
 
                lastSelectedObject = null;
                currentSelectedObject = null;
            }
        }

        lastSelectedObject = currentSelectedObject;
    }

    public GameObject getPlayerUnit()
    {
        //Change this later to get the player unit from the hextile prop
        GameObject unit = lastSelectedObject.GetComponent<TileProp>().unit;
        return unit;
    }

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
}
