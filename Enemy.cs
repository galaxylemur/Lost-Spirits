using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Spirit/Create new enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] SpiritType type;
    [SerializeField] int hp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int agility;
    [SerializeField] Spell[] spells;
    [SerializeField] SpiritType[] weaknesses;

    public string Name
    {
        get { return name; }
    }
    public Sprite Sprite
    {
        get { return sprite; }
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
    public Spell[] Spells
    {
        get { return spells; }
    }
}
