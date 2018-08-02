using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

public class PilotController : MonoBehaviour
{

    // TODO: This is the next class that should be refactored.

    // UI
    public Canvas canvas;
    public Text modeLabel;
    public Text thrustLabel;
    public Text speedLabel;

    // Inputs
    private InputManager InputManager;

    // Camera
    private PilotCamController pilotCamController;

    // Combat
    private WeaponsController weaponsController;

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
    public float sensitivityZ = 0.5f;
    public float rollSpeed = 90;
    public float acceleration = 50f;
    public float brakeModifier = 20f;

    public float cruiseSpeed = 60f;
    public float attackSpeed = 80f;

    private GameObject hull;
    private Rigidbody hullRigidbody;

    // Player Management
    [HideInInspector]
    public GameObject pilot;

    // Ship Movement
    private enum ThrustMode { Off, Free, Hover, Cruise, Reverse }
    private ThrustMode activeThrustMode;
    private Vector3 movement;

    private float thrust = 0.0f;
    private float maxThrust = 400.0f;
    private float minThrust = -100.0f;

    private float vThrust = 0.0f;
    private float maxV = 100.0f;
    private float minV = -100.0f;

    private float sThrust = 0.0f;
    private float maxS = 100.0f;
    private float minS = -100.0f;

    private float bank = 0.0f;
    private float currentSpeed = 0.0f;

    private bool needStabilizing = false;
    private bool isStabilizing = false;

    private void Awake()
    {
        hull = transform.Find("Hull").gameObject;
        hullRigidbody = GetComponent<Rigidbody>();  
    }

    private void Start()
    {
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

        currentSpeed = hullRigidbody.velocity.magnitude;
        speedLabel.text = currentSpeed.ToString();
        thrustLabel.text = thrust.ToString();

        if (hullRigidbody.angularVelocity != Vector3.zero || hullRigidbody.velocity != Vector3.zero)
        {
            needStabilizing = true;
            pilotCamController.OnDestabilize(isStabilizing);
        }
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
        sThrust = 0.0f;
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
        sThrust = 0.0f;
    }

    void SetReverse()
    {
        activeThrustMode = ThrustMode.Reverse;
        modeLabel.text = "Reverse";
        sThrust = 0.0f;
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

        //thrust = Mathf.Lerp(thrust, target, 0.5f);
        thrust = Global.FloatLerp(thrust, target, 0.5f, 0.01f);
        transform.position += transform.forward * thrust * Time.deltaTime;
    }

    void Elevate()
    {
        float yAxis = InputManager.LeftVerticalAxis();
        float target = 0.0f;

        if (yAxis > 0)
            target = Mathf.Clamp(vThrust + acceleration * Time.deltaTime, minV, maxV);

        if (yAxis < 0)
            target = Mathf.Clamp(vThrust - acceleration * Time.deltaTime, minV, maxV);

        if (yAxis == 0.0f && vThrust > 0)
            target = Mathf.Clamp(vThrust - acceleration * Time.deltaTime, 0, maxV);

        if (yAxis == 0.0f && vThrust < 0)
            target = Mathf.Clamp(vThrust + acceleration * Time.deltaTime, minV, 0);

        //vThrust = Mathf.Lerp(vThrust, target, 5 * Time.deltaTime);
        vThrust = Global.FloatLerp(vThrust, target, 0.5f, 0.01f);
        transform.position += transform.up * vThrust * Time.deltaTime;

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
        float target = yAxis * sensitivityY * Time.deltaTime;
        rotationY = Global.FloatLerp(rotationY, target, 0.5f, 0.01f);
        Vector3 localRotation = new Vector3(rotationY, 0.0f, 0.0f);
        transform.Rotate(localRotation);
        pilotCamController.Pitch(-yAxis, thrust / maxThrust);
    }

