using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotCamController : MonoBehaviour
{

    public Vector3 defaultCameraRigPosition;
    public Vector3 defaultCameraPivotPosition;
    public Vector3 defaultCameraPosition;
    public Vector3 defaultCameraRotation;

    public float cameraLag = 0.95f;

    private GameObject rig;
    private GameObject pivot;
    private Camera cam;

    private GameObject hull;

    private float maxMod = 3.5f;

    // Use this for initialization
    void Start()
    {
        hull = transform.Find("Hull").gameObject;
    }

    public void Pitch(float shipPitch, float currentThrustPercentage)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetY = shipPitch * (currentThrustPercentage * maxMod + 1) + defaultCameraPosition.y;
        Vector3 targetPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, 1.5f * Time.deltaTime);
    }

    public void Yaw(float shipYaw, float currentThrustPercentage)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetX = shipYaw * (currentThrustPercentage * maxMod + 1) + defaultCameraPosition.x;
        Vector3 targetPosition = new Vector3(targetX, currentPosition.y, currentPosition.z);

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, 1.5f * Time.deltaTime);
    }

    public void Strafe(float shipStrafe) 
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetX = shipStrafe + defaultCameraPosition.x;
        Vector3 targetPosition = new Vector3(targetX, currentPosition.y, currentPosition.z);

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime);
    }

    public void Elevate(float shipElevate)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetY = shipElevate + defaultCameraPosition.y;
        Vector3 targetPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime);
    }

    public void SetCameraRig(GameObject rigObject)
    {
        Debug.Log("Setting rig: " + rigObject);

        rig = rigObject;
        pivot = rig.transform.Find("Pivot").gameObject;
        cam = pivot.transform.Find("MainCamera").gameObject.GetComponent<Camera>();
        SetCameraPosition();
    }

    public void RemoveCameraRig()
    {
        rig = null;
        pivot = null;
        cam = null;
    }

    void SetCameraPosition()
    {
        rig.transform.SetParent(gameObject.transform);
        rig.transform.localScale = new Vector3(1, 1, 1);
        rig.transform.localPosition = defaultCameraRigPosition;
        pivot.transform.localPosition = defaultCameraPivotPosition;
        cam.transform.localPosition = defaultCameraPosition;
        cam.transform.localEulerAngles = defaultCameraRotation;
    }
}