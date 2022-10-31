using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum EncounterState { Start, PlayerAction, SpiritAction, Busy }

public enum PlayerAction { Catch, Run }

public class EncounterSystem : MonoBehaviour
{
    [SerializeField] Spirit spirit;
    [SerializeField] Text nameText;
    [SerializeField] Text typeText;
    [SerializeField] Image spiritUnit;
    [SerializeField] EncounterDialog dialogBox;
    [SerializeField] GameObject dialogBoxUI;
    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject EncounterCamera;
    [SerializeField] GameObject WildEncounter;
    [SerializeField] GameObject bottle;
    public Animator bottleAnim;

    public event Action<bool> OnEncounterOver;

    EncounterState state;
    int currentAction;
    Vector3 spiritPos;

    //Called on the start of an encounter
    public void StartEncounter(Spirit wildSpirit)
    {
        //Plugs wildSpirit from MapArea into the spirit field at the top
        spirit = wildSpirit;
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        nameText.text = spirit.Name;
        typeText.text = spirit.Type.ToString() + " Type";

        //resets spirit pos (so they dont fly off the screen lol..)
        spiritUnit.sprite = spirit.FrontSprite;
        spiritPos = spiritUnit.transform.localPosition = new Vector3(0, 0, 0);
        spiritUnit.transform.localScale = new Vector3(2, 2, 1);

        //entrance animation
        spiritUnit.DOFade(1, 0.5f);
        spiritUnit.transform.localPosition = new Vector3(500f, spiritPos.y);
        spiritUnit.transform.DOLocalMoveX(spiritPos.x, 1f);

        yield return StartCoroutine(dialogBox.TypeDialog($"A wild {spirit.Name} appeared!"));
        yield return new WaitForSeconds(1f);

        StartCoroutine(PlayerAction());
    }

    IEnumerator PlayerAction()
    {   
        yield return StartCoroutine(dialogBox.TypeDialog("What will you do?"));
        yield return new WaitForSeconds(1f);
        dialogBox.EnableActionSelector(true);
        state = EncounterState.PlayerAction;
    }

    public void HandleUpdate() //runs every frame but only in an encounter (called by game controller)
    {
        //this function was mainly used for testing, gonna keep it here in case we need it though
    }


    IEnumerator HandleSpiritAction()
    {
        state = EncounterState.Busy;

        if(UnityEngine.Random.Range(1, 101) <= spirit.FleeRate)
        {
            //Spirit ran away and encounter ends
            spiritUnit.DOFade(0, 2f);
            yield return dialogBox.TypeDialog($"{spirit.Name} ran away!");
            yield return new WaitForSeconds(2f);

            //this applies to game controller
            OnEncounterOver(true);
        }
        else
        {
            //Spirit waits and player gets another turn
            int spiritWaitText = UnityEngine.Random.Range(1, 4);

            //random splash text bc why not
            if ((spiritWaitText) == 1)
            {
                yield return dialogBox.TypeDialog($"{spirit.Name} is waiting...");
            }
            else if ((spiritWaitText) == 2)
            {
                yield return dialogBox.TypeDialog($"{spirit.Name} does a funny little dance.");
            }
            else if ((spiritWaitText) == 3)
            {
                yield return dialogBox.TypeDialog($"{spirit.Name} is getting belligerent!");
            }

            yield return new WaitForSeconds(2f);
            state = EncounterState.PlayerAction;
            StartCoroutine(PlayerAction());
        }
    }

    public void Run() //used by button
    {
        StartCoroutine(RunAction());
    }

    public void Catch() //used by button
    {
        StartCoroutine(CatchSpirit());
    }

    private IEnumerator RunAction()
    {
        state = EncounterState.Busy;
        yield return dialogBox.TypeDialog("You ran away!");
        yield return new WaitForSeconds(2f);
        OnEncounterOver(true);
    }

    IEnumerator CatchSpirit()
    {
        state = EncounterState.Busy;

        yield return dialogBox.TypeDialog("You try to catch the Spirit!");
        yield return new WaitForSeconds(1f);

        //Bottle throw animation
        var bottleObject = Instantiate(this.bottle, dialogBox.transform.position - new Vector3(-7, 2), Quaternion.identity);
        var bottle = bottleObject.GetComponent<SpriteRenderer>();
        var bottleAnim = bottleObject.GetComponent<Animator>();
        var bottleThrow = DOTween.Sequence();
        bottleThrow.Join(bottle.transform.DOJump(spiritUnit.transform.position + new Vector3(0, -2), 5f, 1, 1));
        bottleThrow.Join(bottle.transform.DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.WorldAxisAdd));
        yield return bottleThrow.WaitForCompletion();

        //Spirit catching animation
        bottleAnim.SetBool("catchingSpirit", true);
        var catchSequence = DOTween.Sequence();
        catchSequence.Append(spiritUnit.DOFade(0, 0.5f));
        catchSequence.Join(spiritUnit.transform.DOLocalMoveY(spiritPos.y + 50f, 0.5f));
        catchSequence.Join(spiritUnit.transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return catchSequence.WaitForCompletion();

        int shakeCount = TryToCatchSpirit(spirit); //this is kinda extra but it works

        for (int i=0; i< Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return bottle.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            //Spirit was caught
            dialogBoxUI.SetActive(true);
            yield return dialogBox.TypeDialog($"You caught the {spirit.Name}!");
            yield return bottle.DOFade(0, 1.5f).WaitForCompletion();
            yield return new WaitForSeconds(2f);

            Destroy(bottle);
            OnEncounterOver(true);
        }
        else
        {
            //Spirit breaks out
            yield return new WaitForSeconds(1f);
            bottle.DOFade(0, 0.2f);
            var breakoutSequence = DOTween.Sequence();
            breakoutSequence.Append(spiritUnit.DOFade(1, 0.5f));
            breakoutSequence.Join(spiritUnit.transform.DOLocalMoveY(spiritPos.y, 0.5f));
            breakoutSequence.Join(spiritUnit.transform.DOScale(new Vector3(2f, 2f, 1f), 0.5f));
            yield return breakoutSequence.WaitForCompletion();

            dialogBoxUI.SetActive(true); //is it ever not set active?
            yield return dialogBox.TypeDialog("The Spirit broke free!");
            yield return new WaitForSeconds(2f);

            Destroy(bottle);

            state = EncounterState.SpiritAction;
            StartCoroutine(HandleSpiritAction());
        }
    }

    int TryToCatchSpirit(Spirit spirit)
    {
        if (UnityEngine.Random.Range(1, 101) <= spirit.CatchRate)
        {
            return 4;
        }
        else
        {
            if (UnityEngine.Random.Range(1, 4) == 3)
            {
                return 3;
            }
            else if (UnityEngine.Random.Range(1, 4) == 2)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
    }
}
