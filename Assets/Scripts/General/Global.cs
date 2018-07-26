using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static float FloatLerp(float value, float target, float damper, float threshold)
    {

        value = Mathf.Lerp(value, target, damper);

        if (value < 0.0f)
        {
            if (value > -threshold)
                return 0.0f;
        }
        else 
        {
            if (value < threshold)
                return 0.0f; 
        }

        return value;
    }

    public static Vector3 Vector3Lerp(Vector3 value, Vector3 target, float damper, float threshold)
    {

        value = Vector3.Lerp(value, target, damper);

        if (value.magnitude < threshold)
            value = Vector3.zero;

        return value;
    }
}
