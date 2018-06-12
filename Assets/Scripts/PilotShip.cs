﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

public class PilotShip : MonoBehaviour {

	// TODO: This is the next class that should be refactored.

	// UI
	public Canvas canvas;
	public Text modeLabel;
	public Text thrustLabel;
	public Text speedLabel;

	// Camera
	public Vector3 defaultCameraRigPosition;
	public Vector3 defaultCameraPivotPosition;
	public Vector3 defaultCameraPosition;
	private GameObject cameraRig;
	private GameObject cameraPivot;
	private Camera mainCamera;

	// Player Interactions
	public Component pilotPosition;
	public Component pilotExitPosition;
	public bool isWalkable;

	// Axes
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float mouseSensitivity = 25.0f;
	public float turnSensitity = 5f;
	private float rotationY = 0.0f; // rotation around the up/y axis
	private float rotationX = 0.0f; // rotation around the right/x axis
	private float rotationZ = 0.0f;
	private float modAngle = 360.0f;


	// Movement Modifiers
	public float sensitivityX = 40.0f;
	public float sensitivityY = 40.0f;
	public float sensitivityZ = 50.0f;
	public float stabilizer = 1.2f;
	public float acceleration = 50f;
	public float brakeModifier = 20f;

	public float cruiseSpeed = 60f;
	public float attackSpeed = 80f;

	private Rigidbody rb;

	// Player Management Props
	private GameObject pilot;
	private bool isPiloting = false;

	// Ship Movement
	private float moveVertical;
	private float moveHorizontal;
	private enum FlightMode { Off, Hover, Cruise, Reverse }
	private FlightMode activeMode;
	private Vector3 movement;
    private bool thrustersEngaged = false;
	private float thrust = 0.0f;
    private float currentSpeed = 0.0f;
	private float maxSpeed = 60.0f;
	private float minSpeed = -1.0f;
	private float brakeMin = 0.0f;

	// Animations
	private Animation animations;
	private bool isGearOut = true;
		
	void Start () {
		Debug.Log ("Starting pilot controller");
		rb = GetComponent<Rigidbody> ();		
		animations = GetComponent<Animation> ();
		//activeMode = FlightMode.Off;
		//modeLabel.text = "Off";
		Debug.Log ("Started Pilot Controller");
	}

	void Update() {
		HandleShipControls ();
        ManageFlightMode();
	}
	
	void FixedUpdate() {
        //MoveShip ();
        if (activeMode == FlightMode.Off)
            return;
        
        Hover();
        Throttle();
        PitchYaw();
        Elevate();

        currentSpeed = rb.velocity.magnitude;

        speedLabel.text = currentSpeed.ToString();
        thrustLabel.text = thrust.ToString();
	}

	void OnCollisionEnter (Collision collision) {
//		Debug.Log ("Collision!" + collision.impulse);
	}

    void ManageFlightMode() {
        if (InputManager.ThrottleUpButtonDown()) {
            activeMode = FlightMode.Cruise;
            Debug.Log("Setting Flight Mode Cruise");
            modeLabel.text = "Cruise";
        }

        if (InputManager.ThrottleDownButtonDown()) {
            activeMode = FlightMode.Reverse;
            Debug.Log("Setting Flight Mode Reverse");
            modeLabel.text = "Reverse";
        }

        if (InputManager.ThrottleUpButtonDown() && InputManager.ThrottleDownButtonDown()) {
            activeMode = FlightMode.Hover;
            Debug.Log("Setting Flight Mode Hover");
            modeLabel.text = "Hover";
        }

        //if (!InputManager.ThrottleUpButton() || !InputManager.ThrottleDownButton()) {
        //    activeMode = FlightMode.Off;
        //    Debug.Log("Setting Flight Mode Off");
        //    modeLabel.text = "Off";
        //}
    }

    //void Throttle() {
    //    if (activeMode != FlightMode.Cruise)
    //        return;

    //    if (InputManager.ThrottleUpButtonDown()) {
    //        thrust = Mathf.Clamp(thrust + 1, minSpeed, maxSpeed);
    //    }

    //    if (InputManager.ThrottleDownButtonDown()) {
    //        thrust = Mathf.Clamp(thrust - 1, minSpeed, maxSpeed);
    //    }

