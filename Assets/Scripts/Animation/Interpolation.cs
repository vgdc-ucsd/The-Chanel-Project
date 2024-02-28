using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public enum InterpolationMode
{
    Linear
}

public static class Interpolation
{
    public static float Interpolate(float from, float to, float t, InterpolationMode mode) {
        switch(mode) {
            case InterpolationMode.Linear:
                return InterpolateLinear(from, to, t);
            default: 
                return InterpolateLinear(from, to, t);
        }
    }

    public static Vector3 Interpolate(Vector3 from, Vector3 to, float t, InterpolationMode mode) {
        switch(mode) {
            case InterpolationMode.Linear:
                return new Vector3(
                    InterpolateLinear(from.x, to.x, t),
                    InterpolateLinear(from.y, to.y, t),
                    InterpolateLinear(from.z, to.z, t)
                );
            default: 
                return new Vector3(
                    InterpolateLinear(from.x, to.x, t),
                    InterpolateLinear(from.y, to.y, t),
                    InterpolateLinear(from.z, to.z, t)
                );
        }
    }
    
    private static float InterpolateLinear(float from, float to, float t) {
        return from+(t*(to-from));
    }
}
