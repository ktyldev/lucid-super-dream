using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;
using PlayerInput = Input.PlayerInput;
public class PlayerShootInput : ShootInputBase
{
    private PlayerInput _actions;

    private bool _isShooting;
    
    private void Awake()
    {
        _actions = new PlayerInput();
    }

    private void OnEnable()
    {
        _actions.Enable();
        _actions.Default.Shoot.performed += Shoot;
    }

    private void OnDisable()
    {
        _actions.Disable();
        _actions.Default.Shoot.performed -= Shoot;
    }

    private void Start()
    {
        StartCoroutine(MakePewSounds());
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        _isShooting = obj.ReadValueAsButton();
    }

    public override bool IsShooting()
    {
        return _isShooting;
    }

    private IEnumerator MakePewSounds()
    {
        var wait = new WaitForSeconds(0.02f);
        
        while (true)
        {
            if (_isShooting)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/shoot");
            }
            
            yield return wait;
        }
    }
    
}