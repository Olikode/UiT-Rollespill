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
    [SerializeField] List<ItemBase> rewards;

    [SerializeField] List<string> startDialog;
    [SerializeField] List<string> endPlayerWonDialog;
    [SerializeField] List<string> endPlayerLostDialog;

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

    public List<ItemBase> Rewards{
        get {return rewards;}
    }

    public List<string> StartDialog{
        get {return startDialog;}
    }
    public List<string> EndPlayerLostDialog{
        get {return endPlayerLostDialog;}
    }
    public List<string> EndPlayerWonDialog{
        get {return endPlayerWonDialog;}
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
            var gameController = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();

            gameController.StartExamBattle(enemyUnits, this);
        }
    }
}
