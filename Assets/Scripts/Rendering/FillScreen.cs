using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FillScreen : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    
    void LateUpdate()
    {
        var pos = (_camera.farClipPlane * 0.9f);
        var camTrans = _camera.transform;
        var trans = transform;
        var h = Mathf.Tan(_camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;

        trans.position = camTrans.position + camTrans.forward * pos;
        trans.localScale = new Vector3(h*_camera.aspect,h,1);
        trans.LookAt(trans.position + camTrans.forward);
    }
}
