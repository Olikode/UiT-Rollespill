using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField] UnitBase _base;
    [SerializeField] int level;

    public UnitBase Base { 
        get {
            return _base;
        }
     }
    public int Level { 
        get {
            return level;
        }
     }

    // current HP
    public int HP { get; set; }

    public List<Move> Moves { get; set; }

    public void Init()
    {
        HP = MaxHP;

        // Generates moves
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }

            if (Moves.Count >= 4)
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

    public DamageDetails TakeDamage(Move move, Unit attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        var damageDetails = new DamageDetails(){
            Critical = critical,
            Fainted = false,
        };

        float modifiers = Random.Range(0.85f, 1f) * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.AttackPower / DefensePower) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical {get; set;}

    // public float type {get; set;}
}
