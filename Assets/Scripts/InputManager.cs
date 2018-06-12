using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager {

    /*
     * Axes
     */

    public static float LeftHorizontalAxis() {
        float r = 0.0f;
        r += Input.GetAxis("J Left Horizontal Axis");
        r += Input.GetAxis("K Left Horizontal Axis");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float LeftVerticalAxis() {
        float r = 0.0f;
        r += Input.GetAxis("J Left Vertical Axis");
        r += Input.GetAxis("K Left Vertical Axis");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float RightHorizontalAxis() {
        float r = 0.0f;
        r += Input.GetAxis("J Right Horizontal Axis");
        r += Input.GetAxis("K Right Horizontal Axis");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float RightVerticalAxis() {
        float r = 0.0f;
        r += Input.GetAxis("J Right Vertical Axis");
        r += Input.GetAxis("K Right Vertical Axis");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    /*
     * Buttons
     */

    public static bool ActionButtonDown() {
        /*
         * PC: U
         * PS: Square, joystick button 0
         * XBox: X, joystick button 18
         */

        return Input.GetButtonDown("Action Button");
    }

    public static bool JumpButtonDown() {
        /*
         * PC: Space
         * PS: X, joystick button 1
         * XBox: A, joystick button 16
         */

        return Input.GetButtonDown("Jump Button");
    }

    public static bool BrakeButtonDown() {
        /*
         * PC: F
         * PS: Circle, joystick button 2
         * XBox: B, joystick button 17
         */

        return Input.GetButtonDown("Brake Button");
    }

    /*
     * Triggers
     */

    public static bool ThrottleDownButtonDown() {
        /*
         * PC: Q
         * PS: L2, joystick button 6, or 4th Axis
         * Xbox: Left Trigger, 5th AXIS
         */

        return Input.GetButtonDown("Throttle Down Button");
    }

    public static bool ThrottleDownButtonUp() {
        return Input.GetButtonUp("Throttle Down Button");
    }

    public static bool ThrottleUpButtonDown() {
        /*
         * PC: E
         * PS: R2, joystick button 7 or 5th Axis
         * XBox: Right Trigger, 6th AXIS
         */

        return Input.GetButtonDown("Throttle Up Button");
    }

    public static bool ThrottleUpButtonUp() {
        return Input.GetButtonUp("Throttle Up Button");
    }
}
