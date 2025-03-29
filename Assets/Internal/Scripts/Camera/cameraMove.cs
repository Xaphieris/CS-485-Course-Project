
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Vector3 CameraPosition;
    public float CameraSpeed = .1F;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CameraPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            CameraPosition.z += CameraSpeed;
        }
        if(Input.GetKey(KeyCode.S))
        {
            CameraPosition.z -= CameraSpeed;
        }
        if(Input.GetKey(KeyCode.A))
        {
            CameraPosition.x -= CameraSpeed;
        }
        if(Input.GetKey(KeyCode.D))
        {
            CameraPosition.x += CameraSpeed;
        }
        if(Input.GetKey(KeyCode.R))
        {
            CameraPosition.y += CameraSpeed;
        }
        if(Input.GetKey(KeyCode.F))
        {
            CameraPosition.y -= CameraSpeed;
        }

        this.transform.position = CameraPosition;
    }
}
