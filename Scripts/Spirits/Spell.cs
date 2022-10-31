using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Spirit/Create new spell")]

public class Spell : ScriptableObject
{
    [SerializeField] new string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] SpellClass spellClass;
    [SerializeField] SpiritType type;

    [SerializeField] int power;
    [SerializeField] int accuracy;

    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public SpiritType Type
    {
        get { return type; }
    }
    public int Power
    {
        get { return power; }
    }
    public int Accuracy
    {
        get { return accuracy; }
    }
    public SpellClass SpellClass
    {
        get { return spellClass; }
    }
}

public enum SpellClass
{
    Attack,
    Heal,
    AtkBuff,
    AtkDebuff,
    DefBuff,
    DefDebuff,
    AgiBuff,
    AgiDebuff,
}
