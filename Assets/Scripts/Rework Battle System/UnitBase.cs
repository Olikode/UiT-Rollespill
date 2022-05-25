using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "Unit", menuName = "Unit/Create new unit")]

public class UnitBase : ScriptableObject
{
    [Header("Apparence")]
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] ClassType type;
    [SerializeField] Sprite sprite;

    [Header("Base stats")]
    [SerializeField] int maxHP;
    [SerializeField] int attackPower;
    [SerializeField] int defensePower;
    [SerializeField] int speed;


    [Header("EXP and loot")]
    [SerializeField] int expYield;
    // TODO add SerializeField list with possible lootdrops

    public static int MaxNumOfMoves {get; set;} = 4;

    [Header("Moves")]
    // Unlock more moves when leveling up
    [SerializeField] List<LearnableMove> learnableMoves;


    public int GetExpForLevel(int level)
    {
        // formula for calculating exp gain
        // TODO: rebalance when game is close to finished
        return (9 *(level * level * level) / 5);
    }

    // TODO add method
    // should calculate possible loot
    /*public ItemBase GetLootDrop()
    {
        
    }*/

    public string Name { 
        get {return name;} 
        set {this.name = value;} }

    public string Description {
        get { return description;}
        set {this.description = value; }
    }

    public ClassType Type {
        get { return type;}
        set {this.type = value; }
    }

    public Sprite Sprite {
        get { return sprite;}
        set {this.sprite = value; }
    }

    public int MaxHP {
        get { return maxHP;}
        set {this.maxHP = value; }
    }

    public int AttackPower {
        get { return attackPower;}
        set {this.attackPower = value; }
    }

    public int DefensePower {
        get { return defensePower;}
        set {this.defensePower = value; }
    }

    public int Speed {
        get { return speed;}
        set {this.speed = value; }
    }

    public List<LearnableMove> LearnableMoves {
        get { return learnableMoves; }
        set {this.learnableMoves = value; }
    }

    public int ExpYield{
        get {return expYield;}
        set {this.expYield = value; }
    }
    
    public void SetPlayerCharacter(UnitBase unitBase, string name, Image image)
    {
        Name = name;
        Description = unitBase.Description;
        Type = unitBase.Type;
        Sprite = image.sprite;
        MaxHP = unitBase.MaxHP;
        AttackPower = unitBase.AttackPower;
        DefensePower = unitBase.DefensePower;
        Speed = unitBase.Speed;
        ExpYield = unitBase.ExpYield;
        LearnableMoves = unitBase.LearnableMoves;
    }

    // used when player has selected character
    // Moves need to be set
    public List<Move> MovesAtFirstLevel()
    {
        var moves = new List<Move>();
        foreach (var move in LearnableMoves)
        {
            if (move.Level <= 1)
            {
                moves.Add(new Move(move.Base));
            }

            if (moves.Count >= UnitBase.MaxNumOfMoves)
                break;
        }

        return moves;
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

public enum Stat{
    Angrep,
    Forsvar,
    Hurtighet,
    Treffsikkerhet,
    Unnvikelse,
}