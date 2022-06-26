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
    public GameObject ExplainUI = null;

    private bool IsShopOpen = false;
    private bool IsExplainOpen = false;
    public bool IsFirstPlay = Tuto._isTuto;
    // Update is called once per frame
    private void Start()
    {
        IsFirstPlay = Tuto._isTuto;
        Cursor.lockState = CursorLockMode.Locked;
        if (IsFirstPlay == false)
        {
            IsExplainOpen = true;
            ExplainUI.SetActive(true);
            Tuto._isTuto = true;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        EffectsList.Add(effect1);
        EffectsList.Add(effect2);
        EffectsList.Add(effect3);
        ShopUI.SetActive(false);
    }
    private void Update()
    {
        CameraMove();
        OnOffExplainUI();


    }

    void CameraMove()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotationX = cameraTransform.localEulerAngles.y + mouseX * detailX;
        rotationX = (rotationX > 180.0f) ? rotationX - 360.0f : rotationX;

        rotationY = rotationY + mouseY * detailY;
        rotationY = (rotationY > 180.0f) ? rotationY - 360.0f : rotationY;

        cameraTransform.localEulerAngles = new Vector3(Mathf.Clamp(-rotationY, -10, 50),rotationX, 0f);
        cameraTransform.position = targetTransform.position;
        if(GameManager.Instance.playerHp <= 0 || GameManager.Instance.EscapeCanvas.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.None;
            detailX = 0f;
            detailY = 0f;
        }
        else
        {
            TurnOnShopUI();
        }
    }
    void OnOffExplainUI()
    {
        if (IsExplainOpen == true)
        {
            Time.timeScale = 0;
            GameManager.Instance.a = true;
            GameManager.Instance.IsUIActivate = true;
        }
        else
        {
            GameManager.Instance.a = false;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (IsExplainOpen == true)
            {
                Time.timeScale = 1;
                IsExplainOpen = false;
                ExplainUI.SetActive(false);
                GameManager.Instance.IsUIActivate = false;
            }
            else if (IsShopOpen != true)
            {
                Time.timeScale = 0;
                IsExplainOpen = true;
                ExplainUI.SetActive(true);
                GameManager.Instance.IsUIActivate = true;

            }
        }

    }
    void TurnOnShopUI()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (IsShopOpen == false && IsExplainOpen == false)
            {
                Time.timeScale = 0;
                ShopUI.SetActive(true);
                GameManager.Instance.IsUIActivate = true;
                Cursor.lockState = CursorLockMode.None;
                detailX = 0f;
                detailY = 0f;
                IsShopOpen = true;
            }
            else if(IsExplainOpen != true)
            {
                Time.timeScale = 1;
                ShopUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                detailX = 5f;
                detailY = 5f;
                IsShopOpen = false;
                GameManager.Instance.IsUIActivate = false;
            }
        }

    }
}
