using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlideSideways : MonoBehaviour
{
    [SerializeField] private Vector2 _speedRange;

    private float _speed;
    
    private void Start()
    {
        var r = new Unity.Mathematics.Random((uint)(Time.time*1000f));
        _speed = r.NextFloat(_speedRange.x, _speedRange.y);
    }

    private void LateUpdate()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
    }
}