        //float activeThrust = Mathf.Clamp(thrust, minSpeed, maxSpeed);
        //float pitchStablize = rotationY * activeThrust * stabilizer;
        //float yawStabilize = rotationX * activeThrust * stabilizer;
        //movement = new Vector3(yawStabilize, -pitchStablize, activeThrust);
        //rb.AddRelativeForce(movement * acceleration);
        //Debug.Log("Active Thrust " + activeThrust);

    //    Roll();
    //}

    void Throttle() {
        if (activeMode != FlightMode.Cruise)
            return;

        if (InputManager.ThrottleUpButtonDown()) {
            thrustersEngaged = true;
            thrust = 1200.0f;
        }
            

        if (InputManager.ThrottleDownButtonDown()) {
            thrustersEngaged = true;
            thrust = -400.0f;
        }

        if (InputManager.ThrottleUpButtonUp() || InputManager.ThrottleDownButtonUp()) {
            thrustersEngaged = false;
            thrust = 0.0f;
        }

        rb.AddRelativeForce(0.0f, 0.0f, thrust);
        float activeThrust = Mathf.Clamp(thrust, minSpeed, maxSpeed);
        float pitchStablize = rotationY * 10 * stabilizer;
        float yawStabilize = rotationX * 10 * stabilizer;
        movement = new Vector3(yawStabilize, -pitchStablize, 0.0f);
        rb.AddRelativeForce(movement * acceleration);

        Roll();
    }

    void Elevate() {
        float zAxis = InputManager.LeftVerticalAxis();

        if (zAxis > 0) {
            rb.AddRelativeForce(0.0f, 100.0f, 0.0f);
        }

        if (zAxis < 0) {
            rb.AddRelativeForce(0.0f, -100.0f, 0.0f);
        }
    }

    void PitchYaw() {
        if (activeMode == FlightMode.Off)
            return;

        Pitch();
        Yaw();
    }

    void Pitch() {
        float yAxis = -InputManager.RightVerticalAxis();
        rotationY = yAxis * sensitivityY * Time.deltaTime;
        Vector3 localRotation = new Vector3(rotationY, 0.0f, 0.0f);
        rb.transform.Rotate(localRotation);
    }

    void Yaw() {
        float xAxis = InputManager.RightHorizontalAxis();
        rotationX = xAxis * sensitivityX * Time.deltaTime;
        Vector3 localRotation = new Vector3(0.0f, rotationX, 0.0f);
        rb.transform.Rotate(localRotation);
    }

    void Roll()
    {
        float zAxis = InputManager.LeftHorizontalAxis();
        rotationZ = -zAxis * sensitivityZ * Time.deltaTime;
        rb.transform.Rotate(0.0f, 0.0f, rotationZ);
    }

    void Hover() {
        if (activeMode != FlightMode.Hover)
            return;

        Strafe();
    }

    void Strafe() {
        float xAxis = InputManager.LeftHorizontalAxis();
        if (xAxis > 0) {
            rb.AddRelativeForce(100.0f, 0.0f, 0.0f);
        }

        if (xAxis < 0) {
            rb.AddRelativeForce(-100.0f, 0.0f, 0.0f);
        }
    }

//	void MoveShip () {
//		if (!isPiloting || activeMode == FlightMode.Off)
//			return;

//        moveVertical = InputManager.LeftVerticalAxis();
//        moveHorizontal = InputManager.LeftHorizontalAxis();
//		thrust = Mathf.Clamp (thrust + moveVertical, minSpeed, maxSpeed);
//		thrustLabel.text = thrust.ToString ();
//		speedLabel.text = rb.velocity.magnitude.ToString ();
//		// TODO: Learn how to stabilize forces after collision

//        if (InputManager.BrakeButton())
//			Brake ();

//		if (activeMode == FlightMode.Hover) {
//			Hover (GetMouseRotation ());
//		} else {
////			OriginalFly (GetMouseRotation ());
////			RogueFly ();
	//		NewFly ();
	//	}

