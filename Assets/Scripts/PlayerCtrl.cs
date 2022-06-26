using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerCtrl : MonoBehaviour
{
    private CharacterController characterController = null;
    private CollisionFlags collisionFlags = CollisionFlags.None;
    private Animator animator = null;
    [Header("¼Ó¼º")]
    public float walkSpeed = 3f;
    public float runSpeed = 24f;
    public float currentFireRate = 0f;
    public float currentGrenadeCoolTime = 0f;
    public float currentTankCoolTime = 0f;
    public float currentTankSkillRate = 0f;
    public float currentMoneyCoolTime = 0f;
    public bool _isMoneyActivate = false;
    private int barriCount = 1;
    public float maxGrenadeRate = 3f;
    private float radius = 20f;
    public float power = 500f;
    public float flyingDistance = 10f;
    private bool _isQskillOn = false;
    private bool _isQskillActivate = false;
    private float aniMoveSpd = 100f;

    public enum PlayerState { None, Idle, Walk, Run }

    public PlayerState playerState = PlayerState.None;

    private GameObject clone = null;

    public Vector3 moveDirection = Vector3.zero;
    public float dirRotateSpeed = 5f;
    public Transform aimTransform = null;
    public Transform camTransform = null;
    public Transform headTransform = null;

    public GameObject damageEffect = null;
    public GameObject shootLight = null;
    public GameObject grenadeObject = null;
    public GameObject grenadeEffect = null;
    public GameObject tank = null;
    public GameObject Soldier = null;
    public GameObject buyTrans = null;

    public Image cooltimeImage1 = null;
    public Image cooltimeImage2 = null;
    public Image cooltimeImage3 = null;
    public Image cooltimeImage4 = null;
    
    public Text item1Tex = null;
    public Text item2Tex = null;
    public Text buyTex = null;

    // Start is called before the first frame update
    void Start()
    {
        tank.SetActive(false);
        cooltimeImage1.fillAmount = 0;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        AimRayCast();
        CalFireRate();
        UseSkill();
        CheckSkill();
    }
    void CalFireRate()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
        if (currentGrenadeCoolTime > 0)
        {
            currentGrenadeCoolTime -= Time.deltaTime;
            cooltimeImage1.fillAmount = currentGrenadeCoolTime / 3f;
        }
        if (currentTankCoolTime > 0)
        {
            currentTankCoolTime -= Time.deltaTime;
            cooltimeImage2.fillAmount = currentTankCoolTime / 30f;
        }
        if (currentTankSkillRate > 0)
        {
            currentTankSkillRate -= Time.deltaTime;
        }
        if (currentMoneyCoolTime > 0)
        {
            currentMoneyCoolTime -= Time.deltaTime;
            cooltimeImage3.fillAmount = currentMoneyCoolTime / 1f;
        }
    }

    void Move()
    {
        Transform cameraTrans = Camera.main.transform;
        Vector3 forward = cameraTrans.TransformDirection(Vector3.forward);
        forward.y = 0f;
        Quaternion rot = new Quaternion(transform.rotation.x, cameraTrans.rotation.y, 0, cameraTrans.rotation.w);

        Vector3 right = new Vector3(forward.z, 0f, -forward.x);

        float vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");

        Vector3 targetDirection = vertical * forward + Horizontal * right;
        moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, dirRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);

        moveDirection = moveDirection.normalized;
        Soldier.SetActive(true);
        float speed = walkSpeed;
        if (_isQskillOn == true)
        {
            speed = runSpeed;
            Camera.main.fieldOfView = 65f;
            Soldier.SetActive(false);
            tank.SetActive(true);
        }
        Camera.main.fieldOfView = 60f;
        Vector3 gravityVec = new Vector3(0f, 0f, 0f);
        Vector3 moveAmount = (moveDirection * speed * Time.deltaTime);
        if (_isQskillOn != true)
        {
            animator.SetFloat("moveSpeed", moveAmount.magnitude * aniMoveSpd);
        }
        else
        {
            animator.SetFloat("moveSpeed", 0f);
        }
        transform.rotation = rot;
        collisionFlags = characterController.Move(moveAmount);
    }

    void AimRayCast()
    {
        if (GameManager.Instance.playerHp <= 0) return;
        Vector3 posStart = aimTransform.position;
        Vector3 posTarget = aimTransform.forward;
        Ray ray = new Ray(posStart, posTarget);
        RaycastHit rayHit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        if (Input.GetMouseButton(0) && currentFireRate <= 0)
        {
            if (Physics.Raycast(ray, out rayHit, 1000f, LayerMask.GetMask("Monster")))
            {
                int damage;
                if (_isQskillOn)
                {
                    damage = 40;
                    Debug.Log(damage);
                }
                else
                {
                    damage = 20;
                }
                rayHit.transform.SendMessage("ApplyDamage", damage);
                GameObject clone = Instantiate(damageEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                Destroy(clone, 0.5f);
            }
            shootLight.SetActive(true);
            currentFireRate = 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.E) && currentMoneyCoolTime <= 0 && _isMoneyActivate == true)
        {
            if (Physics.Raycast(ray, out rayHit, 1000f, LayerMask.GetMask("Item")))
            {
                GameManager.Instance.money += 10;
                GameManager.Instance.SetScore(10, "Á¡¼ö +10(µ· È¹µæ) ");
                Destroy(rayHit.transform.gameObject);
                currentMoneyCoolTime += 1f;
            }
        }
        else
        {
            shootLight.SetActive(false);
        }

    }
    void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentGrenadeCoolTime <= 0)
        {
            clone = Instantiate(grenadeObject, new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), Quaternion.identity);
            Rigidbody rigi = clone.GetComponent<Rigidbody>();
            rigi.AddForce(camTransform.forward * 500);
            currentGrenadeCoolTime = 3f;
        }
        if (clone != null)
        {
            Ray ray = new Ray(clone.transform.position, Vector3.down);
            RaycastHit raycastHit;
            Vector3 hitVec = Vector3.zero;
            if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
            {
                hitVec.y = raycastHit.point.y;
            }
            if (clone.transform.position.y <= hitVec.y)
            {

                GameObject Explode = Instantiate(grenadeEffect, clone.transform.position, Quaternion.identity);
                Vector3 posSkillEffect = clone.transform.position;
                Collider[] colliders = Physics.OverlapSphere(posSkillEffect, 5f);
                foreach (Collider collider in colliders)
                {
                    Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        if (collider.CompareTag("Monster") == true)
                        {
                            collider.SendMessage("Explode");
                            GameManager.Instance.SetScore(50, "Á¡¼ö +50(¼ö·ùÅº »ç¿ë)");
                        }
                    }
                }
                Destroy(Explode, 1f);
                Destroy(clone);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && currentTankCoolTime <= 0 && _isQskillActivate == true
            && GameManager.Instance.IsEscapeActivate == false)
        {
            if (_isQskillOn == false)
            {
                _isQskillOn = true;
                Debug.Log(_isQskillOn);
            }
            currentTankSkillRate += 15f;
            currentTankCoolTime += 30f;
        }
        if (Input.GetKeyDown(KeyCode.R) && GameManager.Instance.IsBarriActivate == false && barriCount > 0)
        {
            barriCount -= 1;
            GameManager.Instance.barriHp = 200;
            GameManager.Instance.BarriGageImage.rectTransform.localScale = new Vector3(200 / 200f, 1f, 1f);
            GameManager.Instance.barri.SetActive(true);
            GameManager.Instance.barri.transform.position = transform.position + Vector3.forward * 2f + Vector3.up;
            GameManager.Instance.IsBarriActivate = true;
            GameManager.Instance.BarriCtrlObject.SetActive(true);
        }
    }
    void CheckSkill()
    {
        if (GameManager.Instance.IsEscapeActivate == true)
        {
            _isQskillActivate = false;
            _isQskillOn = false;
        }
        if (_isQskillActivate == false)
        {
            cooltimeImage2.fillAmount = 1;
        }
        if (currentTankSkillRate <= 0)
        {
            _isQskillOn = false;
        }
        if (_isQskillOn == false)
        {
            tank.SetActive(false);
        }
        cooltimeImage4.fillAmount = barriCount;   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Money")
        {
            GameManager.Instance.money += 10;
            GameManager.Instance.SetScore(10, "Á¡¼ö +10(µ· È¹µæ) ");
            Destroy(other.gameObject);
        }
        if (other.tag == "Escape")
        {
            GameManager.Instance.SetScore(300, "Á¡¼ö +300(Å»Ãâ)");
            GameManager.Instance.CheckEscape();
            Debug.Log("Å»Ãâ ¼º°ø");
        }
    }
    public void OnClickItem1()
    {
        if (GameManager.Instance.money >= 40)
        {
            if (_isMoneyActivate == false)
            {
                _isMoneyActivate = true;
                GameManager.Instance.money -= 40;
                cooltimeImage3.fillAmount = 0f;
                item1Tex.text = "±¸¸Å ¿Ï·á";
                buyTex.text = "±¸¸Å ¼º°ø!";
            }
        }
        else
        {
            buyTex.text = "µ· ºÎÁ·";

        }
    }
    public void OnClickItem2()
    {
        if (GameManager.Instance.money >= 60)
        {
            if (_isQskillActivate == false)
            {
                _isQskillActivate = true;
                cooltimeImage2.fillAmount = 0f;
                Debug.Log(_isQskillActivate);
                GameManager.Instance.money -= 60;
                item2Tex.text = "±¸¸Å ¿Ï·á";
                buyTex.text = "±¸¸Å ¼º°ø!";

            }
        }
        else
        {
            buyTex.text = "µ· ºÎÁ·";

        }
    }
    public void OnClickItem3()
    {
        if (GameManager.Instance.money >= 100)
        {
            barriCount += 1;
            GameManager.Instance.money -= 100;
            buyTex.text = "±¸¸Å ¼º°ø!";

        }
        else
        {
            buyTex.text = "µ· ºÎÁ·";

        }
    }
}
