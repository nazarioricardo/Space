using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotCamController : MonoBehaviour
{

    public Vector3 defaultCameraRigPosition;
    public Vector3 defaultCameraPivotPosition;
    public Vector3 defaultCameraPosition;

    private GameObject cameraRig;
    private GameObject cameraPivot;
    private Camera mainCamera;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    public void SetCameraRig(GameObject rig)
    {
        Debug.Log("Setting rig: " + rig);
        cameraRig = rig;
        cameraPivot = cameraRig.transform.GetChild(0).gameObject;
        mainCamera = cameraPivot.transform.GetChild(0).gameObject.GetComponent<Camera>();
    }

    public void RemoveCameraRig()
    {
        cameraRig = null;
        cameraPivot = null;
        mainCamera = null;
    }

    void SetCameraPosition()
    {
        cameraRig.transform.SetParent(gameObject.transform);
        cameraRig.transform.localScale = new Vector3(1, 1, 1);
        cameraRig.transform.localPosition = defaultCameraRigPosition;
        cameraPivot.transform.localPosition = defaultCameraPivotPosition;
        mainCamera.transform.localPosition = defaultCameraPosition;
        mainCamera.transform.localEulerAngles = new Vector3(5, 0, 0);
    }
}