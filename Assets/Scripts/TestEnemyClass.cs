using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;

public class TestEnemyClass : TestUnit
{

    private string attackName1 = "Lurespørsmål";
    private string attackName2 = "Bonus-oppgave";


    public float FiendeAttack1(){

        Random rnd = new Random();
        int baseDmg = rnd.Next(1,4);

        return baseDmg*this.CalculateDmgNodifier();
    }

    public float FiendeAttack2(){

        Random rnd = new Random();
        int baseDmg = rnd.Next(6);

        return baseDmg*this.CalculateDmgNodifier();
    }
}
