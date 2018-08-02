using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotCamController : MonoBehaviour
{
    public Vector3 defaultCameraPosition;
    public Vector3 defaultCameraRotation;

    public float maxCamDislocation = 3.5f;
    public float yawLag = 1.5f;
    public float pitchLag = 1.5f;

    private GameObject rig;
    private Camera cam;

    public void OnDestabilize(bool isStabilizing) 
    {
        if (isStabilizing)
            return;
    }

    public void OnStabilize()
    {
      
    }

    public void Pitch(float shipPitch, float currentThrustPercentage)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetY = shipPitch * (currentThrustPercentage * maxCamDislocation + 1) + defaultCameraPosition.y;
        Vector3 targetPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);

        cam.transform.localPosition = Global.Vector3Lerp(currentPosition, targetPosition, yawLag * Time.deltaTime, 0.05f);
    }

    public void Yaw(float shipYaw, float currentThrustPercentage)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetX = shipYaw * (currentThrustPercentage * maxCamDislocation + 1) + defaultCameraPosition.x;
        Vector3 targetPosition = new Vector3(targetX, currentPosition.y, currentPosition.z);

        cam.transform.localPosition = Global.Vector3Lerp(currentPosition, targetPosition, pitchLag * Time.deltaTime, 0.05f);
    }

    public void Strafe(float shipStrafe) 
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetX = shipStrafe + defaultCameraPosition.x;
        Vector3 targetPosition = new Vector3(targetX, currentPosition.y, currentPosition.z);

        cam.transform.localPosition = Global.Vector3Lerp(currentPosition, targetPosition, Time.deltaTime, 0.05f);
    }

    public void Elevate(float shipElevate)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetY = shipElevate + defaultCameraPosition.y;
        Vector3 targetPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);

        cam.transform.localPosition = Global.Vector3Lerp(currentPosition, targetPosition, Time.deltaTime, 0.05f);
    }

    public void SetCamera(GameObject camObject)
    {
        rig = camObject;
        cam = rig.gameObject.GetComponent<Camera>();

        rig.transform.SetParent(gameObject.transform);
        rig.transform.localPosition = Vector3.zero;
        rig.transform.localScale = new Vector3(1, 1, 1);
        rig.transform.rotation = gameObject.transform.rotation;

        cam.transform.localPosition = defaultCameraPosition;
        cam.transform.rotation = gameObject.transform.rotation;
    }

    public void RemoveCameraRig()
    {
        rig = null;
        cam = null;
    }
}