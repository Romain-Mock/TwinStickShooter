using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerInputActions _playerInputAction;
    private FieldOfView _fov;
    private PlayerStateFactory _states;
    public Weapon _currentWeapon;

    Vector3 mouseWorldPos;

    [Tooltip("Movement speed"), SerializeField]
    private float _moveSpeed = 5f;
    [Tooltip("Roll speed"), SerializeField]
    private float _rollSpeed = 15f;

    public TextMeshPro stateText;

    //Getters and setters
    public PlayerState CurrentState { get; set; }
    public WeaponManager WeaponManager { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public Vector3 MoveDirection { get; private set; }
    public Vector3 AimDirection { get; private set; }
    public float MoveSpeed { get { return _moveSpeed;} set { _moveSpeed = value; } }
    public float RollSpeed { get { return _rollSpeed; } set { _rollSpeed = value; } }
    public bool IsMoving { get; private set; }
    public bool IsAiming { get; private set; }
    public bool IsRolling { get; set; }
    public bool CanMove { get; set; }
    public Vector3 WantedDirection { get; set; }

    private void OnEnable()
    {
        _playerInputAction.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInputAction.Player.Disable();
    }

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        _fov = GetComponent<FieldOfView>();

        _states = new PlayerStateFactory(this);
        CurrentState = _states.Idle();
        CurrentState.EnterState();

        WeaponManager = FindObjectOfType<WeaponManager>();
        _currentWeapon = WeaponManager._currentWeapon;

        _playerInputAction = new PlayerInputActions();
        _playerInputAction.Player.Enable();
        _playerInputAction.Player.Move.started += OnMovementInput;
        _playerInputAction.Player.Move.performed += OnMovementInput;
        _playerInputAction.Player.Move.canceled += OnMovementInput;
        _playerInputAction.Player.FireGamepad.started += OnAimInput;
        _playerInputAction.Player.FireGamepad.performed += OnAimInput;
        _playerInputAction.Player.FireGamepad.canceled += OnAimInput;
        //_playerInputAction.Player.LookMouse.performed += OnMouseMove;
        _playerInputAction.Player.Roll.started += OnRolling;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        stateText.text = CurrentState.ToString();

        _currentWeapon = WeaponManager._currentWeapon;

        CurrentState.UpdateState();

        Rotate(WantedDirection);

        _fov.FindVisibleTargets();

        Rigidbody.velocity = new Vector3(0, -1, 0);
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();   //Movement input (ZQSD / Left stick)
        MoveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized; //Movement normalized in 3D
        IsMoving = moveInput.x != 0 || moveInput.y != 0;
}

    void OnAimInput(InputAction.CallbackContext context)
    {
        Vector2 aimInput = context.ReadValue<Vector2>();   //Aiming input (Mouse / Right stick)
        AimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;   //Aiming normalized in 3D
        IsAiming = aimInput.x != 0 || aimInput.y != 0;
    }

    void OnMouseMove(InputAction.CallbackContext context)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(_playerInputAction.Player.LookMouse.ReadValue<Vector2>());   //Tranform the mouse pos in screen value to world value

        Plane p = new Plane(Vector3.up, Vector3.zero);  //Create a plane for the mouse raycast to hit

        //If the raycast hit the plane
        if (p.Raycast(mouseRay, out float hitDist))
        {
            Vector3 hitPoint = mouseRay.GetPoint(hitDist); //Locate the mouse
            AimDirection = new Vector3(hitPoint.x, 0, hitPoint.z);
            //transform.LookAt(new Vector3(hitPoint.x, transform.position.y, hitPoint.z)); //Rotate the player
        }

        if (context.phase == InputActionPhase.Performed)
            IsAiming = true;
        else if (context.phase == InputActionPhase.Canceled)
            IsAiming = false;
    }

    void OnRolling(InputAction.CallbackContext context)
    {
        CurrentState = _states.Rolling();
        CurrentState.EnterState();
    }

    public void AnimationEnd()
    {
        CurrentState.CheckSwitchStates();
    }

    //Rotate the player to a given vector
    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            Rigidbody.MoveRotation(targetRotation);
        }
    }
}