	//}

//	void Hover (Vector3 mouseRotation) {
//		float landingThrust = Mathf.Clamp (thrust, minSpeed, maxSpeed);
//		movement = new Vector3 (0.0f, -mouseRotation.x * 5f, landingThrust);
//		rb.transform.Rotate (0.0f, mouseRotation.y, -moveHorizontal * sensitivityZ);
//		rb.AddRelativeForce (movement * acceleration);
//	}

//	void OriginalFly (Vector3 mouseRotation) {
//		float activeThrust = Mathf.Clamp (thrust, minSpeed, maxSpeed);
//		movement = new Vector3 (mouseRotation.y * activeThrust * stabilizer, (-mouseRotation.x * activeThrust * stabilizer), activeThrust);
//		rb.transform.Rotate (mouseRotation.x, mouseRotation.y, -moveHorizontal * sensitivityZ);
//		rb.AddRelativeForce (movement * acceleration);
//	}

//	void RogueFly () {
//		RogueMouseLook ();
////		RollInput ();
	//	float activeThrust = Mathf.Clamp (thrust, minSpeed, maxSpeed);
	//	float yawStabilize = rotationY * activeThrust * stabilizer;
	//	float pitchStabilize = rotationX * activeThrust * stabilizer;
	//	movement = new Vector3 (yawStabilize, pitchStabilize, activeThrust);
	//	movement = new Vector3 (0.0f, 0.0f, activeThrust);
	//	rb.AddRelativeForce (movement * acceleration);

	//}

	//void NewFly () {
	//	NewMouseLook ();
	//	RollInput ();
		//float activeThrust = Mathf.Clamp (thrust, minSpeed, maxSpeed);
		//float yawStabilize = rotationY * activeThrust * stabilizer;
		//float pitchStabilize = rotationX * activeThrust * stabilizer;
		//movement = new Vector3 (yawStabilize, pitchStabilize, activeThrust);
		//rb.AddRelativeForce (movement * acceleration);
	//}

	void Brake () {
		thrust = Mathf.Clamp (thrust - brakeModifier, brakeMin, maxSpeed);
	}

	void RollInput () {
		rotationZ = -moveHorizontal * mouseSensitivity * Time.deltaTime;
		rb.transform.Rotate (0.0f, 0.0f, rotationZ);
	}

	void RollWithYaw () {
		rb.transform.Rotate (0.0f, 0.0f, -rotationY * 0.5f);
	}

	//void RogueMouseLook () {
	//	float mouseX = Input.GetAxis("Right X");
	//	float mouseY = -Input.GetAxis("Right Y");
	//	rotationX += mouseY * mouseSensitivity * Time.deltaTime;
	//	rotationY += mouseX * mouseSensitivity * Time.deltaTime;
	//	rotationX = rotationX % modAngle;
	//	rotationY = rotationY % modAngle;
	//	Quaternion localRotation = Quaternion.Euler(-rotationX, rotationY, 0.0f);
	//	transform.localRotation = localRotation;
	//}

	void NewMouseLook () {
        float mouseX = InputManager.RightHorizontalAxis();
        float mouseY = -InputManager.RightVerticalAxis();
		rotationX = mouseY * mouseSensitivity * Time.deltaTime;
		rotationY = mouseX * mouseSensitivity * Time.deltaTime;
		Vector3 localRotation = new Vector3 (-rotationX, rotationY, 0.0f);
		rb.transform.Rotate (localRotation);
	}

	//Vector3 GetMouseRotation () {
 //       Vector3 mouse = new Vector3(InputManager.RightHorizontalAxis(), InputManager.RightHorizontalAxis(), Input.mousePosition.z);
	//	Vector3 mouseToScreen = mainCamera.ScreenToViewportPoint(mouse);
	//	float yToCenter = mouseToScreen.y - 0.5f;
	//	float xToCenter = mouseToScreen.x - 0.5f;
	//	Vector3 rotation = new Vector3 (yToCenter, xToCenter, 0.0f);
	//	return rotation;
	//}

	void HandleShipControls () {
		if (!isPiloting)
			return;

		// Shift Down
  //      if (InputManager.ThrottleDownButton()) {
		//	ShiftDown ();
		//}

		// Shift Up
  //      if (InputManager.ThrottleUpButton()) {
		//	ShiftUp ();
		//}

		//if (Input.GetKeyDown ("g")) {
		//	if (isGearOut) {
		//		animations.Play ("Vanguard Gear In");
		//		isGearOut = false;
		//	} else {
		//		animations.Play ("Vanguard Gear Out");
		//		isGearOut = true;
		//	}
		//}

		//if (Input.GetKeyDown ("o")) {
		//	if (isGearOut) {
		//		gameObject.SendMessage ("OpenClose");
		//	} else {
		//		gameObject.SendMessage ("OpenClose");
		//	}
		//}

	}

