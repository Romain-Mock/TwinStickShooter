using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerInputActions _playerInputAction;
    private Animator _animator;

    //States
    private PlayerState _currentState;
    private PlayerStateFactory _states;

    Vector3 mouseWorldPos;

    private bool _canMove;
    private Vector3 _moveDirection;
    private bool _isMoving;

    private Vector3 _aimDirection;
    private bool _isAiming;
    private Vector3 _wantedDirection;

    private bool _isRolling;

    [Tooltip("Movement speed"), SerializeField]
    private float _moveSpeed = 5f;
    [Tooltip("Roll speed"), SerializeField]
    private float _rollSpeed = 15f;

    //Getters and setters
    public PlayerState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Rigidbody Rigidbody { get { return _rb; } }
    public Animator Animator { get { return _animator; } }
    public Vector3 MoveDirection { get { return _moveDirection; } }
    public Vector3 AimDirection { get { return _aimDirection; } }
    public float MoveSpeed { get { return _moveSpeed;} set { _moveSpeed = value; } }
    public float RollSpeed { get { return _rollSpeed; } set { _rollSpeed = value; } }
    public bool IsMoving { get { return _isMoving; } }
    public bool IsAiming { get { return _isAiming; } }
    public bool IsRolling { get { return _isRolling; } set { _isRolling = value; } }
    public bool CanMove { get { return _canMove; } set { _canMove = value; } }
    public Vector3 WantedDirection { get { return _wantedDirection; } set { _wantedDirection = value; } }


    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _playerInputAction = new PlayerInputActions();
        _playerInputAction.Player.Enable();
        _playerInputAction.Player.Move.started += OnMovementInput;
        _playerInputAction.Player.Move.performed += OnMovementInput;
        _playerInputAction.Player.Move.canceled += OnMovementInput;
        _playerInputAction.Player.FireGamepad.started += OnAimInput;
        _playerInputAction.Player.FireGamepad.performed += OnAimInput;
        _playerInputAction.Player.FireGamepad.canceled += OnAimInput;
        _playerInputAction.Player.Roll.started += OnRolling;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _currentState.UpdateStates();

        Rotate(_wantedDirection);
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();   //Movement input (ZQSD / Left stick)
        _moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized; //Movement normalized in 3D
        _isMoving = moveInput.x != 0 || moveInput.y != 0;
}

    void OnAimInput(InputAction.CallbackContext context)
    {
        Vector2 aimInput = context.ReadValue<Vector2>();   //Aiming input (Mouse / Right stick)
         _aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;   //Aiming normalized in 3D
        _isAiming = aimInput.x != 0 || aimInput.y != 0;
    }

    void OnRolling(InputAction.CallbackContext context)
    {
        _currentState = _states.Rolling();
        _currentState.EnterState();
    }

    public void EndRoll()
    {
        _isRolling = false;
    }

    private void OnEnable()
    {
        _playerInputAction.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInputAction.Player.Disable();
    }

    //Rotate the player to a given vector
    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _rb.MoveRotation(targetRotation);
        }
    }
}
