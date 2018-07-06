﻿using System.Collections;
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

    private GameObject shipHull;

    private void Awake()
    {
        shipHull = transform.Find("Hull").gameObject;
    }

    public void OnDestabilize(bool isStabilizing) 
    {
        if (isStabilizing)
            return;

        Debug.Log("On Destabilized Cam called");
        Vector3 direction = (shipHull.transform.localPosition - cam.transform.localPosition).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(defaultCameraRotation);
        Vector3 targetPosition = shipHull.transform.localPosition + defaultCameraPosition;

        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, targetRotation, 2f * Time.deltaTime);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetPosition, 2f * Time.deltaTime);
    }

    public void OnStabilize()
    {
        cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(defaultCameraRotation), 2f * Time.deltaTime);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, defaultCameraPosition, 2f * Time.deltaTime);
    }

    public void Pitch(float shipPitch, float currentThrustPercentage)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetY = shipPitch * (currentThrustPercentage * maxCamDislocation + 1) + defaultCameraPosition.y;
        Vector3 targetPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);
        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, yawLag * Time.deltaTime);
    }

    public void Yaw(float shipYaw, float currentThrustPercentage)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetX = shipYaw * (currentThrustPercentage * maxCamDislocation + 1) + defaultCameraPosition.x;
        Vector3 targetPosition = new Vector3(targetX, currentPosition.y, currentPosition.z);

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, pitchLag * Time.deltaTime);
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

    public void SetCamera(GameObject camObject)
    {
        Debug.Log("Setting rig");

        rig = camObject;
        rig.transform.SetParent(gameObject.transform);
        rig.transform.localPosition = Vector3.zero;
        rig.transform.localScale = new Vector3(1, 1, 1);

        cam = rig.gameObject.GetComponent<Camera>();
        cam.transform.localPosition = defaultCameraPosition;
        cam.transform.localEulerAngles = defaultCameraRotation;
    }

    public void RemoveCameraRig()
    {
        rig = null;
        cam = null;
    }

    void SetCameraPosition()
    {
        Debug.Log("Setting Cam");

        rig.transform.SetParent(gameObject.transform);
        rig.transform.localScale = new Vector3(1, 1, 1);
        rig.transform.localPosition = defaultCameraPosition;
        rig.transform.localEulerAngles = defaultCameraRotation;
    }
}