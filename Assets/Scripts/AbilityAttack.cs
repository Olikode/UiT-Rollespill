using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=System.Random;

public class AbilityAttack : MonoBehaviour
{
    Random rnd = new Random();
    
    public (int baseDmg, bool isStuned, bool isAsleep) FindAbilityAttack1(int classID){

        // attack damage and effects
        int baseDmg = 0;
        bool stun = false;
        bool sleep = false;


        switch(classID){

            // testclass
            case 1:
            baseDmg = rnd.Next(3); // base dmg 0-2

            stun = false;
            int stunChance = rnd.Next(2);
            if(stunChance == 1){
                stun = true;
            }
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
