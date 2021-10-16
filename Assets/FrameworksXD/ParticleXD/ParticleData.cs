using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleData", menuName = "FrameworksXD/ParticleData")]
public class ParticleData : ScriptableObject
{
    [System.Serializable]
    public class ParticleKVP 
    {
        public string Key;
        public ParticleSystem Value;
    }

    [SerializeField] public List<ParticleKVP> ParticleDataRaw;

    [NonSerialized] private Dictionary<ParticleType, ParticleSystem> _Particles;
    public Dictionary<ParticleType, ParticleSystem> Particles
    {
        get 
        {
            if (_Particles == null)
            {
                Initialize();
            }

            return _Particles;
        }
    }

    public void Initialize()
    {
        _Particles = new Dictionary<ParticleType, ParticleSystem>();
        foreach (var kvp in ParticleDataRaw)
        {
            var e = (ParticleType)Enum.Parse(typeof(ParticleType), kvp.Key);
            _Particles.Add(e, kvp.Value);
        }
    }
}