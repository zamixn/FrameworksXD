using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool : MonoBehaviour
{
    private static PrefabPool instance;
    public static PrefabPool Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PrefabPool>();
            if (instance == null)
                throw new System.Exception("Instance of ParticleManager not present in scene");
            return instance;
        }
    }

    private Dictionary<GameObject, Pool<GameObject>> Pools = new Dictionary<GameObject, Pool<GameObject>>();

    private void AddNewPool(GameObject prefab)
    {
        Pools.Add(prefab, new Pool<GameObject>(
            () => { return Instantiate(prefab, transform); },
            onPushed: (obj) => obj.SetActive(false),
            onPopped: (obj) => obj.SetActive(true)
            ));
    }

    public T Spawn<T>(T prefab, float destroyIn = -1) where T : Component
    {
        if (!Pools.ContainsKey(prefab.gameObject))
            AddNewPool(prefab.gameObject);

        return Pools[prefab.gameObject].Pop(destroyIn).GetComponent<T>();
    }
}
