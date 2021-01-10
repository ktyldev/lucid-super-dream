using System;
using System.Collections;
using System.Collections.Generic;
using Ktyl.Util;
using UnityEngine;

public class TunnelController : MonoBehaviour
{
    [SerializeField] private SerialFloat _intensity;
    [SerializeField] private SerialFloat _duration;
    [SerializeField] private AnimationCurve _anim;
    [SerializeField] private SerialFloat _playerXPos;
    [SerializeField] private SerialFloat _playerXMove;

    private float _start = -1;

    private void Awake()
    {
        Shader.SetGlobalFloat("_CameraShake", 0);
    }

    void LateUpdate()
    {
        Shader.SetGlobalFloat("_PlayerXMove", _playerXMove);
        Shader.SetGlobalFloat("_PlayerXPos", _playerXPos);
        
        var elapsed = Time.time - _start;
        if (elapsed > _duration) return;

        var normalisedElapsed = elapsed / _duration;
        
        Shader.SetGlobalFloat("_CameraShake", _anim.Evaluate(normalisedElapsed) * _intensity);
    }

    public void LoseLifeShake()
    {
        _start = Time.time;
    }
}
