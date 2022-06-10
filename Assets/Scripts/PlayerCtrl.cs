using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    private CharacterController characterController = null;
    private CollisionFlags collisionFlags = CollisionFlags.None;
    private Animator animator = null;
    [Header("¼Ó¼º")]
    public float walkSpeed = 3f;
    public float runSpeed = 10f;
    public float currentFireRate = 0f;


    public enum PlayerState { None, Idle, Walk, Run}

    public PlayerState playerState = PlayerState.None;


    public Vector3 moveDirection = Vector3.zero;
    public float dirRotateSpeed = 5f;
    public Transform aimTransform = null;
    public GameObject shootEffect = null;
    public GameObject damageEffect = null;
    public GameObject shootLight = null;
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
    }
    void CalFireRate()
    {
        if(currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
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

}
