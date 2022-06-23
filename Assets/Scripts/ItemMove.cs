using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemMove : MonoBehaviour
{
    //�� �������� �����Ǹ� DoTween
    void Start()
    {
        transform.DOMoveY(2, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        transform.DORotate(Vector3.up * 180, 4f).SetLoops(-1, LoopType.Yoyo);
    }
    
}
