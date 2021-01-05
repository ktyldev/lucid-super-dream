using System;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private void Shoot(InputAction.CallbackContext obj)
    {
        _isShooting = obj.ReadValueAsButton();
    }

    public override bool IsShooting()
    {
        return _isShooting;
    }

}