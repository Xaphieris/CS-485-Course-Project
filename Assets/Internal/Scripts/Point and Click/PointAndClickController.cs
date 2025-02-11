using UnityEngine;

public class PointAndClickController : MonoBehaviour
{

    public enum mouseButtonCode : ushort
    {
        leftMouse = 0,
        rightMouse = 1,
        middleMouse = 2
    }

    public mouseButtonCode mouseClick = mouseButtonCode.leftMouse;
    public Camera cam = null;
    public Transform marker = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(this.cam == null)
        {
            this.cam = Camera.main;
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

            //Check if mouse-ray hits anything
            RaycastHit hitInfo;

            if(Physics.Raycast(mouseRay, out hitInfo, 100.0F))
            {
                Debug.Log("Ray hit: " + hitInfo.collider.name);
                if(this.marker != null)
                {
                    this.marker.position = hitInfo.point;
                }
            }
        }
    }
}
