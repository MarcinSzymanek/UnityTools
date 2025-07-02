using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Useful convenience methods for linear interpolation of various data in Unity
 */
public class Lerp
{
    static Dictionary<Type, Delegate> lerpMethods = new Dictionary<Type, Delegate>()
    {
        [typeof(float)] = (Func<float, float, float, float>)Mathf.Lerp,
        [typeof(Vector3)] = (Func<Vector3, Vector3, float, Vector3>)Vector3.Lerp,
        [typeof(Color)] = (Func<Color, Color, float, Color>)Color.Lerp,
        [typeof(Quaternion)] = (Func<Quaternion, Quaternion, float, Quaternion>)Quaternion.Lerp
    };

    // See https://discussions.unity.com/t/how-to-implement-a-flexible-lerp-helper-method/1657772/11
    public static IEnumerator LerpRoutine<T>(
        Action<T> setter,
        T start,
        T end,
        float duration)
    {
        float t = 0;
        var lerp = (Func<T, T, float, T>)lerpMethods[typeof(T)];
        while (t < duration)
        {
            float factor = t / duration;
            T newValue = lerp(start, end, factor);
            setter(newValue);
            yield return null;
            t += Time.deltaTime;
        }
        setter(end);
    }

    public static IEnumerator LerpRoutineRealtime<T>(
        Action<T> setter,
        T start,
        T end,
        float duration,
        float tickAmount)
    {
        float t = 0;
        float tickValue = duration / tickAmount;
        var lerp = (Func<T, T, float, T>)lerpMethods[typeof(T)];
        while (t < duration)
        {
            float factor = t / duration;
            T newValue = lerp(start, end, factor);
            setter(newValue);
            yield return new WaitForSecondsRealtime(tickValue);
            t += tickValue;
        }
        setter(end);
    }
}