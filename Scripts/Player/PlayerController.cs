using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Animator animator;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;
    public LayerMask battleTrigger;

    public event Action OnEncountered;
    public event Action OnBattle;

    [SerializeField] GameObject WildEncounter;
    [SerializeField] GameObject Battle;
    public GameObject MainCamera;
    public GameObject EncounterCamera;
    public GameObject BattleCamera;
    
    private bool isMoving;
    private Vector2 input;


    public void HandleUpdate() //called once per frame
    {
        if(!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal"); //sets input.x to -1 or 1 if either left or right is pressed
            input.y = Input.GetAxisRaw("Vertical"); //sets input.y to -1 or 1 if either down or up is pressed

            if (input.x != 0) input.y = 0; //removes diagonal movement

            if(input != Vector2.zero) //if an input was pressed...
            {
                animator.SetFloat("moveX", input.x); //updates animator variables when input is pressed
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position; //set targetPos to current position (for now)
                targetPos.x += input.x; //set targetPos.x to current position plus the horizontal input
                targetPos.y += input.y; //set targetPos.y to current position plus the vertical input

                if (Walkable(new Vector3(targetPos.x, targetPos.y - 0.5f))) //if the target position is walkable
                    StartCoroutine(Move(targetPos)); //triggers the moving coroutine
            }
        }

        animator.SetBool("isMoving", isMoving); //tells the animator that our player is moving
    }

    IEnumerator Move(Vector3 targetPos) //function that moves player from current position to target position
    {
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) //checks if targetPos is NOT current position
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime); //changes current position to targetPos
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

        CheckEncounters();
        CheckBattle();
    }

    private bool Walkable(Vector3 targetPos) //function for checking collisions
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void CheckEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }
    }

    private void CheckBattle()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, battleTrigger) != null)
        {
            animator.SetBool("isMoving", false);
            OnBattle();
        }
    }
}
