using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float currentHealth { get; private set; }
    public float maxHealth { get; private set; }

    public bool canBeDamaged { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlterCurrentHealth(float amount)
    {
        currentHealth += amount;
    }

    public void AlterMaxHealth(float amount)
    {
        if (!canBeDamaged) { return; }
        maxHealth += amount;
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    public void GiveInvincibility()
    {
        canBeDamaged = false;
    }

    public void MakeVulnerable()
    {
        canBeDamaged = true;
    }

    public virtual void OnDeath()
    {

    }
}
