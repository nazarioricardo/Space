using System.Collections;
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
	private float rotationY = 0.0f; // rotation around the up/y axis
	private float rotationX = 0.0f; // rotation around the right/x axis
	private float rotationZ = 0.0f;

	// Movement Modifiers
	public float sensitivityX = 60.0f;
	public float sensitivityY = 60.0f;
	public float sensitivityZ = 50.0f;
	public float acceleration = 50f;
	public float brakeModifier = 20f;

	public float cruiseSpeed = 60f;
	public float attackSpeed = 80f;

	private Rigidbody rb;

	// Player Management Props
	private GameObject pilot;

	// Ship Movement
	private enum ThrustMode { Off, Free, Hover, Cruise, Reverse }
	private ThrustMode activeThrustMode;
	private Vector3 movement;
	private float thrust = 0.0f;
    private float currentSpeed = 0.0f;

		
	void Start () {
		Debug.Log ("Starting pilot controller");
		rb = GetComponent<Rigidbody> ();		
		activeThrustMode = ThrustMode.Off;
		modeLabel.text = "Off";
	}

	void Update() {
        ManageThrustMode();
	}
	
	void FixedUpdate() {
        if (activeThrustMode == ThrustMode.Off)
            return;
        
        Hover();
        Throttle();
        Roll();
        PitchYaw();
        Elevate();

        currentSpeed = rb.velocity.magnitude;

        speedLabel.text = currentSpeed.ToString();
        thrustLabel.text = thrust.ToString();
	}

    void ManageThrustMode() {
        if (activeThrustMode == ThrustMode.Hover) {
            if (InputManager.ThrottleDownButtonUp())
                SetCruise();

            if (InputManager.ThrottleUpButtonUp())
                SetReverse();
        }

        if (activeThrustMode == ThrustMode.Off && InputManager.ThrottleUpButtonDown() ||
            activeThrustMode == ThrustMode.Free && InputManager.ThrottleUpButtonDown()) {
            SetCruise();
        }

        if (activeThrustMode == ThrustMode.Off && InputManager.ThrottleDownButtonDown() ||
            activeThrustMode == ThrustMode.Free && InputManager.ThrottleDownButtonDown()) {
            SetReverse();
        }

        if (activeThrustMode == ThrustMode.Cruise) {
            if (InputManager.ThrottleUpButtonUp())
                SetFree();

            if (InputManager.ThrottleDownButtonDown())
                SetHover();
        }

        if (activeThrustMode == ThrustMode.Reverse) {
            if (InputManager.ThrottleDownButtonUp())
                SetFree();

            if (InputManager.ThrottleUpButtonDown())
                SetHover();
        }

        if (InputManager.ThrottleUpButtonDown() && InputManager.ThrottleDownButtonDown()) {
            SetHover();
        }
    }

    void SetFree() {
        activeThrustMode = ThrustMode.Free;
        modeLabel.text = "Free";
    }

    void SetHover() {
        activeThrustMode = ThrustMode.Hover;
        modeLabel.text = "Hover";
        StablizeFromTumble();
    }

    void SetCruise() {
        activeThrustMode = ThrustMode.Cruise;
        modeLabel.text = "Cruise";
    }

    void SetReverse() {
        activeThrustMode = ThrustMode.Reverse;
        modeLabel.text = "Reverse";
    }

    void Throttle() {
        if (activeThrustMode == ThrustMode.Hover || activeThrustMode == ThrustMode.Free)
            return;

        if (activeThrustMode == ThrustMode.Cruise) {
            thrust = 200.0f;
        }
            

        if (activeThrustMode == ThrustMode.Reverse) {
            thrust = -100.0f;
        }

        transform.position += transform.forward * Time.deltaTime * thrust;
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
        if (activeThrustMode == ThrustMode.Off)
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

    void Roll() {
        if (activeThrustMode == ThrustMode.Hover)
            return;
        
        float zAxis = InputManager.LeftHorizontalAxis();
        rotationZ = -zAxis * sensitivityZ * Time.deltaTime;
        rb.transform.Rotate(0.0f, 0.0f, rotationZ);
    }

    void Hover() {
        if (activeThrustMode != ThrustMode.Hover)
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

    void StablizeFromTumble() {
        rb.angularVelocity = Vector3.zero;
    }

	public void SetPilot (GameObject player) {
		pilot = player;
		pilot.transform.localPosition = pilotPosition.transform.localPosition;
		pilot.transform.localEulerAngles = new Vector3 (0, 0, 0);
        SetCameraPosition();
    }

    public void RemovePilot () {
		pilot.transform.localPosition = pilotExitPosition.transform.localPosition;
		pilot.transform.localEulerAngles = new Vector3 (0, 0, 0);
		pilot = null;
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
