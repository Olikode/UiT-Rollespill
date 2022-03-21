using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using System;

public class EnemyUnit : MonoBehaviour
{
    public int enemyID;
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

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;

        //if hp goes under 0
        if (currentHP < 0)
        {
            currentHP = 0;
        }
    }

    public void HealUnit(float healAmount)
    {
        currentHP += healAmount;

        //if hp goes over max
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void CalculateHitScore(){
        Random rnd = new Random();
        int roll = rnd.Next(1, 21); // random number 1 - 20

        // if has item with hit bonus, + itemBonus
        this.hitScore = roll + level/2;
    }

    public (float dmg, float heal, int hitModifier, int stun, int sleep, int poison, int protection) UseEnemyAttack1(
        bool isCritical
    )
    {
        // finds attack1 from the unit's class
        EnemyAttack abilityAttack = new EnemyAttack();
        var attackInfo = abilityAttack.FindEnemyAttack1(enemyID, isCritical);
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

}
