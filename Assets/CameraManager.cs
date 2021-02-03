using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;
    public float rotationSpeed = 8f;

    public Terrain terrain;

    private float terrainSize;
    private Vector3? dragOrigin;
    private float dragSensibility = 10;
    private float T0;
    bool moveVertical = false;

    void Start()
    {
        dragOrigin = null;
        terrainSize = terrain.terrainData.size.x;
    }

    void Update()
    {
        DragCamera();
        RotateCamera();
        ZoomCamera();
    }

    private void RotateCamera()
    {
        if (Input.GetMouseButton(1))
        {
            float xRotation = rotationSpeed * Input.GetAxis("Mouse X");
            float yRotation = rotationSpeed * Input.GetAxis("Mouse Y");
            float xAngle = transform.eulerAngles.x % 360f;
            float newAngle = ((xAngle + yRotation));

            if (Math.Cos(newAngle / 360) <= 0f)
                yRotation = 0;

            var angle = new Vector3(transform.eulerAngles.x + yRotation, transform.eulerAngles.y + xRotation, 0);
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(angle.x, angle.y, 0));
        }
    }


    private void DragCamera()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
        {
            moveVertical = Input.GetMouseButtonDown(2);
            dragOrigin = Input.mousePosition;
            T0 = Time.timeSinceLevelLoad;
        }
        
        if (Input.GetMouseButtonUp(2) || Input.GetMouseButtonUp(0))
        {
            moveVertical = false;
            dragOrigin = null;
        }

        if (dragOrigin == null)
        {
            return;
        }
        
        if (moveVertical)
        {
            DragCameraY();
        }else
            DragCameraXZPlane();
    }

    private void DragCameraY()
    {
        float exp = (float) Math.Exp(-(Time.timeSinceLevelLoad - T0));
        float smoot = (1.0f / (1.0f + exp))-0.5f;

        Vector3 worldPos = Input.mousePosition - dragOrigin ?? Input.mousePosition;
        Vector3 pos = worldPos.normalized;
        Vector3 moveDirection = new Vector3(0, pos.y, 0);
        moveDirection *= dragSensibility * smoot;
        
        transform.position += moveDirection;
    }

    private void DragCameraXZPlane()
    {
        float exp = (float) Math.Exp(-(Time.timeSinceLevelLoad - T0));
        float smoot = (1.0f / (1.0f + exp)) - 0.5f;
        Vector3 worldPos = Input.mousePosition - dragOrigin ?? Input.mousePosition;
        Vector3 pos = worldPos.normalized;
        var forward = mainCamera.transform.forward.normalized;
        var right = mainCamera.transform.right.normalized;
        
        Vector3 moveDirection  = new Vector3(right.x * pos.x, 0, right.z * pos.x);
        moveDirection += new Vector3(forward.x * pos.y, 0, forward.z * pos.y);
        moveDirection *= dragSensibility * smoot;

        Vector3 newCamerapos = transform.position + moveDirection;
        if (Math.Abs(newCamerapos.x) < terrainSize * 2 && Math.Abs(newCamerapos.z) < terrainSize * 2)
            transform.position += moveDirection;
    }



    void ZoomCamera()
    {
        float scrollFactor = Input.GetAxis("Mouse ScrollWheel");

        if (scrollFactor != 0)
        {
            transform.localScale = transform.localScale * (1f - scrollFactor);
        }
    }
}