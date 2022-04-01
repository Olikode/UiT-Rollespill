using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Unit
{
    [SerializeField]
    UnitBase _base;

    [SerializeField]
    int level;

    public UnitBase Base
    {
        get { return _base; }
    }
    public int Level
    {
        get { return level; }
    }

    // current HP
    public int HP { get; set; }

    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }

    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public void Init()
    {
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

        CalculateStats();

        HP = MaxHP;

        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.Speed, 0 },
        };
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.AttackPower * Level) / 10f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.DefensePower * Level) / 10f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 10f) + 5);

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 10f) + 10;
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // TODO: apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[]{1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        if(boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);


        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts){
        foreach (var statBoost in statBoosts){
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int MaxHP { get; private set; }

    public int AttackPower
    {
        get { return GetStat(Stat.Attack); }
    }

    public int DefensePower
    {
        get { return GetStat(Stat.Defense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    /* public int Hit
    {
        get { return Mathf.FloorToInt((Base.Hit * Level) / 10f) + 5; }
    }*/

    public DamageDetails TakeDamage(Move move, Unit attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        var damageDetails = new DamageDetails() { Critical = critical, Fainted = false, };

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
    public float Critical { get; set; }

    // public float type {get; set;}
}
