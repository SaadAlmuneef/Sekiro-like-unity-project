using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    public InputHandler inputHandler;
    public PlayerMovement Playermovement;
    public CameraHandler CameraHandler;
    public AnimatorHandler animatorHandler;
    public CombatHandler combatHandler;
    public HealthHandler healthHandler;

    public bool AbleToDeathBlow = false;
    public bool executedDeathBlow = false;
    public GameObject PlayerLockOnObject;

    void Update()
    {
        float delta = Time.deltaTime;

        inputHandler.PlayerInputsHandler();
        Playermovement.PlayerHandler(delta);
        animatorHandler.UpdatePlayerAnimator(delta);
        combatHandler.ComboHandler();
        healthHandler.HandleHealth();

       // Time.timeScale = 0.2f;
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;
        CameraHandler.cameraHandler(delta);
    }
}
