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

    // Use this for initialization
    void Start()
    {
        hull = transform.Find("Hull").gameObject;
    }

    public void Yaw(float shipYaw)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetX = shipYaw * 3.5f + defaultCameraPosition.x;
        Vector3 targetPosition = new Vector3(targetX, currentPosition.y, currentPosition.z);

        //if (shipYaw == 0f)
            //targetPosition = defaultCameraPosition;

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, 1.5f * Time.deltaTime);
    }

    public void Pitch(float shipPitch)
    {
        Vector3 currentPosition = cam.transform.localPosition;
        float targetY = shipPitch * 3.5f + defaultCameraPosition.y;
        Vector3 targetPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);

        //if (shipPitch == 0f)
            //targetPosition = defaultCameraPosition;

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, 1.5f * Time.deltaTime);
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