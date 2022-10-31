using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum GameState { FreeRoam, Encounter, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] EncounterSystem encounterSystem;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] TitleScreen titleScreen;
    [SerializeField] Camera worldCamera;
    [SerializeField] GameObject spiritUnit;
    [SerializeField] EncounterDialog dialogBox;

    Vector3 spiritPos;

    GameState state;

    private void Start()
    {
        playerController.OnEncountered += StartEncounter;
        encounterSystem.OnEncounterOver += EndEncounter;

        playerController.OnBattle += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        spiritPos = spiritUnit.transform.localPosition;
    }

    void StartEncounter()
    {
        state = GameState.Encounter;
        encounterSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        //Gets a random spirit from MapArea
        var wildSpirit = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildSpirit();

        //Starts encounter while passing in the random spirit from MapArea
        encounterSystem.StartEncounter(wildSpirit);
    }

    void EndEncounter(bool run)
    {
        state = GameState.FreeRoam;
        encounterSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        dialogBox.EnableActionSelector(false);

    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        battleSystem.StartBattle();
    }

    void EndBattle(bool run)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        dialogBox.EnableActionSelector(false);
    }


    private void Update()
    {
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if(state == GameState.Encounter)
        {
            encounterSystem.HandleUpdate();
        }
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
