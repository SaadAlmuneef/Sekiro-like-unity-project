using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;


[RequireComponent(typeof(Animator))]

public class AnimatorHandler : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    private ControlPlayerState playerState;
    private HealthHandler healthHandler;
    private InputHandler inputhandler;

    [Header("Animator Parameters ")]
    [SerializeField] string[] BlendTreeParameters = new string[2];

    [Header("Animator Settings")]
    [SerializeField] float DampTime = 0.1f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerState = GetComponentInParent<ControlPlayerState>();
        inputhandler = GetComponent<InputHandler>();    
        healthHandler = GetComponent<HealthHandler>();
        healthHandler.OnDeath += DeathAnimation;
    }

    public void UpdatePlayerAnimator(float delta)
    {
        animator.SetBool("Block", inputhandler.isBlocking);

        ControlBlendTree(inputhandler.isLockedOn ? inputhandler.horizontal : 0,
            inputhandler.isLockedOn ? inputhandler.vertical : inputhandler.movementAmount
            , inputhandler.isSprinting, inputhandler.crouchInput, delta);


        Animations();
        isMovingHandler();
        CrouchTree(inputhandler.crouchInput);
    }

    private void OnDestroy()
    {
        healthHandler.OnDeath -= DeathAnimation;
    }
    private void ControlBlendTree(float horizontal, float vertical, bool isSprinting , bool isCrouching, float delta)
    {
        
        animator.SetBool("Crouch", isCrouching);
        float horizontalAmount = 0;
        float verticalAmount = 0;

        #region horizontal
        if (horizontal > 0 && horizontal <= 0.5f)
        {
            horizontalAmount = 0.5f;
        }
        else if (horizontal > 0.5f && horizontal <= 1)
        {
            horizontalAmount = 1;
        }
        else if (horizontal < 0 && horizontal >= -0.5f)
        {
            horizontal = -0.5f;
        }
        else if (horizontal <= -0.5f && horizontal >= -1)
        {
            horizontalAmount = -1;
        }
        else
        {
            horizontalAmount = 0;
        }
        #endregion

        #region vertical 
        if (vertical > 0 && vertical <= 0.5f)
        {
            verticalAmount = 0.5f;
        }
        else if (vertical > 0.5f && vertical <= 1)
        {
            verticalAmount = 1;
        }
        else if (vertical < 0 && vertical >= -0.5f)
        {
            verticalAmount = -0.5f;
        }
        else if (vertical <= -0.5f && vertical >= -1)
        {
            verticalAmount = -1;
        }
        else
        {
            verticalAmount = 0;
        }
        #endregion

        if (isSprinting)
        {
            verticalAmount = 2;
        }
        animator.SetFloat(BlendTreeParameters[0], horizontalAmount, DampTime, delta);
        animator.SetFloat(BlendTreeParameters[1], verticalAmount, DampTime, delta);
    }


    private void CrouchTree(bool isCrouching)
    {
        // Adjust Player's collider
        if (animator.GetBool("IsInteracting") == true)
        {
            animator.SetBool("Crouch", false);
            return;
        }

        if (isCrouching)
        {
            playerState.CurrentState = PlayerState.crouch;
            animator.SetBool("Crouch", true);
        }
       
    }
    void Animations()
    {
        if (playerState.CurrentState != PlayerState.nromal || animator.GetBool("IsInteracting") == true)
            return;

        DashAnimation();
    }

    public void PlayAnimation(string target)
    {
        animator.SetBool("IsInteracting", true);
        animator.applyRootMotion = true;
        animator.CrossFade(target, 0.2f);
    }

    void PlayAnimationBoolean(string target)
    {
        animator.SetBool("IsInteracting", true);
        animator.applyRootMotion = true;
        animator.SetBool(target, true);
    }
    void DashAnimation()
    {

        if (inputhandler.DashInput)
        {
            if (inputhandler.isLockedOn && !inputhandler.isSprinting)
            {
                if((inputhandler.vertical == 0 && inputhandler.horizontal == 0) || inputhandler.vertical == 1 && inputhandler.horizontal ==0)
                    PlayAnimation("Dash Forward");
                else
                {
                    if(inputhandler.vertical == 1 && inputhandler.horizontal != 0)
                    {
                        if (inputhandler.horizontal == 1)
                        {
                            PlayAnimation("Dash Right");
                        }
                        else if (inputhandler.horizontal == -1)
                        {
                            PlayAnimation("Dash Left");
                        }

                    }
                    else if(inputhandler.vertical == 0 || inputhandler.vertical == -1 && inputhandler.horizontal != 0)
                    {
                        if(inputhandler.horizontal == 1)
                        {
                            PlayAnimation("Dash Right");
                        }
                        else if (inputhandler.horizontal == -1)
                        {
                            PlayAnimation("Dash Left");
                        }
                    }
                    else if (inputhandler.vertical == -1 && inputhandler.horizontal == 0)
                    {
                        PlayAnimation("Dash Back");
                    }


                }

            }
            else if (inputhandler.isLockedOn && inputhandler.isSprinting)
            {
                PlayAnimation("Dash Forward");
            }
            else if(!inputhandler.isLockedOn)
            {
                if (inputhandler.movementAmount > 0)
                {
                    PlayAnimation("Dash Forward");
                }
                else
                {
                    PlayAnimation("BackStep");
                }
            }

        }

        inputhandler.DashInput = false;
        return;

    }

    private void isMovingHandler()
    {
        animator.SetFloat("IsMoving", inputhandler.movementAmount);
    }

    public void DeathAnimation()
    {
        animator.ResetTrigger("Attack 1");
        animator.ResetTrigger("Attack 2");
        animator.ResetTrigger("Attack 2");
        animator.ResetTrigger("Attack 4");
        animator.SetTrigger("Death");
    }

    
}
