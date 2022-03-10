using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestUnit : MonoBehaviour
{
    public int classID;
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

    /*public float CalculateDmgNodifier(){
        float dmgModifier = 1.0f + 0.05f * level;
        return dmgModifier;
    }*/

    public (float dmg, bool isStuned, bool isAsleep) UseAbilityAttack1(){

        // finds attack1 from the unit's class
        AbilityAttack abilityAttack = new AbilityAttack();
        var attackInfo = abilityAttack.FindAbilityAttack1(classID);

        // calculates damage
        float dmgModifier = 1.0f + 0.05f * level;
        float dmg = (attackInfo.baseDmg * dmgModifier);

        // returns damage and effects
        return (dmg, attackInfo.isStuned, attackInfo.isAsleep);
    }

        /*public (float dmg, bool isStuned, bool isAsleep) UseAbilityAttack2(){

        AbilityAttack abilityAttack = new AbilityAttack();
        var attackInfo = abilityAttack.FindAbilityAttack2(classID);

        float dmgModifier = 1.0f + 0.05f * level;
        float dmg = (attackInfo.baseDmg * dmgModifier);
        return (dmg, attackInfo.isStuned, attackInfo.isAsleep);
    }*/
        
}
