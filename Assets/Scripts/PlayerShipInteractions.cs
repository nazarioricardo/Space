using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerShipInteractions : MonoBehaviour {

	public Vector3 defaultCameraRigPosition;
	public Vector3 defaultCameraPosition;
	private GameObject cameraRig;

	private CharacterMovement characterMovement;
	private CharacterController characterController;
	private PilotShip pilotController;
	private GameObject ship;
	private Collider activeTrigger;
	private Collider gateTrigger;
	private Collider pilotTrigger;

	private bool inGateTrigger = false;
	private bool inCockpitTrigger = false;
	private bool inShip = false;
	private bool isPilot = false;

	// Use this for initialization
	void Start () {
		characterMovement = gameObject.GetComponent<CharacterMovement> ();
		characterController = gameObject.GetComponent<CharacterController> ();
		cameraRig = gameObject.transform.Find ("MultipurposeCameraRig").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		HandlePlayerInputs ();
	}
		
	void OnTriggerEnter (Collider trigger) {
		Debug.Log ("Entered Trigger " + trigger.tag);
		activeTrigger = trigger;
	
		// TODO: Change GateTrigger to a generic UseTrigger
		if (trigger.tag == "GateTrigger") {
			inGateTrigger = true;
			gateTrigger = trigger;
		}

		if (trigger.tag == "CockpitTrigger") {
			inCockpitTrigger = true;
			pilotTrigger = trigger;
		}

		if (trigger.tag == "EntryTrigger") {
			EnterShip (trigger);
		}
	}

	void OnTriggerExit (Collider trigger) {
		Debug.Log ("Left Trigger " + trigger.tag);
		if (trigger.tag == "GateTrigger") {
			Debug.Log (trigger.tag);
			inGateTrigger = false;
			gateTrigger = null;
		}	

		if (trigger.tag == "CockpitTrigger") {
			inCockpitTrigger = false;
			pilotTrigger = null;
		}

		if (trigger.tag == "EntryTrigger") {
			ExitShip ();
		}
	}

	void HandlePlayerInputs () {
		if (Input.GetButtonDown ("Use")) {
			if (gateTrigger) {
				gateTrigger.SendMessageUpwards ("OpenClose");
			}

			if (pilotTrigger) {
				TogglePilot (pilotTrigger);
			}
		}
	}

	void EnterShip (Collider trigger) {
		ship = trigger.gameObject.transform.root.gameObject;
		gameObject.transform.SetParent (ship.transform);
		pilotController = ship.GetComponent<PilotShip> ();
		inShip = true;
	}

	void ExitShip () {
		gameObject.transform.SetParent (null);
		inShip = false;
		ship = null;
		pilotController = null;
	}

	void TogglePilot (Collider trigger) {
		if (!isPilot)
			StartPilot (trigger);
		else
			StopPilot ();

		isPilot = !isPilot;
	}

	void StartPilot (Collider trigger) {
		if (!inShip)
			EnterShip (trigger);

		characterMovement.enabled = false;
		characterController.enabled = false;
		pilotController.SetCameraRig (cameraRig);
		pilotController.SetPilot (gameObject);
		pilotController.enabled = true;
	}

	void StopPilot () {
		pilotController.RemovePilot ();
        pilotController.RemoveCameraRig ();
		pilotController.enabled = false;
		characterMovement.enabled = true;
		characterController.enabled = true;
		SetThirdPersonCamera ();

		// Temp solution
		pilotTrigger = null;
		// Problem: On exit pilot, TriggerExit is not registered, and I can board the ship from pressing 'u' anywhere on the map
		// Potential solutions: 
		// 1. Examine my current use of triggers
		// 		-Check if I actually need to store the trigger as a private property
		// 2. Learn Ray Casting


		if (!pilotController.isWalkable)
			ExitShip ();
	}

	void SetThirdPersonCamera () {
        Debug.Log("Camera Rig: " + cameraRig);
  
		ResetAutoCam (cameraRig.GetComponent<AutoCam> ());
		cameraRig.transform.SetParent (gameObject.transform);
		cameraRig.transform.localScale = new Vector3 (1, 1, 1);
		cameraRig.transform.localPosition = defaultCameraRigPosition;
		GameObject pivot = cameraRig.transform.GetChild(0).gameObject;
		pivot.transform.localPosition = new Vector3 (0, 0, 0);
		GameObject camera = pivot.transform.GetChild (0).gameObject;
		camera.transform.localPosition = new Vector3 (0, 0, 0);
		camera.transform.localEulerAngles = new Vector3 (5, 0, 0);
	}

	void ResetAutoCam(AutoCam autoCam) {
		autoCam.enabled = false;
		autoCam.SetTarget (gameObject.transform);
	}
}
