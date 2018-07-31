using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerShipInteractions : MonoBehaviour
{

    public Vector3 defaultCameraPosition;
    private InputManager InputManager;

    private GameObject cam;
    private CharacterMovement characterMovement;
    private CharacterController characterController;
    private PilotController pilotController;
    private GameObject ship;
    private Collider activeTrigger;
    private Collider gateTrigger;
    private Collider pilotTrigger;
    private Animator animator;

    private bool isPilot = false;

    // Use this for initialization
    void Start()
    {
        InputManager = GetComponent<InputManager>();
        characterMovement = gameObject.GetComponent<CharacterMovement>();
        characterController = gameObject.GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        cam = gameObject.transform.Find("MainCamera").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerInputs();
    }

    void OnTriggerEnter(Collider trigger)
    {
        activeTrigger = trigger;

        // TODO: Change GateTrigger to a generic UseTrigger
        if (trigger.tag == "GateTrigger")
            gateTrigger = trigger;

        if (trigger.tag == "CockpitTrigger")
            pilotTrigger = trigger;

        if (trigger.tag == "EntryTrigger")
            EnterShip(trigger);
    }

    void OnTriggerExit(Collider trigger)
    {
        activeTrigger = null;
            
        if (trigger.tag == "GateTrigger")
            gateTrigger = null;

        if (trigger.tag == "CockpitTrigger")
            pilotTrigger = null;

        if (trigger.tag == "EntryTrigger")
            ExitShip();
    }

    void HandlePlayerInputs()
    {
        if (InputManager.ActionButtonDown())
        {
            if (gateTrigger)
                gateTrigger.SendMessageUpwards("OpenClose");

            if (pilotTrigger)
                TogglePilot(pilotTrigger);
        }
    }

    void EnterShip(Collider trigger)
    {
        ship = trigger.gameObject.transform.root.gameObject;
        pilotController = ship.GetComponent<PilotController>();
    }

    void ExitShip()
    {
        gameObject.transform.SetParent(null);
        ship = null;
        pilotController = null;
    }

    void TogglePilot(Collider trigger)
    {
        if (!isPilot)
            StartPilot(trigger);
        else
            StopPilot();

        isPilot = !isPilot;
    }

    void StartPilot(Collider trigger)
    {
        EnterShip(trigger);

        characterMovement.enabled = false;
        characterController.enabled = false;
        pilotController.SetPilot(gameObject);
        pilotController.enabled = true;
        animator.SetInteger("isSitting", 1);
    }

    void StopPilot()
    {
        pilotController.RemovePilot();
        pilotController.enabled = false;
        characterMovement.enabled = true;
        characterController.enabled = true;
        SetThirdPersonCamera();

        // Temp solution
        pilotTrigger = null;
        // Problem: On exit pilot, TriggerExit is not registered, and I can board the ship from pressing 'u' anywhere on the map
        // Potential solutions: 
        // 1. Examine my current use of triggers
        // 		-Check if I actually need to store the trigger as a private property
        // 2. Learn Ray Casting

        ExitShip();
        animator.SetInteger("isSitting", 0);

    }

    void SetThirdPersonCamera()
    {
        Debug.Log("Camera Rig: " + cam);


        // Consider current differences between this file and pilotcamcontroller regarding cam and rig
        cam.transform.SetParent(gameObject.transform);
        cam.transform.localScale = new Vector3(1, 1, 1);
        cam.transform.localPosition = defaultCameraPosition;
        cam.transform.localEulerAngles = Vector3.zero;
    }
}
