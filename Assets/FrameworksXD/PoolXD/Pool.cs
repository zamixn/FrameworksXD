using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T>
{
    private List<T> Unused;
    private List<T> All;

    private Func<T> InstanceGetter;
    private Action<T> OnPushed;
    private Action<T> OnPopped;

    public const int DefaultPoolSize = 10;

    public Pool(Func<T> instanceGetter, int poolSize = DefaultPoolSize, Action<T> onPushed = null, Action<T> onPopped = null)
    {
        InstanceGetter = instanceGetter;
        OnPushed = onPushed;
        OnPopped = onPopped;

        All = new List<T>();
        for (int i = 0; i < poolSize; i++)
            All.Add(InstanceGetter.Invoke());

        Unused = new List<T>();
        foreach (var t in All)
        {
            Push(t);
        }
    }

    public void Push(T t)
    {
        OnPushed?.Invoke(t);
        Unused.Add(t);
    }

    /// <summary>
    /// pops an unused element from the pool
    /// </summary>
    /// <param name="pushIn">if greater than 0, popped element is automaticallly pushed into the pool</param>
    /// <returns></returns>
    public T Pop(float pushIn = -1)
    {
        SpawnIfNeeded();
        T t = Unused[0];
        Unused.RemoveAt(0);
        OnPopped?.Invoke(t);
        if (pushIn > 0)
        {
            DelayedEvents.Invoke(pushIn, () => Push(t));
        }
        return t;
    }

    private void SpawnIfNeeded()
    {
        if (Unused.Count == 0)
        {
            var t = InstanceGetter.Invoke();
            All.Add(t);
            Unused.Add(t);
        }
    }

}