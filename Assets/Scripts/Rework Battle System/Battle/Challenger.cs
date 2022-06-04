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
    [SerializeField] List<ItemBase> rewards;

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
