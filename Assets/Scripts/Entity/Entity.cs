using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    // Need a way to get head location from entities for LOS raycasting, etc!
    public abstract Transform GetHeadTransform();
}
