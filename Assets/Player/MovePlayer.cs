using Ktyl.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInput = Input.PlayerInput;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] private SerialFloat speed;
    [SerializeField] private RectOffset extents;
    [SerializeField] private float lerpAmount = 0.95f;
    [SerializeField] private SerialFloat horizontalInput;
    [SerializeField] private SerialFloat horizontalPosition;
    [SerializeField] private SerialFloat zPosition;
    
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

    private float _x;
    
    private void Update()
    {
        _transform.localPosition += (Vector3) _currentInput * speed * Time.deltaTime;

        var w = Mathf.Abs(extents.right - extents.left);
        var x = _transform.localPosition.x;
        if (x > extents.right)
        {
            x -= w;
        }
        else if (x < extents.left)
        {
            x += w;
        }
        
        _transform.localPosition = new Vector3(
            x,
            _yPos,
            _zPos);
        
        horizontalInput.Value = _currentInput.x;
        horizontalPosition.Value = _transform.localPosition.x;
        zPosition.Value = transform.position.z;
    }

    private void DoMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        _currentInput = Vector2.Lerp(_currentInput, value, lerpAmount);
    }
}
