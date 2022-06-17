using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEffectCtrl : MonoBehaviour
{
    public enum AttackState { None, Atack }
    public AttackState attackState = AttackState.None;
    public Transform gunPos = null;
    public GameObject shootEffect = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(GameManager.Instance.playerHp > 0)
        {
            CheckClick();
            CheckAttack();
            shootEffect.transform.position = gunPos.position;
            shootEffect.transform.rotation = gunPos.rotation;
        }
    }
    void CheckClick()
    {
        if (Input.GetMouseButton(0))
        {
            attackState = AttackState.Atack;
        }
        else
        {
            attackState = AttackState.None;
        }

    }
    void CheckAttack()
    {
        switch (attackState)
        {
            case AttackState.None:
                shootEffect.SetActive(false);
                break;
            case AttackState.Atack:
                shootEffect.SetActive(true);
                break;
            default:
                break;
        }

    }
}
