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

    public static List<GameObject> EffectsList = new List<GameObject>();
    public GameObject effect1 = null;
    public GameObject effect2 = null;
    public GameObject effect3 = null;
    public GameObject ShopUI = null;

    private bool IsShopOpen = false;
    // Update is called once per frame
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        EffectsList.Add(effect1);
        EffectsList.Add(effect2);
        EffectsList.Add(effect3);
        ShopUI.SetActive(false);
    }
    private void Update()
    {
        TurnOnShopUI();
    }
    void LateUpdate()
    {
        CameraMove();
    }
    void CameraMove()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX = cameraTransform.localEulerAngles.y + mouseX * detailX;
        rotationX = (rotationX > 180.0f) ? rotationX - 360.0f : rotationX;

        rotationY = rotationY + mouseY * detailY;
        rotationY = (rotationY > 180.0f) ? rotationY - 360.0f : rotationY;

        cameraTransform.localEulerAngles = new Vector3(Mathf.Clamp(-rotationY, -20, 50),rotationX, 0f);
        cameraTransform.position = targetTransform.position;
    }
    void TurnOnShopUI()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (IsShopOpen == false)
            {
                Time.timeScale = 0;
                ShopUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                detailX = 0f;
                detailY = 0f;
                IsShopOpen = true;
            }
            else
            {
                Time.timeScale = 1;
                ShopUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                detailX = 5f;
                detailY = 5f;
                IsShopOpen = false;
            }
        }


    }
}
