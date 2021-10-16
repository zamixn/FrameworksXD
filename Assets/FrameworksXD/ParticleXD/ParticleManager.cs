using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager instance;
    public static ParticleManager Instance
    {
        get 
        {
            if (instance == null)
                instance = FindObjectOfType<ParticleManager>();
            if (instance == null)
                throw new System.Exception("Instance of ParticleManager not present in scene");
            return instance;
        }
    }

    [Tooltip("Create a scriptable object of particle data and assign. (Create -> FrameworksXD/ParticleData)")]
    [SerializeField] private ParticleData ParticleData;

    private void Awake()
    {
        ParticleData.Initialize();
    }

    public void Play(ParticleType type, Vector3 position, Quaternion rotation)
    {
        var prefab = ParticleData.Particles[type];

        var aliveTime = prefab.main.duration;

        PrefabPool.Instance.Spawn(prefab, aliveTime);
    }
}
