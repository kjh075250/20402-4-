using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCtrl : MonoBehaviour
{
    private CharacterController characterController = null;
    private CollisionFlags collisionFlags = CollisionFlags.None;
    private Animator animator = null;
    [Header("¼Ó¼º")]
    public float walkSpeed = 3f;
    public float runSpeed = 10f;
    public float currentFireRate = 0f;
    public float currentGrenadeRate = 0f;
    private float radius = 20f;
    public float power = 500f;
    public float flyingDistance = 10f;


    public enum PlayerState { None, Idle, Walk, Run}

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
    // Start is called before the first frame update
    void Start()
    {
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
    }
    void CalFireRate()
    {
        if(currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
        if (currentGrenadeRate > 0)
        {
            currentGrenadeRate -= Time.deltaTime;
        }

    }

    void Move()
    {
        Transform cameraTrans = Camera.main.transform;
        Vector3 forward = cameraTrans.TransformDirection(Vector3.forward);
        forward.y = 0f;
        Quaternion rot = new Quaternion(transform.rotation.x,cameraTrans.rotation.y,0,cameraTrans.rotation.w);

        Vector3 right = new Vector3(forward.z, 0f, -forward.x);

        float vertical = Input.GetAxis("Vertical");
        float Horizontal = Input.GetAxis("Horizontal");

        Vector3 targetDirection = vertical * forward + Horizontal * right;

        moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, dirRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);

        moveDirection = moveDirection.normalized;

        float speed = walkSpeed;
       
        Vector3 gravityVec = new Vector3(0f, 0f, 0f);
        Vector3 moveAmount = (moveDirection * speed * Time.deltaTime);
        animator.SetFloat("moveSpeed", moveAmount.magnitude *100f);
        transform.rotation = rot;
        collisionFlags = characterController.Move(moveAmount);
    }
    void AimRayCast()
    {
        if (GameManager.Instance.playerHp <= 0) return;
        Vector3 posStart = aimTransform.position;
        Vector3 posTarget = aimTransform.forward;
        Ray ray = new Ray(posStart,posTarget);
        RaycastHit rayHit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        if (Input.GetMouseButton(0) && currentFireRate <= 0)
        {
            
                if (Physics.Raycast(ray, out rayHit, 1000f, LayerMask.GetMask("Monster")))
                {
                    rayHit.transform.SendMessage("ApplyDamage", 20);
                    GameObject clone = Instantiate(damageEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    Destroy(clone, 0.5f);
                }
            shootLight.SetActive(true);
            currentFireRate = 0.1f;
        }
        else
        {
            shootLight.SetActive(false);
        }

    }
    void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentGrenadeRate <= 0)
        {
            clone = Instantiate(grenadeObject,new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z),Quaternion.identity);
            Rigidbody rigi = clone.GetComponent<Rigidbody>();
            rigi.AddForce(camTransform.forward * 500);
            currentGrenadeRate += 3f;
        }
        if(clone != null)
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
                        }
                    }
                }
                Destroy(Explode, 1f);
                Destroy(clone);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Money")
        {
            GameManager.Instance.money += 10;
            Destroy(other.gameObject);
        }
    }
    public void OnClickItem1()
    {
        if(GameManager.Instance.money >= 20)
        {
            grenadeEffect = CameraCtrl.EffectsList[0];
            GameManager.Instance.money -= 20;
            Debug.Log(grenadeEffect);
        }
        else
        {
            Debug.Log("No Money!");
        }
    }
    public void OnClickItem2()
    {
        if (GameManager.Instance.money >= 40)
        {
            grenadeEffect = CameraCtrl.EffectsList[1];
            GameManager.Instance.money -= 40;
            Debug.Log(grenadeEffect);
        }
        else
        {
            Debug.Log("No Money!");
        }
    }
    public void OnClickItem3()
    {
        if (GameManager.Instance.money >= 60)
        {
            grenadeEffect = CameraCtrl.EffectsList[2];
            GameManager.Instance.money -= 60;
            Debug.Log(grenadeEffect);
        }
        else
        {
            Debug.Log("No Money!");
        }
    }
}
