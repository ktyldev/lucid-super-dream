using System;
using Ktyl.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using PlayerInput = Input.PlayerInput;

public class MoveCrosshair : MonoBehaviour
{
    [SerializeField] private SerialFloat speed;
    [SerializeField] private RectOffset extents;
    [SerializeField] private RectTransform canvas;
    
    private PlayerInput _input;
    
    private Vector2 _currentInput;

    private RectTransform _rt;

    private Rect _pos;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _input ??= new PlayerInput();
        
        _input.Enable();
        _input.Default.Aim.performed += DoAim;
    }
    
    private void OnDisable()
    {
        _input.Disable();
        _input.Default.Aim.performed -= DoAim;
    }

    private void Update()
    {
        _rt.anchoredPosition += _currentInput * speed * Time.deltaTime;
        _pos = extents.Remove(canvas.rect);
        _pos.center += new Vector2(canvas.rect.width / 2f, canvas.rect.height / 2f);
        _rt.anchoredPosition = new Vector3(
            Mathf.Clamp(_rt.anchoredPosition.x, _pos.xMin, _pos.xMax),
            Mathf.Clamp(_rt.anchoredPosition.y, _pos.yMin, _pos.yMax));
    }

    private void DoAim(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        
        switch (context.control.device)
        {
            case Gamepad _:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _currentInput = value;
                break;
            case Mouse _:
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
                _currentInput =Vector2.zero;
                transform.position = value;
                break;
        }
    }
}
