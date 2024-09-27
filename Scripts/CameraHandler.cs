using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class CameraHandler : MonoBehaviour
{
    public CharacterHandler nearestLockOnTarget = null;
    public GameObject currentLockOnTarget = null;


    [Header("(References)"), Space(20)]
    [SerializeField] ControlPlayerState playerState;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform PlayerLockOnTransform;
    [SerializeField] Transform cameraPivot;
    [SerializeField] Transform cameraTransform;

    [Header("(Camera Settings)"), Space(20)]
    [SerializeField] float FollowSpeed = 2f;
    [SerializeField] float rotationSpeed = 7f;
    [SerializeField] float finalX = 0;
    [SerializeField] float finalY = 0;
    [SerializeField] float MaxY = 35;
    [SerializeField] float MinY = -35;


    [Header("(Lock On Settings)"),Space(20)]
    [SerializeField] float ShortestDistance = Mathf.Infinity;
    [SerializeField] float scanRadius = 35;
    [SerializeField] float LockOnFollowSpeed = 0.2f;
    [SerializeField] float MaxDistanceFromPlayer = -50;
    [SerializeField] float min_viewableAngle = -50;
    [SerializeField] float max_viewableAngle = 50;
    [SerializeField] LayerMask LAYERSTODETECT;
    [SerializeField] public List<CharacterHandler> availableTargets = new List<CharacterHandler>();

    public event Action head;
    [Header("HEAD TO TARGET")]
    private Vector3 _vel;
    [SerializeField] AnimatorHandler _anim;
    [SerializeField] float l_Speed = 3;
    [SerializeField] float nearestThreshold = 2;

    private void Start()
    {
         ShortestDistance = Mathf.Infinity;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void FollowPlayer(float delta)
    {
        Vector3 targetDirection = Vector3.Lerp(this.transform.position, playerTransform.position, FollowSpeed * delta); 
         transform.position = targetDirection;
    }

    void HeadToTarget()
    {
        Vector3 dir = playerTransform.position - currentLockOnTarget.GetComponentInParent<Transform>().position;

          
            
                playerTransform.position = Vector3.Lerp(playerTransform.position,  new Vector3(0,0,currentLockOnTarget.transform.position.z)
                    , l_Speed);
            
        

    }
    private void OnEnable()
    {
        head += HeadToTarget;
    }

    private void OnDisable()
    {
        head -= HeadToTarget;
    }
    private void CameraRotation(float delta)
    {

        if (!inputHandler.isLockedOn && currentLockOnTarget == null)
        {
            // UPDATE MOUSE INPUTS
            finalY += (inputHandler.MouseX * rotationSpeed) * delta;
            finalX -= (inputHandler.MouseY * rotationSpeed) * delta;
            finalX = Mathf.Clamp(finalX, MinY, MaxY);

            Vector3 targetRotation = Vector3.zero;
            targetRotation.x = finalX;
            Quaternion Rotation = Quaternion.Euler(targetRotation);
            cameraPivot.localRotation = Rotation;

            targetRotation = Vector3.zero;
            targetRotation.y = finalY;
            Rotation = Quaternion.Euler(targetRotation);
            transform.rotation = Rotation;
        }
        else
        {
            Vector3 rotationDirection;
            Quaternion targetRotation;
 
            // MAIN PLAYER CAMERA OBJECT
            rotationDirection = currentLockOnTarget.transform.position - PlayerLockOnTransform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, LockOnFollowSpeed);



            rotationDirection = currentLockOnTarget.transform.position - PlayerLockOnTransform.position;
            rotationDirection.Normalize();
            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivot.transform.rotation = Quaternion.Slerp(cameraPivot.transform.rotation, targetRotation, LockOnFollowSpeed);

            finalX = transform.eulerAngles.x;
            finalY = transform.eulerAngles.y;
            DistanceFromCurrentTarget();

            if(Input.GetKeyDown(KeyCode.V))
            {
                head?.Invoke();
            }
        }

    }
    private void DistanceFromCurrentTarget()
    {
        float d = Vector3.Distance(playerTransform.position, currentLockOnTarget.transform.position);
        if( d > scanRadius )
        {
            inputHandler.isLockedOn = false;
            ClearLockOnTargets();
        }
    }


    public void LockOn()
    {
        ClearLockOnTargets();
        ShortestDistance = Mathf.Infinity;


        Collider[] colliders = Physics.OverlapSphere(PlayerLockOnTransform.position, scanRadius, LAYERSTODETECT, QueryTriggerInteraction.Ignore);
         for (int i = 0; i < colliders.Length; i++)
         {
             CharacterHandler character = colliders[i].GetComponent<CharacterHandler>();
             if (character != null && character.IsDead == false)
             {
                 Vector3 LockOnDirection = character.transform.position - PlayerLockOnTransform.position;
                 float viewAbleAngle = Vector3.Angle(LockOnDirection, transform.forward);

                 if (viewAbleAngle >= min_viewableAngle && viewAbleAngle <= max_viewableAngle && character.transform.root != playerTransform.root)
                 {
                     RaycastHit hit;
                     if (Physics.Linecast(PlayerLockOnTransform.position, character.LockOnObject.transform.position, out hit, GameManager.instance.EnvironmentLayers))
                     {
                         continue;
                     }
                     else
                     {
                         availableTargets.Add(character);
                     }
                 }
             }
         }

         // Reset ShortestDistance before finding the nearest target

         for (int i = 0; i < availableTargets.Count; i++)
         {
             if (availableTargets[i] != null)
             {
                 float DistanceFromTarget = Vector3.Distance(playerTransform.position, availableTargets[i].transform.position);
                 if (DistanceFromTarget <= ShortestDistance)
                 {
                     ShortestDistance = DistanceFromTarget;
                     nearestLockOnTarget = availableTargets[i];
                 }

             
             }
         }

         
         // Handle the case where there are no available targets
         if (nearestLockOnTarget == null)
         {
             ClearLockOnTargets();
         }
    }


    public void ClearLockOnTargets()
    {
        currentLockOnTarget = null;
        nearestLockOnTarget = null;
        availableTargets.Clear();
    }



    public void cameraHandler(float delta)
    {
        FollowPlayer(delta);
        CameraRotation(delta);
    }

}
