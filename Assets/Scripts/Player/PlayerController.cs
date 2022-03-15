using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;   //Rigidbody of the player
    private PlayerInput _playerInput;
    private PlayerInputActions _playerInputAction;
    private Animator _animator;
    private Roll _roll;

    Vector3 mouseWorldPos;

    public enum PlayerState { Idle, OnlyMoving, OnlyAiming, MovingAndAiming, Rolling };
    PlayerState currentState;

    [Tooltip("Movement speed")]
    public float moveSpeed = 5f;
    [Range(1, 10), Tooltip("Rotation speed")]
    public float rotateSpeed = 6f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _roll = GetComponent<Roll>();

        _playerInput = GetComponent<PlayerInput>();
        _playerInputAction = new PlayerInputActions();
        _playerInputAction.Player.Enable();
        _playerInputAction.Player.Roll.started += OnRolling;
    }

    void FixedUpdate()
    {
        Vector2 moveInput = _playerInputAction.Player.Move.ReadValue<Vector2>();   //Movement input (ZQSD / Left stick)
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized; //Movement normalized in 3D

        Vector2 aimInput = _playerInputAction.Player.FireGamepad.ReadValue<Vector2>();   //Aiming input (Mouse / Right stick)
        Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;   //Aiming normalized in 3D

        currentState = GetPlayerState(moveInput, aimInput);
        Debug.Log(currentState.ToString());

        switch (currentState)
        {
            case PlayerState.OnlyMoving:

                Rotate(moveDirection);
                _rb.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

                _animator.SetBool("isMoving", true);
                _animator.SetBool("isShooting", false);


                break;

            case PlayerState.OnlyAiming:

                Rotate(aimDirection);

                _animator.SetBool("isMoving", false);
                _animator.SetBool("isShooting", true);

                break;

            case PlayerState.MovingAndAiming:

                Rotate(aimDirection);

                _animator.SetBool("isMoving", true);
                _animator.SetBool("isShooting", true);   //Animation conditions

                float normalizedAngle = Vector3.Angle(moveDirection, aimDirection) / 180;    //Calaculate the angle between movement and aim and normalize it from 0 to 1
                _animator.SetFloat("angle", normalizedAngle);

                _rb.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

                break;

            case PlayerState.Idle:

                _animator.SetBool("isMoving", false);
                _animator.SetBool("isShooting", false);
                break;

            case PlayerState.Rolling:

                _rb.AddForce(moveDirection, ForceMode.Impulse);

                break;
        }
    }

    PlayerState GetPlayerState(Vector3 movement, Vector3 aim)
    {
        PlayerState state;

        if (movement != Vector3.zero)
        {
            if (aim != Vector3.zero)
            {
                state = PlayerState.MovingAndAiming;
            }
            else
            {
                state = PlayerState.OnlyMoving;
            }
        }
        else
        {
            if (aim != Vector3.zero)
            {
                state = PlayerState.OnlyAiming;

                if (movement != Vector3.zero)
                    state = PlayerState.MovingAndAiming;
            }
            else
            {
                state = PlayerState.Idle;
            }
        }
        if (_roll.IsRolling)
            state = PlayerState.Rolling;
        else
            state = PlayerState.Idle;

        return state;
    }

    //Rotate the player to look at the mouse
    public void LookMouse(InputAction.CallbackContext context)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(_playerInputAction.Player.LookMouse.ReadValue<Vector2>());   //Tranform the mouse pos in screen value to world value

        Plane p = new Plane(Vector3.up, Vector3.zero);  //Create a plane for the mouse raycast to hit

        //If the raycast hit the plane
        if (p.Raycast(mouseRay, out float hitDist))
        {
            Vector3 hitPoint = mouseRay.GetPoint(hitDist); //Locate the mouse
            transform.LookAt(new Vector3(hitPoint.x, transform.position.y, hitPoint.z)); //Rotate the player
        }
    }

    //Rotate the player to a given vector
    void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _rb.MoveRotation(targetRotation);
        }
    }

    void OnRolling(InputAction.CallbackContext context)
    {
        _animator.SetTrigger("Roll");
    }
}
