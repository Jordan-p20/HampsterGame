using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    /*
     * keeps track of health
     */

    public float currentHealth { get; private set; }//the current health
    public float maxHealth { get; private set; }// the max health

    public bool canBeDamaged { get; private set; }//whether or not the character can be damaged

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //alter the current health by amount
    public void AlterCurrentHealth(float amount)
    {
        currentHealth += amount;
    }

    //alter the max health by amount
    public void AlterMaxHealth(float amount)
    {
        if (!canBeDamaged) { return; }
        maxHealth += amount;
        if (currentHealth <= 0)
        {
            OnDeath();
        }
        else if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    //make the character not be able to be damaged
    public void GiveInvincibility()
    {
        canBeDamaged = false;
    }

    //make the chracter be able to be damaged
    public void MakeVulnerable()
    {
        canBeDamaged = true;
    }

    // what happens when the character dies
    public virtual void OnDeath()
    {

    }
}
