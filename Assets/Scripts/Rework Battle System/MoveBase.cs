using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Unit/Create new move")]
public class MoveBase : ScriptableObject
{
    [Header("Name and description")]
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] ClassType type;

    [SerializeField] MoveCategory category;

    [Header("Stats")]
    [SerializeField] int power;

    [SerializeField] int accuracy;
    [SerializeField] bool alwaysHit;

    [SerializeField] int pp;
    [SerializeField] int priority;

    [Header("Heal")]
    [SerializeField] int healPower;
    [SerializeField] HealType healType;

    [Header("Effects")]
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;
    [SerializeField] SecondaryEffects secondaryEffects;


    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public ClassType Type
    {
        get { return type; }
    }
    public MoveCategory Category
    {
        get { return category; }
    }
    public int Power
    {
        get { return power; }
    }
    public int HealPower
    {
        get { return healPower; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public bool AlwaysHit
    {
        get { return alwaysHit; }
    }
    public int PP
    {
        get { return pp; }
    }
    public int Priority
    {
        get { return priority; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public HealType HealType
    {
        get { return healType; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }

    public SecondaryEffects SecondaryEffects
    {
        get { return secondaryEffects; }
    }

}

[System.Serializable]
public class MoveEffects
{
    [SerializeField]
    List<StatBoost> boosts;

    [SerializeField]
    ConditionID status;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }

    public ConditionID Status
    {
        get {return status;}
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {
        get {return chance;}
    }

    public MoveTarget Target {
        get {return target;}
    }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Normal,
    Status,
    Heal, // when attack should not do any damage to enemy
}

public enum HealType
{
    Null,
    Percentage,
    Drain,
}

public enum MoveTarget
{
    Self,
    Enemy
}
