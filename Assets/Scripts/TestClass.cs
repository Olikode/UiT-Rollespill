using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;


public class TestClass : TestUnit
{

    private string attackName1 = "Hack Attack";
    private string attackName2 = "Nerd Rage";

    // Calculate attack power of ability attack
    public float HackAttack(){

        // add stun effect later

        Random rnd = new Random();
        int baseDmg = rnd.Next(3);

        return baseDmg*this.CalculateDmgNodifier();
    }

    public float NerdRage(){

        Random rnd = new Random();
        int baseDmg = rnd.Next(2,7);

        return baseDmg*this.CalculateDmgNodifier();
    }
}
