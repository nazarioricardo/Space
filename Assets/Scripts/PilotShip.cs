﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

public class PilotShip : MonoBehaviour
{

    // TODO: This is the next class that should be refactored.

    // UI
    public Canvas canvas;
    public Text modeLabel;
    public Text thrustLabel;
    public Text speedLabel;

    // Camera
    private PilotCamController pilotCamController;

    // Player Interactions
    public Component pilotPosition;
    public Component pilotExitPosition;

    // Movement Modifiers
    public float yawSenstivity = 60.0f;
    public float pitchSensitivity = 60.0f;
    public float rollSpeed = 90;
    public float acceleration = 50f;

    public float maxThrust = 400.0f;
    public float minThrust = -100.0f;
    public float maxVerticalThrust = 100.0f;
    public float minVerticalThrust = -100.0f;
    public float maxHorizontalThrust = 100.0f;
    public float minHorizontalThrust = -100.0f;
    public int maxBank = 70;

    // Ship Parts
    private GameObject hull;
    private Rigidbody rb;

    // Player Management Props
    private GameObject pilot;

    // Ship Movement
    private enum ThrustMode { Off, Free, Hover, Cruise, Reverse }
    private ThrustMode activeThrustMode;

    private float thrust = 0.0f;
    private float verticalThrust = 0.0f;
    private float horizontalThrust = 0.0f;
    private float bank = 0.0f;
    private float currentSpeed = 0.0f;

    // Axes
    private float rotationY = 0.0f; // rotation around the up/y axis
    private float rotationX = 0.0f; // rotation around the right/x axis
    private float rotationZ = 0.0f;

    void Start()
    {
        Debug.Log("Starting pilot controller");
        hull = transform.Find("Hull").gameObject;
        rb = hull.GetComponent<Rigidbody>();
        pilotCamController = GetComponent<PilotCamController>();
        activeThrustMode = ThrustMode.Off;
        modeLabel.text = "Off";
    }

    void Update()
    {
        ManageThrustMode();
    }

    void FixedUpdate()
    {
        if (activeThrustMode == ThrustMode.Off)
            return;

        FreeCruise();
        Hover();
        Throttle();
        Roll();
        PitchYaw();
        Elevate();

        currentSpeed = rb.velocity.magnitude;

        if (rb.angularVelocity != Vector3.zero || rb.velocity != Vector3.zero)
            transform.position = hull.transform.position;

        speedLabel.text = currentSpeed.ToString();
        thrustLabel.text = thrust.ToString();
    }

    void ManageThrustMode()
    {

        if (activeThrustMode == ThrustMode.Hover)
        {
            if (InputManager.ThrottleDownButtonUp())
                SetCruise();

            if (InputManager.ThrottleUpButtonUp())
                SetReverse();
        }

        if (activeThrustMode == ThrustMode.Off && InputManager.ThrottleUpButtonDown() ||
            activeThrustMode == ThrustMode.Free && InputManager.ThrottleUpButtonDown())
        {
            SetCruise();
        }

        if (activeThrustMode == ThrustMode.Off && InputManager.ThrottleDownButtonDown() ||
            activeThrustMode == ThrustMode.Free && InputManager.ThrottleDownButtonDown())
        {
            SetReverse();
        }

        if (activeThrustMode == ThrustMode.Cruise)
        {
            if (InputManager.ThrottleUpButtonUp())
                SetFree();

            if (InputManager.ThrottleDownButtonDown())
                SetHover();
        }

        if (activeThrustMode == ThrustMode.Reverse)
        {
            if (InputManager.ThrottleDownButtonUp())
                SetFree();

            if (InputManager.ThrottleUpButtonDown())
                SetHover();
        }

        if (InputManager.ThrottleUpButtonDown() && InputManager.ThrottleDownButtonDown())
            SetHover();
    }

    void SetFree()
    {
        activeThrustMode = ThrustMode.Free;
        modeLabel.text = "Free";
        horizontalThrust = 0.0f;
    }

    void SetHover()
    {
        activeThrustMode = ThrustMode.Hover;
        modeLabel.text = "Hover";
    }

    void SetCruise()
    {
        activeThrustMode = ThrustMode.Cruise;
        modeLabel.text = "Cruise";
        horizontalThrust = 0.0f;
    }

    void SetReverse()
    {
        activeThrustMode = ThrustMode.Reverse;
        modeLabel.text = "Reverse";
        horizontalThrust = 0.0f;
    }

    void Throttle()
    {
        if (activeThrustMode == ThrustMode.Hover || activeThrustMode == ThrustMode.Free)
            return;

        float target = 0.0f;

        if (activeThrustMode == ThrustMode.Cruise)
            target = Mathf.Clamp(thrust + acceleration * Time.deltaTime, minThrust, maxThrust);

        if (activeThrustMode == ThrustMode.Reverse)
            target = Mathf.Clamp(thrust - acceleration * Time.deltaTime, minThrust, maxThrust);

        thrust = Mathf.Lerp(thrust, target, 0.5f);
        transform.position += transform.forward * thrust * Time.deltaTime;
    }

    void Elevate()
    {
        float yAxis = InputManager.LeftVerticalAxis();
        float target = 0.0f;

        if (yAxis > 0)
            target = Mathf.Clamp(verticalThrust + acceleration * Time.deltaTime, minVerticalThrust, maxVerticalThrust);

        if (yAxis < 0)
            target = Mathf.Clamp(verticalThrust - acceleration * Time.deltaTime, minVerticalThrust, maxVerticalThrust);

        if (yAxis == 0.0f && verticalThrust > 0)
            target = Mathf.Clamp(verticalThrust - acceleration * Time.deltaTime, 0, maxVerticalThrust);

        if (yAxis == 0.0f && verticalThrust < 0)
            target = Mathf.Clamp(verticalThrust + acceleration * Time.deltaTime, minVerticalThrust, 0);

        verticalThrust = Mathf.Lerp(verticalThrust, target, 5 * Time.deltaTime);
        transform.position += transform.up * verticalThrust * Time.deltaTime;
        pilotCamController.Elevate(-yAxis);
    }

