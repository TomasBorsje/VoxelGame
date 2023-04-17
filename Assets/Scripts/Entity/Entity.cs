using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected float age = 0;
    protected virtual void Update()
    {
        age += Time.deltaTime;
    }
}
