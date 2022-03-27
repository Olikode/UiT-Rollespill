using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit1
{
    public UnitBase Base {get; set;}
    public int Level {get; set;}

    // current HP
    public int HP {get; set;}

    public List<Move> Moves {get; set;}

    public Unit1(UnitBase pBase, int pLevel)
    {
        Base = pBase;
        Level = pLevel;
        HP = MaxHP;


        // Generates moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if(move.Level <= Level){
                Moves.Add(new Move(move.Base));
            }

            if(Moves.Count >= 4)
                break;
        }
    }

    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 10f) + 10; }
    }
    public int AttackPower
    {
        get { return Mathf.FloorToInt((Base.AttackPower * Level) / 10f) + 5; }
    }
    public int DefensePower
    {
        get { return Mathf.FloorToInt((Base.DefensePower * Level) / 10f) + 5; }
    }
    public int Dodge
    {
        get { return Mathf.FloorToInt((Base.Dodge * Level) / 10f) + 5; }
    }
    public int Hit
    {
        get { return Mathf.FloorToInt((Base.Hit * Level) / 10f) + 5; }
    }
}
