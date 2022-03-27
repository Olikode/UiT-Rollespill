using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class EnemyAttack : MonoBehaviour
{
    Random rnd = new Random();

    // attack damage and hit and rounds effects lasts
    int baseDmg = 0; // base damage
    int baseHeal = 0;
    int hitModifier = 0; // bonus for the attack to hit
    int stun = 0; // number of rounds defender is stuned
    int sleep = 0; // number of rounds defender is sleeping
    int poison = 0; // number of rounds defender is poisoned
    int protection = 0; // number of hitpoints on defenders protection

    // class attack 1
    public (int baseDmg, int baseHeal, int hitModifier, int stun, int sleep, int poison, int protection) FindEnemyAttack1(
        int enemyID,
        bool isCritical
    )
    {
        switch (enemyID)
        {
            // MATTEPRØVE
            case 1:

                if (isCritical)
                {
                    baseDmg = 5;
                    hitModifier = 50;
                    stun = 3;

                    break;
                }

                baseDmg = rnd.Next(6); // base dmg 0-5
                int stunChance = rnd.Next(10); // 0-9
                hitModifier = 3;
                if (stunChance > 9)
                {
                    stun = rnd.Next(1, 4); // stuned 1-3 rounds
                }
                break;

            // 
            case 2:
                break;

            default:
                break;
        }
        return (baseDmg, baseHeal, hitModifier, stun, sleep, poison, protection);
    }

    // class attack 2
    public (int baseDmg, int baseHeal, int hitModifier, int stun, int sleep, int poison, int protection) FindEnemyAttack2(
        int enemyID,
        bool isCritical
    )
    {
        // attack damage and hit and rounds effects lasts
        int baseDmg = 0; // base damage
        int baseHeal = 0;
        int hitModifier = 0; // bonus for the attack to hit
        int stun = 0; // number of rounds defender is stuned
        int sleep = 0; // number of rounds defender is sleeping
        int poison = 0; // number of rounds defender is poisoned
        int protection = 0; // number of hitpoints on defenders protection

        switch (enemyID)
        {
            // MATTEPRØVE
            case 1:
                if (isCritical)
                {
                    baseDmg = 5;
                    hitModifier = 50;
                    break;
                }

                baseDmg = rnd.Next(2, 6); // base dmg 2-5
                hitModifier = 5;

                break;

            //
            case 2:
                
                break;

            //
            case 3:
                
                break;

            default:
                break;
        }
        return (baseDmg, baseHeal, hitModifier, stun, sleep, poison, protection);
    }

    public (string attackName1, string attackName2) FindAttackName(int enemyID)
    {
        string attackName1 = "";
        string attackName2 = "";
        string attackDescription1 = "";
        string attackDescription2 = "";
        // attack description should be added later

        switch (enemyID)
        {
            // Matteprøve
            case 1:
                attackName1 = "Lurespørsmål";
                attackName2 = "Fuck";
                attackDescription1 = "";
                attackDescription1 = "";
                break;

            // SYKEPLEIER
            case 2:
                break;

            //
            case 3:

                break;
        }

        return (attackName1, attackName2);
    }
}
