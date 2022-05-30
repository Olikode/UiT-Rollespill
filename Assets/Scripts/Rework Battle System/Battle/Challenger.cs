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

    public bool Lost{ get; set;}

    public string Name{
        get {return name;}
    }

    public string Prefix{
        get {return prefix;}
    }

    public Sprite Sprite{
        get {return sprite;}
    }

    public void Start()
    {
        Lost = false;
    }

    public void StartChallengerBattle()
    {
        if(!Lost)
        {
            var enemyUnits = gameObject.GetComponent<UnitList>();
            gameController.StartExamBattle(enemyUnits, this);
        }
    }
}
