using UnityEditor;
using UnityEngine;

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

    public mouseButtonCode mouseClick = mouseButtonCode.leftMouse;
    public Camera cam = null;
    public Transform marker = null;
    private GameObject lastSelectedObject;
    private GameObject currentSelectedObject;
    public GameObject selectionBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(this.cam == null)
        {
            this.cam = Camera.main;
        }
        if(this.selectionBox == null)
        {
            selectionBox = (GameObject)Resources.Load("Assets/External/Low Poly Hexagons/Assets/Prefabs/Objects/SelectionRed.prefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown((int) this.mouseClick))
        {
            //Get mouse cursor screen position
            Vector3 mouseCurrentPos = Input.mousePosition;
            Debug.Log("Mouse pressed at: " + mouseCurrentPos.x + ", " + mouseCurrentPos.y);

            //Convert mouse cursor position into 3D mouse-ray
            Ray mouseRay = cam.ScreenPointToRay(mouseCurrentPos);
            Debug.DrawRay(mouseRay.origin, mouseRay.direction * 10, Color.yellow);

            
            RaycastHit hitInfo;
            
            //Check if mouse-ray hits anything
            if(Physics.Raycast(mouseRay, out hitInfo, 100.0F))
            {
                Debug.Log("Ray hit: " + hitInfo.collider.name);
                
                //Establish current selected tile
                currentSelectedObject = hitInfo.collider.gameObject;

                //Instantiate(selectionBox, currentSelectedObject.transform.position, Quaternion.identity);

                //Check is the last selection is null
                if(lastSelectedObject != null)
                {
                    Debug.Log("Last Selected Object:" + lastSelectedObject.name);
                    Debug.Log("Current Selected Object:" + currentSelectedObject.name);
                    Debug.Log("Last Selected Has Player Unit: " + lastSelectedObject.GetComponent<TileProp>().hasPlayerUnit);

                    //Move player unit if player tile was selected last, and new tile does not have player
                    if(lastSelectedObject.GetComponent<TileProp>().hasPlayerUnit == true && CanTraverse(currentSelectedObject))
                    {
                        Traverse(lastSelectedObject, currentSelectedObject);
                        currentSelectedObject = null;
                    }
                }


                if(this.marker != null)
                {
                    this.marker.position = hitInfo.point;
                }

                lastSelectedObject = currentSelectedObject;
                currentSelectedObject = null;
            }
        }
    }

    //Check for ability for player unit to move
    private bool CanTraverse(GameObject selection)
    {
        if(selection.GetComponent<TileProp>().hasPlayerUnit)
        {
            Debug.Log("Cannot Traverse");
            return false;
        }
        else
        {
            Debug.Log("Can Traverse");
            return true;
        }
    }

    //Move player unit to selected tile
    private void Traverse(GameObject lastSelection, GameObject currentSelection)
    {
        GameObject unit = GameObject.Find("Player Unit 1");
        
        unit.transform.position = currentSelection.transform.position;
        lastSelection.GetComponent<TileProp>().hasPlayerUnit = false;
        currentSelection.GetComponent<TileProp>().hasPlayerUnit = true;

        Debug.Log("Traversed");
    }
}
