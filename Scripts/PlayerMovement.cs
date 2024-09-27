using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header(" References ")]
    [HideInInspector] public CharacterController controller;
    [SerializeField] CameraHandler cameraHandler;
    [SerializeField] Transform CameraObj;
    private InputHandler inputHandler;
    private AnimatorHandler animatorHandler;



    [Header("Player Settings")]
    public Vector3 PlayerDirection = Vector3.zero;
    [SerializeField] public float movementSpeed = 0;
    [SerializeField] float sprintSpeed = 0;
    [SerializeField] float rotationSpeed = 0;
    [SerializeField] Vector3 lastForward;

    [Header("Gravity")]
    [SerializeField] float gravityValue = 9f;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponent<AnimatorHandler>();
    }

    private void OnDrawGizmos()
    {
         lastForward = PlayerDirection.magnitude > 0f ? transform.forward : lastForward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(this.transform.position, lastForward * 4);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(this.transform.position  , transform.forward * 4);
    }

    public float CurrentSpeed()
    {
        float speed = 0;
        if (inputHandler.crouchInput)
        {
            speed = 2;
            return speed;
        }
        else
        {
            speed = (inputHandler.isBlocking) ? 2f : (inputHandler.isSprinting) ? sprintSpeed : movementSpeed;
            return speed;

        }
    }

    private void ApplyGravity()
    {
        PlayerDirection.y -= gravityValue;
    }


    private void RotatePlayer(float delta)
    {
        if (inputHandler.isLockedOn)
        {

            if (inputHandler.isSprinting)
            {
                Vector3 targetDirection = Vector3.zero;

                targetDirection = CameraObj.transform.right * inputHandler.horizontal;
                targetDirection += CameraObj.forward * inputHandler.vertical;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                    targetDirection = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion RotateToTarget = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
                transform.rotation = RotateToTarget;

            }
            else
            {
                if (inputHandler.isLockedOn)
                {
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    Quaternion RotateToTarget = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
                    transform.rotation = RotateToTarget;

                }
            }

        }
        else
        {
            Vector3 targetDirection = Vector3.zero;

            targetDirection = CameraObj.transform.right * inputHandler.horizontal;
            targetDirection += CameraObj.forward * inputHandler.vertical;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion RotateToTarget = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
            transform.rotation = RotateToTarget;

        }
       

    }
    private void MovePlayer(float delta)
    {
        // CHECK IF THE PLAYER IS GROUNDED TO MOVE HIM OR ROTATE HIM
        PlayerDirection = CameraObj.transform.right * inputHandler.horizontal;
        PlayerDirection += CameraObj.forward * inputHandler.vertical;
        PlayerDirection.Normalize();
        PlayerDirection.y = 0;
        ApplyGravity();
        controller.Move(PlayerDirection * CurrentSpeed() * delta);
    }


    public void PlayerHandler(float delta)
    {
        if(animatorHandler.animator.GetBool("canMove") == true)
        MovePlayer(delta);
        if (animatorHandler.animator.GetBool("canRotate") == true)
            RotatePlayer(delta);
    }





    
 }

 