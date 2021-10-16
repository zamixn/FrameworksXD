using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOnClick : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var pos = ray.origin + ray.direction * 5;
            SpawnRandomParticle(pos);
        }
    }

    private void SpawnRandomParticle(Vector3 pos)
    {
        var types = System.Enum.GetValues(typeof(ParticleType));
        var type = (ParticleType)types.GetValue(Random.Range(0, types.Length));

        SpawnParticle(type, pos);
    }

    private void SpawnParticle(ParticleType type, Vector3 pos)
    {
        ParticleManager.Instance.Play(type, pos, Quaternion.identity);
    }
}
