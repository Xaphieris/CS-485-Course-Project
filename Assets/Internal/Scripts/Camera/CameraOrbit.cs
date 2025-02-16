using System;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    //Set Sensitivity
    [SerializeField] private float moveSensitivity = 1F;
    [SerializeField] private float minOrbitRadius = 5F;
    [SerializeField] private float maxOrbitRadius = 25f;

    //Set Initial Variables including max angle and distance
    private float orbitRadius = 10F;
    private float moveX = 0F;
    private float moveY = 0F;
    private float angleMax = 60F;
    private float angleMin = 5F;

    private Vector3 center = new Vector3(5.0f, 0.0f, 4.0f);


    // Update is called once per frame
    void Update()
    {
        //Mouse controls on middle click
        if(Input.GetMouseButton(2))
        {

            this.transform.LookAt(center);

            moveX = Input.GetAxis("Mouse X");
            moveY = Input.GetAxis("Mouse Y");

            //Max angle cap
            if(this.transform.eulerAngles.x - (moveY * moveSensitivity * Time.deltaTime) >= angleMax)
            {
                this.transform.eulerAngles = new Vector3(angleMax, this.transform.eulerAngles.y + (moveX * moveSensitivity * Time.deltaTime), 0);
            }
            else if(this.transform.eulerAngles.x - (moveY * moveSensitivity * Time.deltaTime) <= angleMin)
            {
                this.transform.eulerAngles = new Vector3(angleMin, this.transform.eulerAngles.y + (moveX * moveSensitivity * Time.deltaTime), 0);
            }
            else
            {
                this.transform.eulerAngles += new Vector3(-moveY * moveSensitivity * Time.deltaTime, moveX * moveSensitivity * Time.deltaTime, 0);
            }


        }
        else
        {
            if(Input.GetKey(KeyCode.W))
            {
                moveY -= moveSensitivity * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.S))
            {
                moveY += moveSensitivity * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.A))
            {
                moveX += moveSensitivity * Time.deltaTime;
            }
            if(Input.GetKey(KeyCode.D))
            {
                moveX -= moveSensitivity * Time.deltaTime;
            }

            //Max angle cap
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


        orbitRadius -= Input.mouseScrollDelta.y;
        orbitRadius = Math.Clamp(orbitRadius, minOrbitRadius, maxOrbitRadius);

        this.transform.position = center - transform.forward * orbitRadius;
    }
}
