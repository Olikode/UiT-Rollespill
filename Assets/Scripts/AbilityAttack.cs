using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=System.Random;

public class AbilityAttack : MonoBehaviour
{
    Random rnd = new Random();

    // ability attack 1
    public (int baseDmg, int hitModifier, int stun, int sleep, int poison, int protection) FindAbilityAttack1(int classID, bool isCritical){

        // attack damage and hit and rounds effects lasts
        int baseDmg = 0; // base damage
        int hitModifier = 0; // bonus for the attack to hit
        int stun = 0; // number of rounds defender is stuned
        int sleep = 0; // number of rounds defender is sleeping
        int poison = 0; // number of rounds defender is poisoned
        int protection = 0; // number of hitpoints on defenders protection


        switch(classID){

            // test datateknikk
            case 1:

            if(isCritical){
                baseDmg = 2;
                hitModifier = 50;
                stun = 3;

                break;
            }
            
            baseDmg = rnd.Next(3); // base dmg 0-2
            int stunChance = rnd.Next(3); // 0-1
            hitModifier = 2;
            if(stunChance > 0){
                stun = rnd.Next(1,3); // stuned 1-2 rounds
            }
            break;

            // test enemy
            case 2:
            baseDmg = 5;
            break;
            

            default:
            break;
        }

        Debug.Log("baseDmg: " + baseDmg);
        return (baseDmg, hitModifier, stun, sleep, poison, protection);
    }

    // ability attack 2
     public (int baseDmg, int hitModifier, int stun, int sleep, int poison, int protection) FindAbilityAttack2(int classID, bool isCritical){

        // attack damage and hit and rounds effects lasts
        int baseDmg = 0; // base damage
        int hitModifier = 0; // bonus for the attack to hit
        int stun = 0; // number of rounds defender is stuned
        int sleep = 0; // number of rounds defender is sleeping
        int poison = 0; // number of rounds defender is poisoned
        int protection = 0; // number of hitpoints on defenders protection


        switch(classID){

            // test datateknikk
            case 1:
            if(isCritical){
                baseDmg = 7;
                hitModifier = 50;
                break;
            }

            baseDmg = rnd.Next(4, 8); // base dmg 4-7
            hitModifier = 5;
            
            break;

            case 2:
            //other class attack
            break;
            

            default:
            break;
        }
        return (baseDmg, hitModifier, stun, sleep, poison, protection);
    }
}
