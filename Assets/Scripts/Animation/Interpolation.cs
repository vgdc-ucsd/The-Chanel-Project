using UnityEngine;

public enum InterpolationMode
{
    Linear,
    Slerp,
    EaseIn,
    EaseOut
}

public static class Interpolation
{
    public static float Interpolate(float from, float to, float t, InterpolationMode mode) {
        switch(mode) {
            case InterpolationMode.Linear:
                return InterpolateLinear(from, to, t);
            case InterpolationMode.Slerp:
                return Slerp(from, to, t);
            case InterpolationMode.EaseIn:
                return EaseIn(from, to, t);
            case InterpolationMode.EaseOut:
                return EaseOut(from, to, t);
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
            case InterpolationMode.Slerp:
                return new Vector3(
                    Slerp(from.x, to.x, t),
                    Slerp(from.y, to.y, t),
                    Slerp(from.z, to.z, t)
                );
            case InterpolationMode.EaseIn:
                return new Vector3(
                    EaseIn(from.x, to.x, t),
                    EaseIn(from.y, to.y, t),
                    EaseIn(from.z, to.z, t)
                );
            case InterpolationMode.EaseOut:
                return new Vector3(
                    EaseOut(from.x, to.x, t),
                    EaseOut(from.y, to.y, t),
                    EaseOut(from.z, to.z, t)
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
            case InterpolationMode.Slerp:
                return new Color(
                    Slerp(from.r, to.r, t),
                    Slerp(from.g, to.g, t),
                    Slerp(from.b, to.b, t),
                    Slerp(from.a, to.a, t)
                );
            case InterpolationMode.EaseIn:
                return new Color(
                    EaseIn(from.r, to.r, t),
                    EaseIn(from.g, to.g, t),
                    EaseIn(from.b, to.b, t),
                    EaseIn(from.a, to.a, t)
                );
            case InterpolationMode.EaseOut:
                return new Color(
                    EaseOut(from.r, to.r, t),
                    EaseOut(from.g, to.g, t),
                    EaseOut(from.b, to.b, t),
                    EaseOut(from.a, to.a, t)
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

    // Quadratic
    // See https://easings.net/
    private static float Slerp(float from, float to, float t) {
        t = t < 0.5f ? 2f * t * t : 1 - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        return from+(t*(to-from));
    }

    private static float EaseIn(float from, float to, float t) {
        t = t*t;
        return from+(t*(to-from));
    }
    
    private static float EaseOut(float from, float to, float t) {
        t = 1f - (1f - t) * (1f - t);
        return from+(t*(to-from));
    }
}
