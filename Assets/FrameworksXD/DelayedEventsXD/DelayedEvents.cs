using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DelayedEvents
{
    private class DelayedEventInvoker : MonoBehaviour { }

    private static DelayedEventInvoker _CouroutineHandle;
    private static DelayedEventInvoker CouroutineHandle
    {
        get 
        {
            if (_CouroutineHandle == null)
            {
                var go = new GameObject("DelayedEvents_CouroutineHandle");
                GameObject.DontDestroyOnLoad(go);
                _CouroutineHandle = go.AddComponent(typeof(DelayedEventInvoker)) as DelayedEventInvoker;
            }

            return _CouroutineHandle;
        }
    }

    public static void Invoke(float secondDelay, Action a)
    {
        CouroutineHandle.StartCoroutine(DelaySeconds(secondDelay, a));
    }

    private static IEnumerator DelaySeconds(float secondDelay, Action a)
    {
        yield return new WaitForSeconds(secondDelay);
        a?.Invoke();
    }
}
