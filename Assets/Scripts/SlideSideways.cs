using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlideSideways : MonoBehaviour
{
    [SerializeField] private Vector2 _speedRange;
    [SerializeField] private Vector2 _xBounds;

    private float _speed;
    private float _xDiff;

    private void Awake()
    {
        _xDiff = _xBounds.y - _xBounds.x;
    }
    
    private void Start()
    {
        var r = new Unity.Mathematics.Random((uint)(Time.time*1000f));
        _speed = r.NextFloat(_speedRange.x, _speedRange.y);
    }

    private void LateUpdate()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if (transform.position.x < _xBounds.x)
        {
            transform.position += Vector3.right * _xDiff;
        }
        else if (transform.position.x > _xBounds.y)
        {
            transform.position -= Vector3.right * _xDiff;
        }
    }
}
