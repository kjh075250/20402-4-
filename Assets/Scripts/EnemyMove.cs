using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonsterCtrl
{
    public enum EnemyState {None, GoTarget, Attack }
    EnemyState enemyState = EnemyState.None;
    public float moveSpd = 100f;
    public GameObject target = null;
    public Transform targetTransform = null;
    public Vector3 posTarget = Vector3.zero;

    public Rigidbody enemyRigidbody = null;
    public Animator animator = null;
    public Transform enemyTransform = null;

    [Header("전투 속성")]
    public int hp = 100;
    public float AtkRange = 15f;
    public GameObject damageEffect = null;
    public GameObject dieEffect = null;


    // Start is called before the first frame update
    void Start()
    {
        enemyState = EnemyState.GoTarget;
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();

        AnimationCtrl();
    }
    void OnDieAnimationFinished()
    {
        Instantiate(dieEffect, enemyTransform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    void SetMove()
    {
        Vector3 distance = Vector3.zero;
        Vector3 posLookAt = Vector3.zero;

        if (target != null)
        {
            distance = target.transform.position - enemyTransform.position;
            if (distance.magnitude < AtkRange)
            {
                enemyState = EnemyState.Attack;
                return;
            }
            posLookAt = new Vector3(target.transform.position.x, enemyTransform.position.y, target.transform.position.z);
        }
        Vector3 direction = distance.normalized;

        direction = new Vector3(direction.x, 0f, direction.z);

        Vector3 amount = direction * moveSpd;
        
        enemyRigidbody.AddForce(amount);
        enemyTransform.LookAt(posLookAt);

    }
    void SetAtk()
    {
        float distance = Vector3.Distance(targetTransform.position, enemyTransform.position); 

        if (distance > AtkRange + 1f)
        {
            enemyState = EnemyState.GoTarget;
        }
    }
    void AnimationCtrl()
    {
        switch(enemyState)
        {
            case EnemyState.GoTarget:
                animator.SetBool("Run Forward", true);
                break;
            case EnemyState.Attack:
                animator.SetBool("Run Forward", false);
                animator.SetTrigger("Stab Attack");
                break;
            default:
                break;
        }
    }
    void CheckState()
    {
        switch(enemyState)
        {
            case EnemyState.GoTarget:
                SetMove();
                break;
            case EnemyState.Attack:
                SetAtk();
                break;
            default:
                break;
        }
    }
}
