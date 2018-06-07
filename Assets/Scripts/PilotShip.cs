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
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;

	// Movement Modifiers
	public float sensitivityX = 0.1f;
	public float sensitivityY = 0.1f;
	public float sensitivityZ = 1f;
	public float antiDriftModifier = 1.5f;
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
	private enum FlightMode { Off, Hover, Approach, Cruise, Attack }
	private FlightMode activeMode;
	private Vector3 movement;
	private float thrust = 0.0f;
	private float maxSpeed = 0.0f;
	private float minSpeed = 0.0f;
	private float brakeMin = 0.0f;

	// Animations
	private Animation animations;
	private bool isGearOut = true;
		
	void Start () {
		Debug.Log ("Starting pilot controller");
		rb = GetComponent<Rigidbody> ();		
		animations = GetComponent<Animation> ();
		activeMode = FlightMode.Off;
		modeLabel.text = "Off";
		SetCameraPosition ();
		Debug.Log ("Started Pilot Controller");
	}

	void Update() {
		HandleShipControls ();
	}
	
	void FixedUpdate() {
		MoveShip ();	
	}

	void OnCollisionEnter (Collision collision) {
//		Debug.Log ("Collision!" + collision.impulse);
	}

	void MoveShip () {
		if (!isPiloting || activeMode == FlightMode.Off)
			return;

		moveVertical = Input.GetAxis ("Vertical");
		moveHorizontal = Input.GetAxis ("Horizontal");
		thrust = Mathf.Clamp (thrust + moveVertical, minSpeed, maxSpeed);
		thrustLabel.text = thrust.ToString ();
		speedLabel.text = rb.velocity.magnitude.ToString ();
		// TODO: Learn how to stabilize forces after collision

		if (Input.GetKeyDown ("f"))
			Brake ();

		if (activeMode == FlightMode.Hover) {
			Hover (GetMouseRotation ());
		} else {
			Fly (GetMouseRotation ());
		}

	}



	void Hover (Vector3 mouseRotation) {
		float landingThrust = Mathf.Clamp (thrust, minSpeed, maxSpeed);
		movement = new Vector3 (0.0f, -mouseRotation.x * 5f, landingThrust);
		rb.transform.Rotate (0.0f, mouseRotation.y, -moveHorizontal * sensitivityZ);
		rb.AddRelativeForce (movement * acceleration);
	}

	void Fly (Vector3 mouseRotation) {
		float activeThrust = Mathf.Clamp (thrust, minSpeed, maxSpeed);
		movement = new Vector3 (mouseRotation.y * activeThrust * antiDriftModifier, (-mouseRotation.x * activeThrust * antiDriftModifier), activeThrust);
		rb.transform.Rotate (mouseRotation.x, mouseRotation.y, -moveHorizontal * (sensitivityZ));
		rb.AddRelativeForce (movement * acceleration);
	}

	void Brake () {
		thrust = Mathf.Clamp (thrust - brakeModifier, brakeMin, maxSpeed);
	}

	Vector3 GetMouseRotation () {
		Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
		Vector3 mouseToScreen = mainCamera.ScreenToViewportPoint(mouse);
		float yToCenter = mouseToScreen.y - 0.5f;
		float xToCenter = mouseToScreen.x - 0.5f;
		Vector3 rotation = new Vector3 (yToCenter, xToCenter, 0.0f);
		return rotation;
	}

	void HandleShipControls () {
		if (!isPiloting)
			return;

		// Shift Down
		if (Input.GetKeyDown ("q")) {
			ShiftDown ();
		}

		// Shift Up
		if (Input.GetKeyDown ("e")) {
			ShiftUp ();
		}

		if (Input.GetKeyDown ("g")) {
			if (isGearOut) {
				animations.Play ("Vanguard Gear In");
				isGearOut = false;
			} else {
				animations.Play ("Vanguard Gear Out");
				isGearOut = true;
			}
		}

		if (Input.GetKeyDown ("o")) {
			if (isGearOut) {
				gameObject.SendMessage ("OpenClose");
			} else {
				gameObject.SendMessage ("OpenClose");
			}
		}

	}

	void ShiftDown () {
		switch (activeMode) {
		case FlightMode.Attack:
			activeMode = FlightMode.Cruise;
			modeLabel.text = "Cruise";
			maxSpeed = cruiseSpeed;
			minSpeed = 20f;
			brakeMin = minSpeed;
			break;
		case FlightMode.Cruise:
			activeMode = FlightMode.Approach;
			modeLabel.text = "Approach";
			maxSpeed = 10f;
			minSpeed = -0.5f;			
			brakeMin = 0.0f;
			break;
		case FlightMode.Approach:
			activeMode = FlightMode.Hover;
			modeLabel.text = "Hover";
			maxSpeed = 5f;
			minSpeed = -0.5f;
			brakeMin = 0.0f;
			break;
		case FlightMode.Hover:
			activeMode = FlightMode.Off;
			rb.useGravity = true;
			modeLabel.text = "Off";
			maxSpeed = 0.0f;
			minSpeed = 0.0f;
			brakeMin = 0.0f;
			break;
		default:
			// Already at lowest FlightMode, so do Nothing!
			break;
		}
	}

	void ShiftUp () {
		switch (activeMode) {
		case FlightMode.Off:
			activeMode = FlightMode.Hover;
			rb.useGravity = false;
			modeLabel.text = "Hover";
			maxSpeed = 5f;
			minSpeed = -0.5f;
			brakeMin = 0.0f;
			break;
		case FlightMode.Hover:
			activeMode = FlightMode.Approach;
			modeLabel.text = "Approach";
			maxSpeed = 10f;
			minSpeed = -0.5f;			
			brakeMin = 0.0f;
			break;
		case FlightMode.Approach:
			activeMode = FlightMode.Cruise;
			modeLabel.text = "Cruise";
			maxSpeed = cruiseSpeed;
			minSpeed = 20f;
			brakeMin = minSpeed;
			break;
		case FlightMode.Cruise:
			activeMode = FlightMode.Attack;
			modeLabel.text = "Attack";
			maxSpeed = attackSpeed;
			minSpeed = 20f;
			brakeMin = minSpeed;
			break;
		default:
			// Already at highest FlightMode, so do Nothing!
			break;
		}
	}

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
	}

	public void RemovePilot () {
		pilot.transform.localPosition = pilotExitPosition.transform.localPosition;
		pilot.transform.localEulerAngles = new Vector3 (0, 0, 0);
		pilot = null;
		isPiloting = false;
	}

	public void SetCameraRig (GameObject rig) {
		cameraRig = rig;
		cameraPivot = cameraRig.transform.GetChild(0).gameObject;
		mainCamera = cameraPivot.transform.GetChild (0).gameObject.GetComponent<Camera> ();
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
