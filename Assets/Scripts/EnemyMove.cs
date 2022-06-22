using UnityEngine;
public class EnemyMove : MonoBehaviour
{ 

    public enum EnemyState {None, GoTarget, Attack, Die}
    EnemyState enemyState = EnemyState.None;
    private float moveSpd = 0f;
    public float defaultMoveSpd = 100f;
    public float dmgMoveSpd = 85f;
    public float fogMoveSpd = 93f;
    public GameObject target = null;
    public GameObject Sectarget = null;
    public Transform targetTransform = null;
    public Vector3 posTarget = Vector3.zero;
    private float velgravity = 0f;

    private Rigidbody enemyRigidbody = null;
    private Animator animator = null;
    public Transform enemyTransform = null;
    [Header("전투 속성")]
    public int hp = 100;
    public float AtkRange = 15f;
    public GameObject damageEffect = null;
    public GameObject dieEffect = null;
    float damageTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        enemyState = EnemyState.GoTarget;
        enemyRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        AnimationCtrl();
        
        SetGravity();
        if(GameManager.Instance.probeEndTime <= 0)
        {
            target = Sectarget;
            moveSpd = fogMoveSpd;
        }
        else
        {
            ChkDamageTime();
        }
    }
    void ChkDamageTime()
    {
        damageTime -= Time.deltaTime;
        if (damageTime > 0)
        {
            moveSpd = dmgMoveSpd;
        }
        else
        {
            moveSpd = defaultMoveSpd;
        }
    }

    void SetMove()
    {
        if(Time.timeScale == 0)
        {
            return;
        }
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

        direction = new Vector3(direction.x, 0, direction.z);
        Vector3 amount = Vector3.zero;
        Vector3 vecGra = new Vector3(0f,velgravity,0f);
        amount = (direction * moveSpd);

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

    void DieEvent()
    {
        GameManager.Instance.monsters.Remove(gameObject);
        Transform myTransform = this.transform;
        myTransform.position = new Vector3(myTransform.position.x,1f,myTransform.position.z);
        GameManager.Instance.DropItem(myTransform);
        Destroy(gameObject);
    }
    void DamageEvent()
    {
        GameManager.Instance.SetHp(10);
    }
    void ApplyDamage(int damage)
    {
        hp -= damage;
        if(hp > 0)
        {
            damageTime = 0.1f;
        }
        if (hp <= 0)
        {
            if(enemyState != EnemyState.Die)
            {
                enemyState = EnemyState.Die;
                animator.SetTrigger("Die");
                GameObject effect = Instantiate(dieEffect, enemyTransform.position, Quaternion.identity);
                Destroy(effect, 1f);
            }
        }
    }
    void Explode()
    {
        ApplyDamage(200);
    }

    void SetGravity()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit raycastHit;
        Vector3 hitVec = Vector3.zero;
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
        {
            hitVec.y = raycastHit.point.y;
        }
        if (transform.position.y <= hitVec.y)
        {
            velgravity = 0;
        }
        else
        {
            velgravity -= Time.deltaTime * 20f;
        }
    }


}
