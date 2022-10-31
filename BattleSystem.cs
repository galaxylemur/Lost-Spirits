using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum BattleState { Busy, PlayerAction, SpellSelect, EnemyAction }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] Spirit[] spiritParty;
    [SerializeField] Image[] spiritUnit;
    [SerializeField] Text[] spiritName;
    [SerializeField] GameObject[] spiritHealth;
    [SerializeField] Text[] spellSlots;
    [SerializeField] Enemy enemy;
    [SerializeField] Image enemyUnit;
    [SerializeField] EncounterDialog dialogBox;
    [SerializeField] Image black;
    [SerializeField] GameObject blackScreen;
    [SerializeField] GameObject gameOver;
    public event Action<bool> OnBattleOver;
    BattleState state;
    int chosenSpell;
    int activeSpirit;
    int enemyHealth;
    int s0hp;
    int s1hp;
    int s2hp;
    float s0percent;
    float s1percent;
    float s2percent;

    public void StartBattle() //this is called by gamecontroller btw (why doesnt it just call setup....)
    {
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        state = BattleState.Busy;

        //setup variables
        //these should be for loops at some point just for efficiency (and to account for an empty party)
        spiritUnit[0].sprite = spiritParty[0].FrontSprite;
        spiritUnit[1].sprite = spiritParty[1].FrontSprite;
        spiritUnit[2].sprite = spiritParty[2].FrontSprite;

        spiritName[0].text = spiritParty[0].Name;
        spiritName[1].text = spiritParty[1].Name;
        spiritName[2].text = spiritParty[2].Name;

        s0hp = spiritParty[0].Hp;
        s1hp = spiritParty[1].Hp;
        s2hp = spiritParty[2].Hp;
        s0percent = 1f;
        s1percent = 1f;
        s2percent = 1f;

        enemyHealth = enemy.Hp;
        enemyUnit.sprite = enemy.Sprite;

        yield return StartCoroutine(dialogBox.TypeDialog($"{enemy.Name} appeared!"));

        //play enter animation
        //these animations kinda lag a bit?
        var startAnimation = DOTween.Sequence();
        startAnimation.Join(spiritUnit[0].transform.DOLocalMoveX(-252, 1f));
        startAnimation.Join(spiritUnit[1].transform.DOLocalMoveX(-300, 1f));
        startAnimation.Join(spiritUnit[2].transform.DOLocalMoveX(-420, 1f));
        startAnimation.Join(enemyUnit.transform.DOLocalMoveX(179, 1f));
        yield return startAnimation.WaitForCompletion();

        //start battle
        
        yield return new WaitForSeconds(1f);
        activeSpirit = 0; //starting spirit is just first one in line, would be cool to base it on agility tho

        StartCoroutine(PlayerAction());
    }

    public void HandleUpdate()
    {

    }

    IEnumerator PlayerAction()
    {
        //called at the start of the battle and end of enemy turn
        yield return StartCoroutine(dialogBox.TypeDialog($"What will {spiritParty[activeSpirit].Name} do?"));
        yield return new WaitForSeconds(1f);
        dialogBox.EnableActionSelector(true);
        state = BattleState.PlayerAction; //states are kinda useless rn
    }

    IEnumerator EnemyAction()
    {
        int enemySpell = UnityEngine.Random.Range(0, enemy.Spells.Length);
        state = BattleState.EnemyAction;

        if(enemy.Spells[enemySpell].SpellClass == SpellClass.Attack) //if enemy uses an attack
        {
            int targetedSpirit = UnityEngine.Random.Range(0, 3); //enemy just picks a random spirit rn, aggro rates would be cool tho
            yield return dialogBox.TypeDialog($"{enemy.Name} uses {enemy.Spells[enemySpell].Name} on {spiritParty[targetedSpirit].Name}!");

            yield return enemyUnit.transform.DOPunchPosition(new Vector3(-100, 0, 0), 0.5f, 0, 0); //enemy attack animation
            yield return spiritUnit[targetedSpirit].transform.DOPunchPosition(new Vector3(10, 10, 0), 0.5f, 10, 1); //spirit hit animation

            yield return new WaitForSeconds(1f);

            //find what spirit is being targeted to change correct values
            //all three outputs here are pretty much the same, just change different values to correspond with the targeted spirit
            if(targetedSpirit == 0)
            {
                int damageTaken = (enemy.Spells[enemySpell].Power + enemy.Attack - spiritParty[targetedSpirit].Defense); //determines how much dmg the attack did
                s0hp = (s0hp - damageTaken); //updates hp
                s0percent = (s0percent - 0.25f); //lol...
                spiritHealth[targetedSpirit].transform.localScale = new Vector3(s0percent, 1, 1); //changes health bar
                yield return dialogBox.TypeDialog($"{spiritParty[targetedSpirit].Name} took {damageTaken} damage!"); //displays value to player
                yield return new WaitForSeconds(1f);
            }
            else if(targetedSpirit == 1)
            {
                int damageTaken = (enemy.Spells[enemySpell].Power + enemy.Attack - spiritParty[targetedSpirit].Defense);
                s1hp = (s1hp - damageTaken);
                s1percent = (s1percent - 0.25f);
                spiritHealth[targetedSpirit].transform.localScale = new Vector3(s1percent, 1, 1);
                yield return dialogBox.TypeDialog($"{spiritParty[targetedSpirit].Name} took {damageTaken} damage!");
                yield return new WaitForSeconds(1f);
            }
            else if(targetedSpirit == 2)
            {
                int damageTaken = (enemy.Spells[enemySpell].Power + enemy.Attack - spiritParty[targetedSpirit].Defense);
                s2hp = (s2hp - damageTaken);
                s2percent = (s2percent - 0.25f);
                spiritHealth[targetedSpirit].transform.localScale = new Vector3(s2percent, 1, 1);
                yield return dialogBox.TypeDialog($"{spiritParty[targetedSpirit].Name} took {damageTaken} damage!");
                yield return new WaitForSeconds(1f);
            }

            //player's turn!
            StartCoroutine(PlayerAction());
        }
        else if(enemy.Spells[enemySpell].SpellClass == SpellClass.AtkBuff) //if enemy uses an attack buffing spell (aka belligerence lol)
        {
            //just tells the player that they used an attack buff
            yield return dialogBox.TypeDialog($"{enemy.Name} uses {enemy.Spells[enemySpell].Name}!");
            yield return enemyUnit.transform.DOJump(enemyUnit.transform.position, 2, 1, 0.5f);
            yield return new WaitForSeconds(1f);
            yield return dialogBox.TypeDialog($"{enemy.Name}'s attack power increased!");
            yield return new WaitForSeconds(1f);

            //player's turn!
            StartCoroutine(PlayerAction());
        }
    }

    public void SpellSelection()
    {
        spellSlots[0].text = spiritParty[activeSpirit].Spells[0].Spell.Name; //should be a for loop, just displays 2 spells for now
        spellSlots[1].text = spiritParty[activeSpirit].Spells[1].Spell.Name;
    }

    //these are all seperate functions so they can be called by the buttons for each spell on click
    public void ChooseFirstSpell()
    {
        chosenSpell = 0;
        StartCoroutine(CastSpell());
    }

    public void ChooseSecondSpell()
    {
        chosenSpell = 1;
        StartCoroutine(CastSpell());
    }

    public void ChooseThirdSpell()
    {
        chosenSpell = 2;
        StartCoroutine(CastSpell());
    }

    public void ChooseFourthSpell()
    {
        chosenSpell = 3;
        StartCoroutine(CastSpell());
    }


    IEnumerator CastSpell()
    {
        yield return dialogBox.TypeDialog($"{spiritParty[activeSpirit].Name} uses {spiritParty[activeSpirit].Spells[chosenSpell].Spell.Name}!");
        yield return new WaitForSeconds(1f);

        if(spiritParty[activeSpirit].Spells[chosenSpell].Spell.SpellClass == SpellClass.Attack) //if chosen spell is an attack...
        {
            enemyHealth = (enemyHealth - (spiritParty[activeSpirit].Spells[chosenSpell].Spell.Power + spiritParty[activeSpirit].Attack - enemy.Defense));
            Debug.Log($"Health remaining: {enemyHealth}");

            //attack animation
            yield return spiritUnit[activeSpirit].transform.DOPunchPosition(new Vector3(100, 0, 0), 0.5f, 0, 0);
            yield return enemyUnit.transform.DOPunchPosition(new Vector3(20, 20, 0), 0.5f, 10, 1);

            yield return dialogBox.TypeDialog($"{enemy.Name} took {enemy.Hp - enemyHealth} damage!");
            yield return new WaitForSeconds(1f);
        }
        else if(spiritParty[activeSpirit].Spells[chosenSpell].Spell.SpellClass == SpellClass.AtkBuff) //if chosen spell is an attack buff (aka belligerence lol)
        {
            yield return spiritUnit[activeSpirit].transform.DOJump(spiritUnit[activeSpirit].transform.position, 1, 1, 0.5f); //spirit cast animation
            yield return dialogBox.TypeDialog($"Party attack power increased!");
            yield return new WaitForSeconds(1f);
            //an animation here would be cool (maybe the party jumping or turning red or something)
        }
        else if(spiritParty[activeSpirit].Spells[chosenSpell].Spell.SpellClass == SpellClass.DefDebuff) //if chosen spell is a defense debuff (aka frail bark lol)
        {
            yield return spiritUnit[activeSpirit].transform.DOJump(spiritUnit[activeSpirit].transform.position, 1, 1, 0.5f); //spirit cast animation
            yield return dialogBox.TypeDialog($"{enemy.Name}'s defense decreased!");
            yield return new WaitForSeconds(1f);
            //an animation here would be cool
        }
        

        if(enemyHealth <= 0) //player win condition 1/1
        {
            yield return dialogBox.TypeDialog($"{enemy.Name} fainted!");
            yield return enemyUnit.DOFade(0f, 1f);
            yield return new WaitForSeconds(1f);
            yield return dialogBox.TypeDialog("You are victorious!");

            //battle should normally end here, this is just for demo purposes:
            blackScreen.SetActive(true);
            yield return new WaitForSeconds(2f);
            yield return black.DOFade(255f, 3f);
            yield return new WaitForSeconds(1f);
            gameOver.SetActive(true);

        }
        else
        {
            //updates which spirit gets next turn (just a loop rn)
            if(activeSpirit < 2)
            {
                activeSpirit += 1;
            }
            else
            {
                activeSpirit = 0;
            }

            //enemy turn!
            StartCoroutine(EnemyAction());
        }
    }
}
