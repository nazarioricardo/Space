using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Made by Lil Sumn Games
 * 
 * Thanks for purchasing our asset!
 * DM us on Twitter @lil_sumn_games with questions, suggestions
 * feedback, or help! We are happy to assist with anything you might need!
 * 
 */

public class ShipController : MonoBehaviour
{

    [Header("Ship Controller Settings")]

    [Header(" ")]

    [Tooltip("Forward speed of the ship")]
    public float Speed = 40;
    [Tooltip("Speed of rolling turning")]
    public float RollSpeed = 90;
    [Range(0f, 1f)]
    [Tooltip("Rolling turn acceleration")]
    public float RollAcceleration = 0.5f;
    [Tooltip("Speed of vertical turning")]
    public float VerticalTurnSpeed = 60;
    [Tooltip("Vertical turn accelerations")]
    [Range(0f, 1f)]
    public float VerticalTurnAcceleration = 0.5f;
    [Tooltip("How much the camera lags behind ship during accelerations")]
    [Range(0f, 1f)]
    public float CameraLag = 0.95f;

    private Transform _camTarget;

    private float _prevVert, _prevRoll;
    private Camera _cam;

    // Use this for initialization
    void Start()
    {
        // initialize children links
        _cam = GetComponentInChildren<Camera>();
        _camTarget = transform.Find("cam-target");
    }

    // Update is called once per frame
    void Update()
    {
        // translate forward with the camera attached
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);

        // calculate vertical turn and roll velocities
        float vert = Input.GetAxis("Vertical") * VerticalTurnSpeed;
        float roll = Input.GetAxis("Horizontal") * -1f * RollSpeed;

        _prevVert = Mathf.Lerp(_prevVert, vert, VerticalTurnAcceleration);
        _prevRoll = Mathf.Lerp(_prevRoll, roll, RollAcceleration);

        // save camera transform
        Vector3 cpos = _cam.transform.position;
        Quaternion crot = _cam.transform.rotation;

        // rotate without the camera
        transform.Rotate(new Vector3(_prevVert, 0, _prevRoll) * Time.deltaTime);
        _cam.transform.position = cpos;
        _cam.transform.rotation = crot;

        // lerp the camera back to its correct position
        _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, _camTarget.transform.localPosition, 1f - CameraLag);
        _cam.transform.localRotation = Quaternion.Lerp(_cam.transform.localRotation, _camTarget.transform.localRotation, 1f - CameraLag);
    }
}