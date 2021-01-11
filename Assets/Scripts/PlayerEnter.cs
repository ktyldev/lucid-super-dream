using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerEnter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(Vector3.zero, 0.5f).SetEase(Ease.OutQuint);
    }
}
