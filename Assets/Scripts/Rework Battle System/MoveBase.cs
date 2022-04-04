using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Unit/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField]
    string name;

    [TextArea]
    [SerializeField]
    string description;

    [SerializeField]
    ClassType type;

    [SerializeField]
    MoveCategory category;

    [SerializeField]
    int power;

    [SerializeField]
    int accuracy;

    [SerializeField]
    int pp;

    [SerializeField]
    MoveEffects effects;

    [SerializeField]
    MoveTarget target;

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
    public int Accuracy
    {
        get { return accuracy; }
    }
    public int PP
    {
        get { return pp; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public MoveTarget Target
    {
        get { return target; }
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
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Normal,
    Status,
}

public enum MoveTarget
{
    Self,
    Enemy
}
