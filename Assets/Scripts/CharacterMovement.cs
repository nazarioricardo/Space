﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CharacterMovement : MonoBehaviour {

	public float speed = 5f;
	public float turnSensitity = 5f;
	public float jumpHeight = 1.5f;
	public float dashDistance = 5f;

	public Vector3 drag;
	public GameObject playerCameraRig;
	public LayerMask ground;
	public Transform groundChecker;
	public float mouseSensitivity = 100.0f;
	private float modAngle = 360.0f;

	private float rotationY = 0.0f; // rotation around the up/y axis
	private float rotationX = 0.0f; // rotation around the right/x axis
	private CharacterController controller;
	private Vector3 velocity;
	private float gravity = -9.81f;
	private float groundDistance;
	private bool isGrounded = false;

	void Start () {
		controller = GetComponent<CharacterController>();
        groundDistance = transform.lossyScale.y / 2;
        groundChecker = transform.GetChild(0);
		InitializeRotation ();
	}
	
	void Update () {
		MovePlayer ();
		MouseLook ();
        Jump();
	}

	void FixedUpdate () {
	}

	// Custom Methods

	void InitializeRotation () {
		Vector3 rotation = transform.localRotation.eulerAngles;
		rotationY = rotation.y;
		rotationX = rotation.x;
		SetCursorToCenter ();
	}

    void MovePlayer () {
        Debug.Log(controller.isGrounded);
		//isGrounded = Physics.CheckSphere (groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);
        //if (!isGrounded)
            //Debug.Log("Not grounded");

//		if (gameObject.transform.parent != null && isGrounded)
//			Debug.Log ("Is in ship and grounded");

		Vector3 move = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		controller.Move (controller.transform.TransformDirection (move * Time.deltaTime * speed));


		controller.Move (controller.transform.TransformVector (velocity));
	}

    void Jump () {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = 0f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded) {
            Debug.Log("Jump Pressed");
            if (controller.isGrounded)
                velocity.y += Mathf.Sqrt(jumpHeight * gravity * -0.05f);
            //velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
    }

	void MouseLook () {
		float mouseX = Input.GetAxis("Right X");
		float mouseY = -Input.GetAxis("Right Y");
        rotationX += mouseY * mouseSensitivity * Time.deltaTime;
        rotationY += mouseX * mouseSensitivity * Time.deltaTime;
        rotationX = rotationX % modAngle;
        rotationY = rotationY % modAngle;

		Quaternion localRotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
		transform.localRotation = localRotation;

		// TODO: Affect Camera Y axis
	}

	void SetCursorToCenter() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.lockState = CursorLockMode.None;
	}
}
