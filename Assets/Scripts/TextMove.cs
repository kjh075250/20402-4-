using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TextMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponentInChildren<Transform>().transform.DOMoveY(3, 4f);
        this.GetComponentInChildren<Text>().DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() =>
        {
            Debug.Log("r");
            Destroy(gameObject);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
