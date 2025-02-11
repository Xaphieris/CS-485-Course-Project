using System;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] private float moveSensitivity = 1F;
    [SerializeField] private float minOrbitRadius = 5F;
    [SerializeField] private float maxOrbitRadius = 25f;

    private float orbitRadius = 10F;
    private float moveX = 0F;
    private float moveY = 0F;
    private float angleMax = 60F;
    private float angleMin = 5F;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(2))
        {

            this.transform.LookAt(Vector3.zero);

            moveX = Input.GetAxis("Mouse X");
            moveY = Input.GetAxis("Mouse Y");

            if(this.transform.eulerAngles.x - (moveY * moveSensitivity) >= angleMax)
            {
                this.transform.eulerAngles = new Vector3(angleMax, this.transform.eulerAngles.y + (moveX * moveSensitivity), 0);
            }
            else if(this.transform.eulerAngles.x - (moveY * moveSensitivity) <= angleMin)
            {
                this.transform.eulerAngles = new Vector3(angleMin, this.transform.eulerAngles.y + (moveX * moveSensitivity), 0);
            }
            else
            {
                this.transform.eulerAngles += new Vector3(-moveY * moveSensitivity, moveX * moveSensitivity, 0);
            }


        }
        else
        {
            if(Input.GetKey(KeyCode.W))
            {
                moveY -= moveSensitivity;
            }
            if(Input.GetKey(KeyCode.S))
            {
                moveY += moveSensitivity;
            }
            if(Input.GetKey(KeyCode.A))
            {
                moveX += moveSensitivity;
            }
            if(Input.GetKey(KeyCode.D))
            {
                moveX -= moveSensitivity;
            }

            if(this.transform.eulerAngles.x - moveY >= angleMax)
            {
                this.transform.eulerAngles = new Vector3(angleMax, this.transform.eulerAngles.y + moveX, 0);
            }
            else if(this.transform.eulerAngles.x - moveY <= angleMin)
            {
                this.transform.eulerAngles = new Vector3(angleMin, this.transform.eulerAngles.y + moveX, 0);
            }
            else
            {
                this.transform.eulerAngles += new Vector3(-moveY, moveX, 0);
            }

            moveX = 0;
            moveY = 0;
        }


        orbitRadius -= Input.mouseScrollDelta.y / moveSensitivity;
        orbitRadius = Math.Clamp(orbitRadius, minOrbitRadius, maxOrbitRadius);

        this.transform.position = Vector3.zero - transform.forward * orbitRadius;
    }
}
