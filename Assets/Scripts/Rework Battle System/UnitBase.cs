using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create new unit")]

public class UnitBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] ClassType type;

    [SerializeField] Sprite sprite;

    //Base stats
    [SerializeField] int maxHP;
    [SerializeField] int attackPower;
    [SerializeField] int defensePower;
    [SerializeField] int dodge;
    [SerializeField] int hit;
    [SerializeField] int initiative;

    // Unlock more moves when leveling up
    [SerializeField] List<LearnableMove> learnableMoves;


    public string Name {
        get { return name;}
    }

    public string Description {
        get { return description;}
    }

    public Sprite Sprite {
        get { return sprite;}
    }

    public int MaxHP {
        get { return maxHP;}
    }

    public int AttackPower {
        get { return attackPower;}
    }

    public int DefensePower {
        get { return defensePower;}
    }

    public int Dodge {
        get { return dodge;}
    }

    public int Hit {
        get { return hit;}
    }

    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
    }
}

[System.Serializable]
public class LearnableMove{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;
    
    public MoveBase Base{
        get {return moveBase;}
    }
    public int Level{
        get {return level;}
    }
}

public enum ClassType{

    // player types
    Student,
    Datateknikk,
    Sykepleier,
    Elektro,
    Bygg,

    // enemy types
    Matte,
    Fysikk,
    Oppgave,
    Eksamen,
}