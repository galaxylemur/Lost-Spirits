using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spirit", menuName = "Spirit/Create new spirit")]
public class Spirit : ScriptableObject
{
    [SerializeField] new string name;

    [SerializeField] Sprite frontSprite;

    [SerializeField] SpiritType type;

    [SerializeField] int hp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int agility;
    [SerializeField] int catchRate;
    [SerializeField] int fleeRate;

    [SerializeField] List<LearnedSpells> spells;
    
    public string Name
    {
        get { return name; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public SpiritType Type
    {
        get { return type; }
    }

    public int Hp
    {
        get { return hp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int Agility
    {
        get { return agility; }
    }

    public int CatchRate
    {
        get { return catchRate; }
    }
    public int FleeRate
    {
        get { return fleeRate; }
    }
    public List<LearnedSpells> Spells
    {
        get { return spells; }
    }
}

[System.Serializable]
public class LearnedSpells
{
    [SerializeField] Spell spell;

    public Spell Spell
    {
        get { return spell; }
    }
}

public enum SpiritType
{
    None,
    Plant,
    Fairy,
    Aqua,
    Geo,
    Fire
}