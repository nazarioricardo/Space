using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotCamController : MonoBehaviour
{

    public Vector3 defaultCameraRigPosition;
    public Vector3 defaultCameraPivotPosition;
    public Vector3 defaultCameraPosition;

    private GameObject rig;
    private GameObject pivot;
    private Camera camera;

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

    public void SetCameraRig(GameObject rigObject)
    {
        Debug.Log("Setting rig: " + rigObject);
        rig = rigObject;
        pivot = rig.transform.GetChild(0).gameObject;
        camera = pivot.transform.GetChild(0).gameObject.GetComponent<Camera>();
        SetCameraPosition();
    }

    public void RemoveCameraRig()
    {
        rig = null;
        pivot = null;
        camera = null;
    }

    void SetCameraPosition()
    {
        rig.transform.SetParent(gameObject.transform);
        rig.transform.localScale = new Vector3(1, 1, 1);
        rig.transform.localPosition = defaultCameraRigPosition;
        pivot.transform.localPosition = defaultCameraPivotPosition;
        camera.transform.localPosition = defaultCameraPosition;
        camera.transform.localEulerAngles = new Vector3(5, 0, 0);
    }
}