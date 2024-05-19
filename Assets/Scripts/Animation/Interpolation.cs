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

    public static Color Interpolate(Color from, Color to, float t, InterpolationMode mode) {
        switch(mode) {
            case InterpolationMode.Linear:
                return new Color(
                    InterpolateLinear(from.r, to.r, t),
                    InterpolateLinear(from.g, to.g, t),
                    InterpolateLinear(from.b, to.b, t),
                    InterpolateLinear(from.a, to.a, t)
                );
            default: 
                return new Color(
                    InterpolateLinear(from.r, to.r, t),
                    InterpolateLinear(from.g, to.g, t),
                    InterpolateLinear(from.b, to.b, t),
                    InterpolateLinear(from.a, to.a, t)
                );
        }
    }
    
    private static float InterpolateLinear(float from, float to, float t) {
        return from+(t*(to-from));
    }
}
