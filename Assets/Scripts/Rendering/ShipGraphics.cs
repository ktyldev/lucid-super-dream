using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShipGraphics : MonoBehaviour
{
    [SerializeField] private Transform _ship;
    [SerializeField] private SerialFloat _horizontalInput;
    
    [SerializeField] private SerialFloat _distanceSinceLastBeat;
    [SerializeField] private float _bounce;

    [SerializeField] private float _maxTilt;
    [Range(0, 100)] [SerializeField] private float _tiltSensitivity;
    
    private Renderer _renderer;
    private float _tilt;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        var targetRotationAngle = _maxTilt * _horizontalInput;
        _tilt = Mathf.Lerp(_tilt, targetRotationAngle, _tiltSensitivity * Time.deltaTime);

        var rot = Quaternion.Euler(0, 0, -_tilt);
        transform.rotation = rot;

        transform.position = _ship.position + Vector3.up * _distanceSinceLastBeat * _bounce;
    }
}
