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
    // public float dodge;
    public bool isStuned = false;
    public bool isAsleep = false;

    public int initiative;
    public int critChance;
    public int dodgeScore = 10;
    public int hitScore = 0;


    


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
        float dmg = ((float)attackInfo.baseDmg * dmgModifier);

        Debug.Log("dmg: " + dmg);
        // returns damage and effects
        return (dmg, attackInfo.isStuned, attackInfo.isAsleep);
    }

        public (float dmg, bool isStuned, bool isAsleep) UseAbilityAttack2(){

        AbilityAttack abilityAttack = new AbilityAttack();
        var attackInfo = abilityAttack.FindAbilityAttack2(classID);

        float dmgModifier = 1.0f + 0.05f * level;
        float dmg = ((float)attackInfo.baseDmg * dmgModifier);

        Debug.Log("dmg: " + dmg);

        return (dmg, attackInfo.isStuned, attackInfo.isAsleep);
    }

    public void SetInitiative(){
        Random rnd = new Random();
        int roll = rnd.Next(21); // random number 0 - 20

        initiative = roll;
    }

    /*public void CalculateDodgeScore(){
        Random rnd = new Random();
        int roll = rnd.Next(11); // random number 0 - 25

        // if has item with doge bonus, + itemBonus
        this.dodgeScore = roll;
        Debug.Log("DODGE: " + roll);
    }*/
    public void CalculateHitScore(){
        Random rnd = new Random();
        int roll = rnd.Next(21); // random number 0 - 100

        // if has item with hit bonus, + itemBonus
        this.hitScore = roll;
        Debug.Log("HIT: " + roll);
    }

    public (string attackName1, string attackName2) FindAttackName(int classID){

        string attackName1 = "";
        string attackName2 = "";

        switch(classID){

            // test datateknikk
            case 1:
            attackName1 = "Hack Attack";
            attackName2 = "Nerd Power";
            break;


            case 2:
            attackName1 = "Slem slag";
            attackName2 = "Frekkis";
            break;
        }

        return (attackName1, attackName2);
    }
        
}