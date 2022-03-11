using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=System.Random;

public class AbilityAttack : MonoBehaviour
{
    Random rnd = new Random();

    // ability attack 1
    public (int baseDmg, bool isStuned, bool isAsleep) FindAbilityAttack1(int classID){

        // attack damage and effects
        int baseDmg = 0;
        bool stun = false;
        bool sleep = false;


        switch(classID){

            // test datateknikk
            case 1:
            baseDmg = rnd.Next(3); // base dmg 0-2
            Debug.Log("in case 1 dmg: " + baseDmg);
            stun = false;
            int stunChance = rnd.Next(3); // 0-2
            if(stunChance >= 1){
                stun = true;
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
        return (baseDmg, stun, sleep);
    }

    // ability attack 2
     public (int baseDmg, bool isStuned, bool isAsleep) FindAbilityAttack2(int classID){

        // attack damage and effects
        int baseDmg = 0;
        bool stun = false;
        bool sleep = false;


        switch(classID){

            // test datateknikk
            case 1:
            baseDmg = rnd.Next(4, 8); // base dmg 4-7

            break;


            case 2:
            //other class attack
            break;
            

            default:
            break;
        }
        return (baseDmg, stun, sleep);
    }
}
