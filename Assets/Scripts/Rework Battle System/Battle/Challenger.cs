using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Challenger : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] string prefix; //Foreleser, student, rektor osv
    [SerializeField] Sprite sprite;
    [SerializeField] GameController gameController;

    public bool hasLost = false;

    public string Name{
        get {return name;}
    }

    public string Prefix{
        get {return prefix;}
    }

    public Sprite Sprite{
        get {return sprite;}
    }

    public void StartChallengerBattle()
    {
        if(!hasLost)
        {
            var enemyUnits = gameObject.GetComponent<UnitList>();
            gameController.StartExamBattle(enemyUnits, this);
        }
    }
}
