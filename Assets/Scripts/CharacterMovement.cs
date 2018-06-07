using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class CharacterMovement : MonoBehaviour {

	public float speed = 5f;
	public float turnSensitity = 5f;
	public float jumpHeight = 10f;
	public float dashDistance = 5f;

	public Vector3 drag;
	public GameObject playerCameraRig;
	public LayerMask ground;
	public Transform groundChecker;
	public float mouseSensitivity = 100.0f;
	private float clampAngle = 80.0f;

	private float rotationY = 0.0f; // rotation around the up/y axis
	private float rotationX = 0.0f; // rotation around the right/x axis
	private CharacterController controller;
	private Vector3 velocity;
	private float gravity = 9.81f;
	private float groundDistance;
	private bool isGrounded = false;

	void Start () {
		controller = GetComponent<CharacterController>();
		groundDistance = transform.lossyScale.y / 2;
		InitializeRotation ();
	}
	
	void Update () {
		MovePlayer ();
		MouseLook ();
	}

	// Custom Methods

	void InitializeRotation () {
		Vector3 rotation = transform.localRotation.eulerAngles;
		rotationY = rotation.y;
		rotationX = rotation.x;
		SetCursorToCenter ();
	}

	void MovePlayer () {
		isGrounded = Physics.CheckSphere (groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);
		if (isGrounded && velocity.y < 0)
			velocity.y = 0f;

//		if (gameObject.transform.parent != null && isGrounded)
//			Debug.Log ("Is in ship and grounded");

		Vector3 move = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		controller.Move (controller.transform.TransformDirection (move * Time.deltaTime * speed));

		if (Input.GetButtonDown ("Jump") && isGrounded) {
			Debug.Log ("Jump Pressed");
			velocity.y += Mathf.Sqrt (jumpHeight * gravity * Time.deltaTime * 2);
		}

		velocity.y -= gravity * Time.deltaTime;
		controller.Move (controller.transform.TransformVector (velocity));
	}

	void MouseLook () {
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		rotationY += mouseX * mouseSensitivity * Time.deltaTime;
		rotationX += mouseY * mouseSensitivity * Time.deltaTime * turnSensitity;

		rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);

		Quaternion localRotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
		transform.localRotation = localRotation;

		// TODO: Affect Camera Y axis
	}

	void SetCursorToCenter() {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.lockState = CursorLockMode.None;
	}
}
