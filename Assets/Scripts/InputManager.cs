using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public static class InputManager {

    private static Player player = Rewired.ReInput.players.GetPlayer(0);

    /*
     * Axes
     */

    public static float LeftHorizontalAxis() {
        float r = 0.0f;
        r += player.GetAxis("Left Horizontal");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float LeftVerticalAxis() {
        float r = 0.0f;
        r += player.GetAxis("Left Vertical");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float RightHorizontalAxis() {
        float r = 0.0f;
        r += player.GetAxis("Right Horizontal");
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float RightVerticalAxis() {
        float r = 0.0f;
        r += player.GetAxis("Right Vertical");
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

        return player.GetButtonDown("Action");
    }

    public static bool JumpButtonDown() {
        /*
         * PC: Space
         * PS: X, joystick button 1
         * XBox: A, joystick button 16
         */

        return player.GetButtonDown("Jump");
    }

    public static bool BrakeButtonDown() {
        /*
         * PC: F
         * PS: Circle, joystick button 2
         * XBox: B, joystick button 17
         */

        return player.GetButtonDown("Brake");
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

        return player.GetButtonDown("Left Trigger");
    }

    public static bool ThrottleDownButtonUp() {
        return player.GetButtonUp("Left Trigger");
    }

    public static bool ThrottleUpButtonDown() {
        /*
         * PC: E
         * PS: R2, joystick button 7 or 5th Axis
         * XBox: Right Trigger, 6th AXIS
         */

        return player.GetButtonDown("Right Trigger");
    }

    public static bool ThrottleUpButtonUp() {
        return player.GetButtonUp("Right Trigger");
    }
}
