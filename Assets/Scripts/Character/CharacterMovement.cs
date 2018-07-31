using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CharacterMovement : MonoBehaviour
{

    public float speed = 5f;
    public float turnSensitity = 5f;
    public float jumpHeight = 1.5f;
    public float dashDistance = 5f;

    public Vector3 drag;
    public GameObject playerCameraRig;
    public float mouseSensitivity = 100.0f;
    private float modAngle = 360.0f;

    private InputManager InputManager;

    private float horizontalMovement;
    private float verticalMovement;
    private float rotationY = 0.0f; // rotation around the up/y axis
    private float rotationX = 0.0f; // rotation around the right/x axis
    private CharacterController controller;
    private Vector3 velocity;
    private float gravity = -9.81f;

    private Animator animator;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        InputManager = GetComponent<InputManager>();
        InitializeRotation();
    }

    void FixedUpdate()
    {
        MovePlayer();
        MouseLook();
        Jump();
    }

    // Custom Methods

    void InitializeRotation()
    {
        Vector3 rotation = transform.localRotation.eulerAngles;
        rotationY = rotation.y;
        rotationX = rotation.x;
        SetCursorToCenter();
    }

    void MovePlayer()
    {
        horizontalMovement = InputManager.LeftHorizontalAxis();
        verticalMovement = InputManager.LeftVerticalAxis();
        Vector3 move = new Vector3(horizontalMovement, 0, verticalMovement);
        controller.Move(controller.transform.TransformDirection(move * 0.3f));
        controller.Move(controller.transform.TransformVector(velocity));

        animator.SetFloat("Horizontal", horizontalMovement);
        animator.SetFloat("Vertical", verticalMovement);
    }

    void Jump()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = 0f;

        if (InputManager.JumpButtonDown() && controller.isGrounded)
        {
            if (controller.isGrounded)
                velocity.y += Mathf.Sqrt(jumpHeight * gravity * -0.05f);
            //velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
    }

    void MouseLook()
    {
        float mouseX = InputManager.RightHorizontalAxis();
        float mouseY = -InputManager.RightVerticalAxis();
        rotationX += mouseY * mouseSensitivity * Time.deltaTime;
        rotationY += mouseX * mouseSensitivity * Time.deltaTime;
        rotationX = rotationX % modAngle;
        rotationY = rotationY % modAngle;

        Quaternion localRotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
        transform.localRotation = localRotation;

        // TODO: Affect Camera Y axis
    }

    void SetCursorToCenter()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
    }
}
