using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineTime
{
    private static Dictionary<float, WaitForSeconds> _waitForSeconds = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWaitForSecondsTime(float time)
    {
        if (_waitForSeconds.ContainsKey(time)) return _waitForSeconds[time];
        else
        {
            WaitForSeconds newTime = new WaitForSeconds(time);
            _waitForSeconds[time] = newTime;
            return newTime;
        }
    }
}
