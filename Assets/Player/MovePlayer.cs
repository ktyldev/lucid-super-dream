using System;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PlayerInput = Input.PlayerInput;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] private SerialFloat speed;
    [SerializeField] private RectOffset extents;
    [SerializeField] private float lerpAmount = 0.95f;
    
    private PlayerInput _input;
    
    private Vector2 _currentInput;

    private Transform _transform;
    
    private Rect _pos;

    private float _yPos;
    private float _zPos;
    
    private void Awake()
    {
        _transform = transform;
        _yPos = _transform.localPosition.y;
        _zPos = _transform.localPosition.z;
    }

    private void OnEnable()
    {
        _input ??= new PlayerInput();
        
        _input.Enable();
        _input.Default.Move.performed += DoMove;
    }
    
    private void OnDisable()
    {
        _input.Disable();
        _input.Default.Move.performed -= DoMove;
    }

    private void Update()
    {
        _transform.localPosition += (Vector3) _currentInput * speed * Time.deltaTime;
        
        _transform.localPosition = new Vector3(
            Mathf.Clamp(_transform.localPosition.x, extents.left, extents.right),
            _yPos,
            _zPos);
    }

    private void DoMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        _currentInput = Vector2.Lerp(_currentInput, value, lerpAmount);
    }
}
