using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using System;

public class Unit : MonoBehaviour
{
    public int classID;
    public string name;
    public int level;
    public int maxStress;
    public float currentStress;
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

    public void TakeStress(float dmg)
    {
        currentStress += dmg;

        //if stress goes over max
        if (currentStress > maxStress)
        {
            currentStress = maxStress;
        }
    }

    public void HealUnit(float healAmount)
    {
        currentStress -= healAmount;

        //if stress goes under 0
        if (currentStress < 0)
        {
            currentStress = 0;
        }
    }

    public void CalculateHitScore(){
        Random rnd = new Random();
        int roll = rnd.Next(1, 21); // random number 1 - 20

        // if has item with hit bonus, + itemBonus
        this.hitScore = roll + level/2;
    }

    public (float dmg, float heal, int hitModifier, int stun, int sleep, int poison, int protection) UseClassAttack1(
        bool isCritical
    )
    {
        // finds attack1 from the unit's class
        ClassAttack abilityAttack = new ClassAttack();
        var attackInfo = abilityAttack.FindClassAttack1(classID, isCritical);
        float heal = 0;

        // calculates damage
        float dmgModifier = 1.0f + 0.05f * level;
        float dmg = ((float)attackInfo.baseDmg * dmgModifier);

        // calculates heal
        if (attackInfo.baseHeal > 0)
        {
            heal = ((float)attackInfo.baseHeal * dmgModifier);
        }

        // calculates hit score
        Random rnd = new Random();
        int roll = rnd.Next(1, 21); // random number 1 - 20
        hitScore = roll + attackInfo.hitModifier + level / 2;

        // returns damage and effects
        return (
            dmg,
            heal,
            hitScore,
            attackInfo.stun,
            attackInfo.sleep,
            attackInfo.poison,
            attackInfo.protection
        );
    }

    public (float dmg, float heal, int hitModifier, int stun, int sleep, int poison, int protection) UseClassAttack2(
        bool isCritical
    )
    {
        // finds attack1 from the unit's class
        ClassAttack abilityAttack = new ClassAttack();
        var attackInfo = abilityAttack.FindClassAttack2(classID, isCritical);
        float heal = 0;

        // calculates damage
        float dmgModifier = 1.0f + 0.05f * level;
        float dmg = ((float)attackInfo.baseDmg * dmgModifier);

        // calculates heal
        if (attackInfo.baseHeal > 0)
        {
            dmgModifier = 1.0f + 0.05f * level;
            heal = ((float)attackInfo.baseHeal * dmgModifier);
        }

        // calculates hit score
        Random rnd = new Random();
        int roll = rnd.Next(1, 21); // random number 1 - 20
        hitScore = roll + attackInfo.hitModifier + level / 2;

        // returns damage and effects
        return (
            dmg,
            heal,
            hitScore,
            attackInfo.stun,
            attackInfo.sleep,
            attackInfo.poison,
            attackInfo.protection
        );
    }
}
