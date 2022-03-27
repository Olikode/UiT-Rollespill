using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class ClassAttack : MonoBehaviour
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
    public (int baseDmg, int baseHeal, int hitModifier, int stun, int sleep, int poison, int protection) FindClassAttack1(
        int classID,
        bool isCritical
    )
    {
        switch (classID)
        {
            // DATATEKNIKK
            case 1:

                if (isCritical)
                {
                    baseDmg = 3;
                    hitModifier = 50;
                    stun = 3;

                    break;
                }

                baseDmg = rnd.Next(4); // base dmg 0-3
                int stunChance = rnd.Next(1,3); // 1-2
                hitModifier = 2;
                if (stunChance > 0)
                {
                    stun = rnd.Next(2, 5); // stuned 1-4 rounds
                }
                break;

            // SYKEPLEIER
            case 2:

                if (isCritical)
                {
                    baseDmg = 1;
                    hitModifier = 50;
                    sleep = 5;

                    break;
                }

                baseDmg = 1;
                int sleepChance = rnd.Next(3); // 0-1
                hitModifier = 5;
                if (sleepChance > 0)
                {
                    sleep = rnd.Next(2, 6); // stuned 2-5 rounds
                }
                break;

            default:
                break;
        }
        return (baseDmg, baseHeal, hitModifier, stun, sleep, poison, protection);
    }

    // class attack 2
    public (int baseDmg, int baseHeal, int hitModifier, int stun, int sleep, int poison, int protection) FindClassAttack2(
        int classID,
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

        switch (classID)
        {
            // DATATEKNIKK
            case 1:
                if (isCritical)
                {
                    baseDmg = 7;
                    hitModifier = 50;
                    break;
                }

                baseDmg = rnd.Next(4, 8); // base dmg 4-7
                hitModifier = 5;

                break;

            // SYKEPLEIER
            case 2:
                //other class attack
                break;

            //
            case 3:
                //other class attack
                break;

            default:
                break;
        }
        return (baseDmg, baseHeal, hitModifier, stun, sleep, poison, protection);
    }

    public (string attackName1, string attackName2) FindAttackName(int classID)
    {
        string attackName1 = "";
        string attackName2 = "";
        string attackDescription1 = "";
        string attackDescription2 = "";
        // attack description should be added later

        switch (classID)
        {
            // DATATEKNIKK
            case 1:
                attackName1 = "Hack Attack";
                attackName2 = "Nerdekraft";
                attackDescription1 = " Hack fienden din.\nGjør mellom 0 og 2 skade.\nDet har også en sjanse til å paralysere fienden.";
                attackDescription1 = " Bruk dine indre nerdekrefter.\nGjør mellom 4 og 7 skade.";
                break;

            // SYKEPLEIER
            case 2:
                attackName1 = "Gal sprøyte";
                attackName2 = "Brukt sprøytespiss";
                break;

            // 
            case 3:
                
                break;
        }

        return (attackName1, attackName2);
    }
}
