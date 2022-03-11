using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;   //Rigidbody of the player
    private PlayerInput playerInput;
    private PlayerInputActions playerInputAction;
    private Animator animator;
    private WeaponManager weaponManager;

    Vector3 mouseWorldPos;

    public enum PlayerState { Idle, OnlyMoving, OnlyAiming, MovingAndAiming };
    PlayerState currentState;

    Vector2 aimInput;

    Vector3 activeDirection;

    [Header("Movement values")]
    private float xMovement;
    private float zMovement;
    [Tooltip("Movement speed")]
    public float moveSpeed = 5f;
    [Range(1, 10), Tooltip("Rotation speed")]
    public float rotateSpeed = 6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        weaponManager = GameObject.FindObjectOfType<WeaponManager>();

        playerInput = GetComponent<PlayerInput>();
        playerInputAction = new PlayerInputActions();
        playerInputAction.Player.Enable();
    }

    void FixedUpdate()
    {
        Vector2 moveInput = playerInputAction.Player.Move.ReadValue<Vector2>();   //Movement input (ZQSD / Left stick)
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized; //Movement normalized in 3D

        Vector2 aimInput = playerInputAction.Player.FireGamepad.ReadValue<Vector2>();   //Aiming input (Mouse / Right stick)
        Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;   //Aiming normalized in 3D

        currentState = GetPlayerState(moveInput, aimInput);

        switch (currentState)
        {
            case PlayerState.OnlyMoving:

                Rotate(moveDirection);

                animator.SetBool("isMoving", true);
                animator.SetBool("isShooting", false);

                rb.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

                break;

            case PlayerState.OnlyAiming:

                Rotate(aimDirection);

                animator.SetBool("isMoving", false);
                animator.SetBool("isShooting", true);

                break;

            case PlayerState.MovingAndAiming:

                Rotate(aimDirection);

                animator.SetBool("isMoving", true);
                animator.SetBool("isShooting", true);   //Animation conditions

                float normalizedAngle = Vector3.Angle(moveDirection, aimDirection) / 180;    //Calaculate the angle between movement and aim and normalize it from 0 to 1
                animator.SetFloat("angle", normalizedAngle);

                rb.position += moveDirection * moveSpeed * Time.fixedDeltaTime;

                break;

            case PlayerState.Idle:

                animator.SetBool("isMoving", false);
                animator.SetBool("isShooting", false);
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

        return state;
    }

    //Rotate the player to look at the mouse
    public void LookMouse(InputAction.CallbackContext context)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(playerInputAction.Player.LookMouse.ReadValue<Vector2>());   //Tranform the mouse pos in screen value to world value

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

            rb.MoveRotation(targetRotation);
        }
    }
}
