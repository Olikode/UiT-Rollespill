using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=System.Random;

public class TestUnit : MonoBehaviour
{
    public int classID;
    public string name;
    public int level;
    public float maxHP;
    public float currentHP;
    public bool isStuned = false;
    public bool isAsleep = false;

    public int initiative;
    public int critChance;
    public int dodgeScore = 10;
    public int hitScore = 0;

    // effects
    public int stun;
    public int sleep;
    public int poison;
    public int protection;

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

    public (float dmg, int hitModifier, int stun, int sleep, int poison, int protection) UseAbilityAttack1(bool isCritical){

        // finds attack1 from the unit's class
        AbilityAttack abilityAttack = new AbilityAttack();
        var attackInfo = abilityAttack.FindAbilityAttack1(classID, isCritical);

        // calculates damage
        float dmgModifier = 1.0f + 0.05f * level;
        float dmg = ((float)attackInfo.baseDmg * dmgModifier);

        // returns damage and effects
        return (dmg, attackInfo.hitModifier, attackInfo.stun, attackInfo.sleep, attackInfo.poison, attackInfo.protection);
    }

        public (float dmg, int hitModifier, int stun, int sleep, int poison, int protection) UseAbilityAttack2(bool isCritical){
        
        // finds attack1 from the unit's class
        AbilityAttack abilityAttack = new AbilityAttack();
        var attackInfo = abilityAttack.FindAbilityAttack2(classID, isCritical);

        // calculates damage
        float dmgModifier = 1.0f + 0.05f * level;
        float dmg = ((float)attackInfo.baseDmg * dmgModifier);

        // returns damage and effects
        return (dmg, attackInfo.hitModifier, attackInfo.stun, attackInfo.sleep, attackInfo.poison, attackInfo.protection);
    }

    public void CalculateHitScore(){
        Random rnd = new Random();
        int roll = rnd.Next(1, 21); // random number 1 - 20

        // if has item with hit bonus, + itemBonus
        this.hitScore = roll + level/2;
    }

    // this function should be moved to AbilityAttacks.cs
    public (string attackName1, string attackName2) FindAttackName(int classID){

        string attackName1 = "";
        string attackName2 = "";
        // attack description should be added later

        switch(classID){

            // test datateknikk
            case 1:
            attackName1 = "Hack Attack";
            attackName2 = "Nerdekraft";
            break;

            // test enemy
            case 2:
            attackName1 = "Slem slag";
            attackName2 = "Frekkis";
            break;

            // test sykepleier
            case 3:
            attackName1 = "Gal sprøyte";
            attackName2 = "Brukt sprøytespiss";
            break;
        }

        return (attackName1, attackName2);
    }
        
}
