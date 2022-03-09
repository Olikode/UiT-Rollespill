using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : MonoBehaviour
{

    public string name;
    public int level;
    public float maxHP;
    public float currentHP;
    // public float dodge;
    public bool isStuned = false;
    public bool isAsleep = false;


    //deal damage to unit and return true if unit dies
    public bool TakeDamage(float dmg){
        currentHP -= dmg;

      if(currentHP <= 0)
        return true;
      else
        return false;
    }

    public void HealUnit(float healAmount){
        currentHP += healAmount;

    //if unit is healed over max health set health to max
        if(currentHP > maxHP)
            currentHP = maxHP;
    }

    public float CalculateDmgNodifier(){
        float dmgModifier = 1.0f + 0.05f * level;
        return dmgModifier;
    }
}
