using RotaryHeart.Lib.SerializableDictionary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleData", menuName = "FrameworksXD/ParticleData")]
public class ParticleData : ScriptableObject
{
    [Serializable]
    public class ParticleDictionary : SerializableDictionaryBase<ParticleType, ParticleSystem> { }

    public ParticleDictionary Particles;
}