    void Yaw()
    {
        float xAxis = InputManager.RightHorizontalAxis();
        float target = xAxis * sensitivityX * Time.deltaTime;
        rotationX = Global.FloatLerp(rotationX, target, 0.5f, 0.01f);
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
        rotationZ = Global.FloatLerp(rotationZ, zAxis, 0.5f, 0.01f);
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

        thrust = Global.FloatLerp(thrust, target, 0.5f, 0.01f);
        transform.position += transform.forward * thrust * Time.deltaTime;
    }

    void Hover()
    {
        if (activeThrustMode != ThrustMode.Hover)
            return;

        Stabilize();

        Strafe();

        float target = 0.0f;

        if (thrust > 0)
            target = Mathf.Clamp(thrust - 20f * Time.deltaTime, minThrust, maxThrust);

        if (thrust < 0)
            target = Mathf.Clamp(thrust + 20f * Time.deltaTime, minThrust, maxThrust);

        thrust = Global.FloatLerp(thrust, target, 0.5f, 0.01f);
        transform.position += transform.forward * thrust * Time.deltaTime;
    }

    void Strafe()
    {
        float xAxis = InputManager.LeftHorizontalAxis();
        float target = 0.0f;
        if (xAxis > 0)
            target = Mathf.Clamp(sThrust + acceleration * Time.deltaTime, minS, maxS);

        if (xAxis < 0)
            target = Mathf.Clamp(sThrust - acceleration * Time.deltaTime, minS, maxS);

        if (xAxis == 0.0f && sThrust > 0)
            target = Mathf.Clamp(sThrust - acceleration * Time.deltaTime, 0, maxS);

        if (xAxis == 0.0f && sThrust < 0)
            target = Mathf.Clamp(sThrust + acceleration * Time.deltaTime, minS, 0);

        Bank(xAxis);
        sThrust = Global.FloatLerp(sThrust, target, 0.5f, 0.01f);
        transform.position += transform.right * sThrust * Time.deltaTime;

        pilotCamController.Strafe(-xAxis);
    }

    void Stabilize()
    {
        isStabilizing = true;
        pilotCamController.OnStabilize();

        hullRigidbody.angularVelocity = Global.Vector3Lerp(hullRigidbody.angularVelocity, Vector3.zero, 0.5f, 0.05f);
        hullRigidbody.velocity = Global.Vector3Lerp(hullRigidbody.velocity, Vector3.zero, 0.5f, 0.05f);

        if (hullRigidbody.angularVelocity == Vector3.zero && hullRigidbody.velocity == Vector3.zero)
        {
            needStabilizing = false;
            isStabilizing = false;   
        }
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

        float target = -xMovement * 70 * Time.deltaTime * thrust / 10;
        bank = Global.FloatLerp(bank, target, 2f * Time.deltaTime, 0.01f);
        Vector3 rotation = new Vector3(0.0f, 0.0f, bank);
        hull.transform.localEulerAngles = rotation;
    }

    public bool SetPilot(GameObject player)
    {

        if (pilot)
            return false;

        GUIHandler.instance.ToggleGUI();

        hull = transform.Find("Hull").gameObject;

        pilot = player;
        pilot.transform.localPosition = pilotPosition.transform.position;
        pilot.transform.rotation = transform.rotation;
        pilot.transform.SetParent(hull.transform);

        pilotCamController = GetComponent<PilotCamController>();
        InputManager = pilot.GetComponent<InputManager>();
        pilotCamController.enabled = true;
        pilotCamController.SetCamera(pilot.transform.Find("MainCamera").gameObject);

        weaponsController = GetComponent<WeaponsController>();
        weaponsController.InputManager = InputManager;
        weaponsController.enabled = true;

        return true;
    }

    public void RemovePilot()
    {
        GUIHandler.instance.ToggleGUI();

        pilot.transform.localPosition = pilotExitPosition.transform.localPosition;
        pilot.transform.localEulerAngles = new Vector3(0, 0, 0);
        pilot = null;
        pilotCamController.RemoveCameraRig();
        pilotCamController.enabled = false;
        weaponsController.enabled = false;
    }
}