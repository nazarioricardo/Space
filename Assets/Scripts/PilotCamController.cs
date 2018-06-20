﻿using System.Collections;
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

    // Use this for initialization
    void Start()
    {
        hull = transform.Find("Hull").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
 
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