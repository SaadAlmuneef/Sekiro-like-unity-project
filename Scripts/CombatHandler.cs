using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CombatHandler : MonoBehaviour
{
    [SerializeField] PostureController postureController;

    private InputHandler inputHandler;
    private AnimatorHandler animatorHandler;
    private ControlPlayerState playerState;
    private WeaponHandler weaponHandler;

    [Header("Animator Parameters")]
    [SerializeField] string [] ATTACK_TRIGGERS; // Attack Triggers
    [SerializeField] string [] Deflect_States; // Deflection Stataes
    [SerializeField] string [] Block_States; // Guard Stataes

    [SerializeField] int DeflectionCounter = 0;
    [SerializeField] int GuardCounter = 0;

    
    public bool Hitable
    {
        get;
        private set;
    }

    [Header("Combo Settings")]
    [SerializeField]  int comboCounter = 0;
    

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponent<AnimatorHandler>();

        playerState = GetComponentInParent<ControlPlayerState>();
        weaponHandler = GetComponentInChildren<WeaponHandler>();

        comboCounter = 0;
        DeflectionCounter = 0;


    }

    private void OnEnable()
    {
        weaponHandler.OnDeflect += Deflect;

        weaponHandler.OnBlock += Block;

    }
    private void OnDisable()
    {
        weaponHandler.OnDeflect -= Deflect;
        weaponHandler.OnBlock -= Block;
    }


    public void ComboHandler()
    {
        // Reset Guard State
        if (!inputHandler.isBlocking)
            GuardCounter = 0;

        Hitable = animatorHandler.animator.GetBool("CanGetDamage");
        animatorHandler.animator.SetInteger("comboCounter", comboCounter);
        Attack();
        Blocking();
    }

    private void GetAttackState(ref int counter)
    {
        if (comboCounter > ATTACK_TRIGGERS.Length - 1)
        {
            comboCounter = 0;
        }
        animatorHandler.animator.SetTrigger(ATTACK_TRIGGERS[counter]);
        comboCounter++;

    }
    private void Attack()
    {
        if (animatorHandler.animator.GetBool("IsInteracting") == true || playerState.CurrentState != PlayerState.nromal)
            return;

        if(comboCounter > 3)
        {
            comboCounter = 0;
        }
        if (inputHandler.AttackInput)
        {
            //inputHandler.movementInput = Vector2.zero;
            inputHandler.AttackInput = false;
            animatorHandler.animator.SetTrigger(ATTACK_TRIGGERS[comboCounter]);
            comboCounter++;
        }
    }

    private void Blocking()
    {
        if (playerState.CurrentState == PlayerState.crouch || animatorHandler.animator.GetBool("IsInteracting") == true)
            return;


            if (inputHandler.isBlocking)
            {
                    playerState.CurrentState = PlayerState.block;              
            }
            else
            {
                if (playerState.CurrentState != PlayerState.crouch)
                {
                    playerState.CurrentState = PlayerState.nromal;
                }
            }
        

    }

    private void Deflect()
    {
      
        if (Hitable == false)
            return;

        TargetDeflectionState();
        postureController.IncreasePosture(70);
    }

    private void TargetDeflectionState()
    {
        if (DeflectionCounter == 2)
            DeflectionCounter = 0;

          animatorHandler.PlayAnimation(Deflect_States[DeflectionCounter]);
          DeflectionCounter++;
    }


    

    private void PlayTargetGuardState()
    {
        if (GuardCounter == 2)
            GuardCounter = 0;

        animatorHandler.PlayAnimation(Block_States[GuardCounter]);

        GuardCounter++;
    }
    private void Block()
    {
        if (Hitable == false)
            return;

        PlayTargetGuardState();
        postureController.IncreasePosture(200);
    }

}
