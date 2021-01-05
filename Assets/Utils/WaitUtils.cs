using System;
using System.Collections;
using UnityEngine;
using Utils;

public static class WaitUtils
{
    public static void Wait(float time, bool scaled, Action onComplete)
    {
        WaitIE(time, scaled, onComplete).Run();
    }
    public static IEnumerator WaitIE(float t, bool scaled, Action onComplete)
    {
        if (scaled)
            yield return new WaitForSeconds(t);
        else
            yield return new WaitForSecondsRealtime(t);
        onComplete?.Invoke();
    }

    public static void WaitUntil(Func<bool> isDone, Action onComplete)
    {
        WaitUntilIE(isDone, onComplete).Run();
    }

    public static IEnumerator WaitUntilIE(Func<bool> isDone, Action onComplete)
    {
        while (!isDone())
            yield return null;
        onComplete.Invoke();
    }
}