	//void ShiftDown () {
	//	switch (activeMode) {
	//	case FlightMode.Attack:
	//		activeMode = FlightMode.Cruise;
	//		modeLabel.text = "Cruise";
	//		maxSpeed = cruiseSpeed;
	//		minSpeed = 20f;
	//		brakeMin = minSpeed;
	//		break;
	//	case FlightMode.Cruise:
	//		activeMode = FlightMode.Approach;
	//		modeLabel.text = "Approach";
	//		maxSpeed = 10f;
	//		minSpeed = -0.5f;			
	//		brakeMin = 0.0f;
	//		break;
	//	case FlightMode.Approach:
	//		activeMode = FlightMode.Hover;
	//		modeLabel.text = "Hover";
	//		maxSpeed = 5f;
	//		minSpeed = -0.5f;
	//		brakeMin = 0.0f;
	//		break;
	//	case FlightMode.Hover:
	//		activeMode = FlightMode.Off;
	//		rb.useGravity = true;
	//		modeLabel.text = "Off";
	//		maxSpeed = 0.0f;
	//		minSpeed = 0.0f;
	//		brakeMin = 0.0f;
	//		break;
	//	default:
	//		// Already at lowest FlightMode, so do Nothing!
	//		break;
	//	}
	//}

	//void ShiftUp () {
	//	switch (activeMode) {
	//	case FlightMode.Off:
	//		activeMode = FlightMode.Hover;
	//		rb.useGravity = false;
	//		modeLabel.text = "Hover";
	//		maxSpeed = 5f;
	//		minSpeed = -0.5f;
	//		brakeMin = 0.0f;
	//		break;
	//	case FlightMode.Hover:
	//		activeMode = FlightMode.Approach;
	//		modeLabel.text = "Approach";
	//		maxSpeed = 10f;
	//		minSpeed = -0.5f;			
	//		brakeMin = 0.0f;
	//		break;
	//	case FlightMode.Approach:
	//		activeMode = FlightMode.Cruise;
	//		modeLabel.text = "Cruise";
	//		maxSpeed = cruiseSpeed;
	//		minSpeed = 20f;
	//		brakeMin = minSpeed;
	//		break;
	//	case FlightMode.Cruise:
	//		activeMode = FlightMode.Attack;
	//		modeLabel.text = "Attack";
	//		maxSpeed = attackSpeed;
	//		minSpeed = 20f;
	//		brakeMin = minSpeed;
	//		break;
	//	default:
	//		// Already at highest FlightMode, so do Nothing!
	//		break;
	//	}
	//}

	// Not In Use
	// TODO: Define reason for function, and make it work based on that reason.
	void RandomTorque() {
		rb.AddRelativeTorque (Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f));
	}

	public void SetPilot (GameObject player) {
		pilot = player;
		isPiloting = true;
		pilot.transform.localPosition = pilotPosition.transform.localPosition;
		pilot.transform.localEulerAngles = new Vector3 (0, 0, 0);
        SetCameraPosition();
    }

    public void RemovePilot () {
		pilot.transform.localPosition = pilotExitPosition.transform.localPosition;
		pilot.transform.localEulerAngles = new Vector3 (0, 0, 0);
		pilot = null;
		isPiloting = false;
	}

	public void SetCameraRig (GameObject rig) {
        Debug.Log("Setting rig: " + rig);
		cameraRig = rig;
		cameraPivot = cameraRig.transform.GetChild(0).gameObject;
		mainCamera = cameraPivot.transform.GetChild (0).gameObject.GetComponent<Camera> ();
	}

    public void RemoveCameraRig () {
        cameraRig = null;
        cameraPivot = null;
        mainCamera = null;
    }

	void SetCameraPosition () {
		// Set Auto Cam Settings
		SetAutoCam (cameraRig.GetComponent<AutoCam> ());

		cameraRig.transform.SetParent (gameObject.transform);
		cameraRig.transform.localScale = new Vector3 (1, 1, 1);
		cameraRig.transform.localPosition = defaultCameraRigPosition;

		cameraPivot.transform.localPosition = defaultCameraPivotPosition;

		mainCamera.transform.localPosition = defaultCameraPosition;
		mainCamera.transform.localEulerAngles = new Vector3 (5, 0, 0);
	}

	void SetAutoCam(AutoCam autoCam) {
		Debug.Log ("Initializing Auto Cam");
		autoCam.SetTarget (gameObject.transform);
		autoCam.enabled = true;
		Debug.Log ("AutoCam target " + autoCam.Target);
	}
}
