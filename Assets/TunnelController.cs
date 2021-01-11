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
    [SerializeField] private SerialFloat _baseTubeRadius;

    [SerializeField] private AccessibilityOptions _accessibility;
    [SerializeField] private float _baseCameraShake;
    [SerializeField] private SerialFloat _distanceToNextBeat;

    [SerializeField] private Renderer _calm;
    [SerializeField] private Renderer _vibe;
    [SerializeField] private Renderer _party;
    public Renderer Active { get; private set; }
    
    private float _start = -1;

    private void Awake()
    {
        Shader.SetGlobalFloat("_CameraShake", 0);
        Shader.SetGlobalFloat("_BaseTubeRadius", _baseTubeRadius);
    
        Debug.Log(_accessibility.Mode);
        switch (_accessibility.Mode)
        {
            case AccessibilityMode.Calm:
                _calm.enabled = true;
                Active = _calm;
                
                _vibe.enabled = false;
                _party.enabled = false;
                break;
            case AccessibilityMode.Vibe:
                _vibe.enabled = true;
                Active = _vibe;
                
                _calm.enabled = false;
                _party.enabled = false;
                break;
            case AccessibilityMode.Party:
                _party.enabled = true;
                Active = _party;
                
                _calm.enabled = false;
                _vibe.enabled = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void LateUpdate()
    {
        var cameraShakeIntensity = _intensity * _accessibility.Intensity.Value;
        
        Shader.SetGlobalFloat("_PlayerXMove", _playerXMove);
        Shader.SetGlobalFloat("_PlayerXPos", _playerXPos);
        Shader.SetGlobalFloat("_CameraShake", _baseCameraShake * _distanceToNextBeat * cameraShakeIntensity);
        
        var elapsed = Time.time - _start;
        if (elapsed > _duration) return;

        var normalisedElapsed = elapsed / _duration;

        var shake = Mathf.Max(_anim.Evaluate(normalisedElapsed), _baseCameraShake) * _intensity;
        Shader.SetGlobalFloat("_CameraShake", shake);
    }

    public void LoseLifeShake()
    {
        _start = Time.time;
    }
}
