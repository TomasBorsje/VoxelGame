using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBreakEffect
{
    static GameObject explosionEffect = Resources.Load<GameObject>("Prefabs/Effect/BlockBreakEffect");
    public static GameObject CreateBlockBreakEffect(Vector3 pos)
    {
        GameObject obj = GameObject.Instantiate(explosionEffect);
        obj.transform.position = pos;
        //ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        return obj;
    }
}
