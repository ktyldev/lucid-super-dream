using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class HitStopOnDie : MonoBehaviour
{
    [SerializeField] private float lengthInSeconds = 0.1f;

    private WaitForSecondsRealtime wait;

    private void Awake()
    {
        wait = new WaitForSecondsRealtime(lengthInSeconds);
    }

    public void OnDie()
    {
        Wait().Run();
    }

    private IEnumerator Wait()
    {
        //Time.timeScale = 0;
        yield return wait;
        Time.timeScale = 1;

    }
}
