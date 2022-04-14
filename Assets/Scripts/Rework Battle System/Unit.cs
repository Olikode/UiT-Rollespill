using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Move CurrentMove {get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }

    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    public Condition Status { get; set; }

    public int StatusTime { get; set; }

    public bool HpChanged { get; set; }

    public event System.Action OnStatusChanged;

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

        ResetStatBoost();
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Angrep, Mathf.FloorToInt((Base.AttackPower * Level) / 10f) + 5);
        Stats.Add(Stat.Forsvar, Mathf.FloorToInt((Base.DefensePower * Level) / 10f) + 5);
        Stats.Add(Stat.Hurtighet, Mathf.FloorToInt((Base.Speed * Level) / 10f) + 5);

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 10f) + 10 + Level;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.Angrep, 0 },
            { Stat.Forsvar, 0 },
            { Stat.Hurtighet, 0 },
            { Stat.Treffsikkerhet, 0 },
            { Stat.Unnvikelse, 0 },
        };
    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        // TODO: apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name} sitt {stat}-nivå økte");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name} sitt {stat}-nivå minsket");
            }

            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}");
        }
    }

    public int MaxHP { get; private set; }

    public int AttackPower
    {
        get { return GetStat(Stat.Angrep); }
    }

    public int DefensePower
    {
        get { return GetStat(Stat.Forsvar); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Hurtighet); }
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

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHP);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null)
            return;

        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
    }

    public bool OnBeforeMove()
    {
        if (Status?.OnBeforeMove != null)
        {
            return Status.OnBeforeMove(this);
        }

        return true;
    }

    public Move GetRandomMove()
    {
        // currently this will give an error if enemy is out of moves with PP
        // TODO: add a move the unit does if out of moves with PP
        // or make sure the enemies always have a move with lots of PP
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public void OnBattleOver()
    {
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }

    // public float type {get; set;}
}
