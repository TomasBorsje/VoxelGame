using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : Entity
{
    public int maxHealth = 20;
    public int currentHealth = 20;

    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth < 0)
        {
            OnDie();
        }
    }
    private void OnDie()
    {
        Debug.Log(this.GetType().Name + " died!");
    }
}
