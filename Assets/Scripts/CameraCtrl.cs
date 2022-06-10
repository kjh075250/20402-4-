using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public float rotationX = 0f;
    public float rotationY = 0f;
    public float detailX = 5f;
    public float detailY = 5f;
    public Transform cameraTransform = null;
    public Transform targetTransform = null;

    // Update is called once per frame
    void LateUpdate()
    {
        CameraMove();   
    }
    void CameraMove()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX = cameraTransform.localEulerAngles.y + mouseX * detailX;
        rotationX = (rotationX > 180.0f) ? rotationX - 360.0f : rotationX;

        rotationY = rotationY + mouseY * detailY;
        rotationY = (rotationY > 180.0f) ? rotationY - 360.0f : rotationY;

        cameraTransform.localEulerAngles = new Vector3(Mathf.Clamp(-rotationY, -5, 25),rotationX, 0f);
        cameraTransform.position = targetTransform.position;
    }
}
