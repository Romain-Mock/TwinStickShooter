using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Weapon weapon;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputAction;
    private Animator animator;
    public Transform aimTarget;
    public float height;
    Vector3 mouseWorldPos;

    public enum PlayerState { Idle, OnlyMoving, OnlyAiming, MovingAndAiming };
    PlayerState currentState;

    Vector2 aimInput;

    Vector3 activeDirection;

    private float xMovement;
    private float zMovement;
    public float moveSpeed = 5f;
    [Range(1, 10)]
    public float rotateSpeed = 6f;
    public bool rotateSmooth;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        playerInputAction = new PlayerInputActions();
        playerInputAction.Player.Enable();
    }

    private void Start()
    {
        weapon = GameObject.FindObjectOfType<Weapon>();
    }

    void FixedUpdate()
    {
        Vector2 inputVector = playerInputAction.Player.Move.ReadValue<Vector2>();   //Movement input (ZQSD / Left stick)
        Vector3 movement = new Vector3(inputVector.x, 0, inputVector.y).normalized; //Movement normalized in 3D
        Vector3 movePos = new Vector3(inputVector.x * 5, 3, inputVector.y * 5);

        Vector2 aimInput = playerInputAction.Player.FireGamepad.ReadValue<Vector2>();   //Aiming input (Mouse / Right stick)
        Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;   //Aiming normalized in 3D
        Vector3 targetPos = new Vector3(aimInput.x * 5, 3, aimInput.y * 5);

        currentState = GetPlayerState(movement, aimDirection);

        switch (currentState)
        {
            case PlayerState.OnlyMoving:

                Rotate(movement);

                aimTarget.position = transform.position + movePos;

                animator.SetBool("isMoving", true);
                animator.SetBool("isShooting", false);

                weapon.StopShooting();
                rb.position += movement * moveSpeed * Time.fixedDeltaTime;

                break;

            case PlayerState.OnlyAiming:

                Rotate(aimDirection);

                aimTarget.position = transform.position + targetPos;

                animator.SetBool("isMoving", false);
                animator.SetBool("isShooting", true);

                weapon.Shoot();
                break;

            case PlayerState.MovingAndAiming:

                Rotate(aimDirection);

                aimTarget.position = transform.position + targetPos;

                animator.SetBool("isMoving", true);
                animator.SetBool("isShooting", true);   //Animation conditions

                float normalizedAngle = Vector3.Angle(movement, aimDirection) / 180;    //Calaculate the angle between movement and aim and normalize it from 0 to 1
                animator.SetFloat("angle", normalizedAngle);

                weapon.Shoot();
                rb.position += movement * moveSpeed * Time.fixedDeltaTime;
                break;

            case PlayerState.Idle:

                aimTarget.position = transform.position + new Vector3(transform.forward.x * 5, 3, transform.forward.z * 5);

                weapon.StopShooting();
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

            if (rotateSmooth)
                targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * rotateSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(targetRotation);
        }
    }
}