    void PitchYaw()
    {
        if (activeThrustMode == ThrustMode.Off)
            return;

        Pitch();
        Yaw();
    }

    void Pitch()
    {
        float yAxis = -InputManager.RightVerticalAxis();
        float target = yAxis * pitchSensitivity * Time.deltaTime;
        rotationY = Mathf.Lerp(rotationY, target, 5 * Time.deltaTime);
        Vector3 localRotation = new Vector3(rotationY, 0.0f, 0.0f);
        transform.Rotate(localRotation);
        pilotCamController.Pitch(-yAxis, thrust / maxThrust);
    }

    void Yaw()
    {
        float xAxis = InputManager.RightHorizontalAxis();
        float target = xAxis * yawSenstivity * Time.deltaTime;
        rotationX = Mathf.Lerp(rotationX, target, 5 * Time.deltaTime);
        Vector3 localRotation = new Vector3(0.0f, rotationX, 0.0f);
        transform.Rotate(localRotation);
        pilotCamController.Yaw(xAxis, thrust / maxThrust);
        Bank(xAxis);
    }

    void Roll()
    {
        if (activeThrustMode == ThrustMode.Hover)
            return;

        float zAxis = InputManager.LeftHorizontalAxis() * -1f * rollSpeed;
        rotationZ = Mathf.Lerp(rotationZ, zAxis, 5 * Time.deltaTime);
        transform.Rotate(new Vector3(0.0f, 0.0f, rotationZ) * Time.deltaTime);
    }

    void FreeCruise()
    {
        if (activeThrustMode != ThrustMode.Free)
            return;

        float target = 0.0f;

        if (thrust > 0)
            target = Mathf.Clamp(thrust - 10f * Time.deltaTime, minThrust, maxThrust);

        if (thrust < 0)
            target = Mathf.Clamp(thrust + 10f * Time.deltaTime, minThrust, maxThrust);

        thrust = Mathf.Lerp(thrust, target, 5 * Time.deltaTime);
        transform.position += transform.forward * thrust * Time.deltaTime;
    }

    void Hover()
    {
        if (activeThrustMode != ThrustMode.Hover)
            return;

        StabilizeFromTumble();

        Strafe();

        float target = 0.0f;

        if (thrust > 0)
            target = Mathf.Clamp(thrust - 20f * Time.deltaTime, minThrust, maxThrust);

        if (thrust < 0)
            target = Mathf.Clamp(thrust + 20f * Time.deltaTime, minThrust, maxThrust);

        thrust = Mathf.Lerp(thrust, target, 5 * Time.deltaTime);
        transform.position += transform.forward * thrust * Time.deltaTime;
    }

    void Strafe()
    {
        float xAxis = InputManager.LeftHorizontalAxis();
        float target = 0.0f;
        if (xAxis > 0)
            target = Mathf.Clamp(horizontalThrust + acceleration * Time.deltaTime, minHorizontalThrust, maxHorizontalThrust);

        if (xAxis < 0)
            target = Mathf.Clamp(horizontalThrust - acceleration * Time.deltaTime, minHorizontalThrust, maxHorizontalThrust);

        if (xAxis == 0.0f && horizontalThrust > 0)
            target = Mathf.Clamp(horizontalThrust - acceleration * Time.deltaTime, 0, maxHorizontalThrust);

        if (xAxis == 0.0f && horizontalThrust < 0)
            target = Mathf.Clamp(horizontalThrust + acceleration * Time.deltaTime, minHorizontalThrust, 0);

        Bank(xAxis);
        horizontalThrust = Mathf.Lerp(horizontalThrust, target, 5 * Time.deltaTime);
        transform.position += transform.right * horizontalThrust * Time.deltaTime;
        pilotCamController.Strafe(-xAxis);
    }

    void Bank(float xMovement)
    {
        /*
         * Turn left
         * negative xMovement
         * postive bank
         * 
         * Turn right
         * positve xMovement
         * negative bank
         */

        bank = Mathf.Lerp(bank, -xMovement * maxBank * Time.deltaTime * thrust / 10, Time.deltaTime * 2);
        Vector3 rotation = new Vector3(0.0f, 0.0f, bank);
        hull.transform.localEulerAngles = rotation;
    }

    void StabilizeFromTumble()
    {
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, 2F * Time.deltaTime);
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 2F * Time.deltaTime);
        hull.transform.localPosition = Vector3.Lerp(hull.transform.localPosition, Vector3.zero, 2f * Time.deltaTime);
    }

    public void SetPilot(GameObject player)
    {
        // TODO: Figure out why I need to call this instead of relying on the call on Start()
        hull = transform.Find("Hull").gameObject;

        pilot = player;
        pilot.transform.localPosition = pilotPosition.transform.position;
        pilot.transform.localEulerAngles = new Vector3(0, 0, 0);
        pilot.transform.SetParent(hull.transform);

        pilotCamController = GetComponent<PilotCamController>();
        pilotCamController.enabled = true;
        pilotCamController.SetCameraRig(pilot.transform.Find("MultipurposeCameraRig").gameObject);
    }

    public void RemovePilot()
    {
        pilot.transform.localPosition = pilotExitPosition.transform.localPosition;
        pilot.transform.localEulerAngles = new Vector3(0, 0, 0);
        pilot = null;
        pilotCamController.RemoveCameraRig();
        pilotCamController.enabled = false;
    }
}