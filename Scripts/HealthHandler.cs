using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
[RequireComponent(typeof(UIManager))]

public class HealthHandler : MonoBehaviour, IDamageable
{
    // Private References
    private UIManager ui_manager;
    private AnimatorHandler animatorHandler;
    private VECTORS v_vector;
    private InputHandler inputHandler;


    [Header("Character UI"),Space(10)]
    [SerializeField] Slider healthBar;
    [SerializeField] YouDiedSc YouDiedUI;


    [Header("Character Status Asset "),Space(10)]
    [SerializeField] CharacterStatus characterStatus;


    public bool IsDead;


    [Header("Character Status"),Space(10)]
    [SerializeField] float currentHealth = 0;
    [SerializeField] int currentSpell = 0;


    [Header("Damage Animations "),Space(20)]
    [SerializeField] string RightHit_state;
    [SerializeField] string LeftHit_state;
    [SerializeField] string forwardHit_state;
    [SerializeField] string backwardHit_state;
    [SerializeField] string Death_state;

    public bool Hitable
    { 
        get;
        private set;
    }

    // EVENTS
    public event Action OnDeath;


    private void Awake()
    {
        ui_manager = GetComponent<UIManager>();
        animatorHandler = GetComponent<AnimatorHandler>();
        v_vector = GetComponent<VECTORS>();
        inputHandler = GetComponent<InputHandler>();
        Init();
    }

    public void HandleHealth()
    {
        Hitable = animatorHandler.animator.GetBool("CanGetDamage");
        UpdateHealth();
    }

    void Init()
    {
        healthBar.maxValue = characterStatus.MaxHealth;
        currentHealth = characterStatus.MaxHealth;
        currentSpell = characterStatus.IntSpell;
    }

    void UpdateHealth()
    {
        ui_manager.UpdateImageBar(ref healthBar, currentHealth);
        DrinkSpell();
    }

  
    void DrinkSpell()
    {

    }

    public void TakeDamage(Transform target , float Amount)
    {

        Vector3 playerPosition = GameManager.instance.thePlayer.transform.position;
        Vector3 targetPosition = target.position;

        // Transform target position into player's local space
        Vector3 targetRelativePosition = GameManager.instance.thePlayer.transform.InverseTransformPoint(targetPosition);

        float horizontalDistance = targetRelativePosition.x;
        float verticalDistance = targetRelativePosition.z; // Assuming a 2D game in the XZ plane

        string HitAnimation;

        if (Mathf.Abs(horizontalDistance) > Mathf.Abs(verticalDistance))
        {
            HitAnimation = (horizontalDistance > 0) ? RightHit_state : LeftHit_state;
        }
        else
        {
            HitAnimation = (verticalDistance > 0) ? forwardHit_state : backwardHit_state;
        }

        if(animatorHandler.animator.GetBool("CanGetDamage") == true)
        {
            if(currentHealth  <= 0)
            {
                IsDead = true;
                Die();
            }
            else
            {
                currentHealth -= Amount;
                animatorHandler.PlayAnimation(HitAnimation);
                inputHandler.crouchInput = false;

            }
        }
    }

  

    public void Die()
    {
        if (currentHealth <= 0)
        {
            YouDiedUI.DisplayYouDied();
            OnDeath?.Invoke();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

        }
    }

    
}
