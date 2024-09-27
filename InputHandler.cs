using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;


public class InputHandler : MonoBehaviour
{ 
    PlayerInputs Playerinputs;
    ControlPlayerState controlPlayerState;
    AnimatorHandler animatorHandler;


    [Header("References"), Space(20)]
    [SerializeField] CameraHandler cameraHandler;
     
    [Header("Input Values"),Space(20)]
    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public Vector2 cameraInput;
    [HideInInspector] public float movementAmount = 0;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;
    /*[HideInInspector]*/ public float MouseX;
    /*[HideInInspector]*/ public float MouseY;
    private bool _sprintInput = false;
    private bool _crouch = false;
    private bool _dash = false;
    private bool _blockInput = false;
    private bool _lockOnInput = false;


    [Header("Player Actions"),Space(20)]
    public bool DashInput = false;
    public bool AttackInput = false;
    public bool crouchInput = false;
    public bool isSprinting = false;
    public bool isBlocking = false;
    public bool isLockedOn = false;
    public bool IsAttacking = false;

    [Header("HOLDING TIME"), Space(20)]
    [SerializeField] float finaTime = 1;
    [SerializeField] double currentTime = 0;
    [SerializeField] bool ForwardMouse = false;
    [SerializeField] bool start = false;
    [SerializeField] bool ReachFinalTime = false;
    void Awake()
    {
        isLockedOn = false;
        _lockOnInput = false;
        controlPlayerState = GetComponentInParent<ControlPlayerState>();
        animatorHandler = GetComponentInParent<AnimatorHandler>();
        if (Playerinputs == null)
        {
            Playerinputs = new PlayerInputs();
            Playerinputs.Axises.movement.performed += _ => movementInput = _.ReadValue<Vector2>();
            Playerinputs.Axises.camera.performed += _  => cameraInput = _.ReadValue<Vector2>();

            Playerinputs.Actions.Sprint.performed += _  => _sprintInput = true;
            Playerinputs.Actions.Sprint.canceled += _  => _sprintInput = false;


            Playerinputs.Actions.Block.performed += _ => _blockInput = true;
            Playerinputs.Actions.Block.canceled += _ => _blockInput = false;

            Playerinputs.Actions.Dash.performed += _  => _dash = true;


            Playerinputs.Actions.Attack.started += _ => AttackInput = true;
            Playerinputs.Actions.Attack.canceled += _ => AttackInput = false;


            Playerinputs.Actions.Crouch.performed += _ => _crouch = true;
            Playerinputs.Actions.Crouch.canceled += _ => _crouch = false;

            Playerinputs.Actions.LockOn.started += _ => _lockOnInput = true;


             Playerinputs.Actions.Hold.performed += OnHold;
        }


    }

    void OnHold(InputAction.CallbackContext i)
    {
       ForwardMouse = true;
    }

    


    private void OnEnable()
    {
        Playerinputs.Enable();
    }

    private void OnDisable()
    {
        Playerinputs.Disable();
    }

    public void PlayerInputsHandler()
    {
        MovementInput();
        dashInput();
        Block();
        Sprint();
        Crouch();
        LockOnInput();
        ReturnToNormalState();
    }

    private void MovementInput()
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        movementAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        MouseX = cameraInput.x;
        MouseY = cameraInput.y;
    }
  
    private void Sprint()
    {
        if (controlPlayerState.CurrentState == PlayerState.block || controlPlayerState.CurrentState == PlayerState.crouch 
            || movementAmount == 0)
        {
            isSprinting = false;
            return;
        }


        isSprinting = _sprintInput;
    }

    private void Block()
    {
        if (controlPlayerState.CurrentState == PlayerState.crouch)
        {
            isBlocking  = false;
            return;
        }
     
            isBlocking = _blockInput;
    }

   

    private void ReturnToNormalState()
    {
        if (!crouchInput && !AttackInput)
            controlPlayerState.CurrentState = PlayerState.nromal;
    }
    private void dashInput()
    {
        if(controlPlayerState.CurrentState == PlayerState.nromal)
        {
            DashInput = _dash;
        }
        _dash = false;
    }
    private void Crouch()
    {
        if (controlPlayerState.CurrentState == PlayerState.block)
            return;

        if (_crouch)
        {
            if (!crouchInput)
            {
                crouchInput = true;
            }
            else if (crouchInput)
            {
                crouchInput = false;
            }
            _crouch = false;
        }
    }


    private void LockOnInput()
    {

        if(_lockOnInput)
        {
            cameraInput = Vector2.zero;
            if (!isLockedOn)
            {
                cameraHandler.ClearLockOnTargets();
                cameraHandler.LockOn();
                if(cameraHandler.nearestLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget.LockOnObject;
                    isLockedOn = true;
                }
             }
            else
            {
                cameraHandler.ClearLockOnTargets();
                isLockedOn = false;
            }

            _lockOnInput = false;
        }
    }
    private void Update()
    {
        
         

    }


}

