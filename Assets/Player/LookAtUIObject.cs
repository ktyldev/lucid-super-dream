using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUIObject : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float distance = 20;
    [SerializeField] private RectTransform lookAt;

    private Vector3 _pos;
    
    private void LateUpdate()
    {
        _pos = cam.ScreenToWorldPoint((Vector3)lookAt.position + Vector3.forward * distance);
        transform.LookAt(_pos, cam.transform.up);
    }

    private void OnDrawGizmos()
    {
        _pos = cam.ScreenToWorldPoint((Vector3)lookAt.position + Vector3.forward * distance);
        Gizmos.DrawWireSphere(_pos, .5f);
    }
}
