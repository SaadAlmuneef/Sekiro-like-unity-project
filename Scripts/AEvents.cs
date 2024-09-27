using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AEvents : MonoBehaviour
{
    private Animator animator;
    private WeaponHandler weaponHandler;
    private AudioSource WeaponAudioSource;
    private AudioSource PlayerAudioSource;
    private InputHandler _input;
    [Header("Camera Shake")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] float shakeDuration = 1;
    public bool Start_deflect = false;
    public bool Start_guard = false;

    [Header("Animation Curve")]
    [SerializeField] AnimationCurve deflectAnimationCurve = null;
    [SerializeField] AnimationCurve GuardAnimationCurve = null;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponHandler = GetComponentInChildren<WeaponHandler>();
        WeaponAudioSource = weaponHandler.GetComponent<AudioSource>();
        PlayerAudioSource = this.GetComponent<AudioSource>();
        _input = GetComponent<InputHandler>();
    }


    [SerializeField] 
    void SetIsInteracting(int value01)
    {
        animator.SetBool("IsInteracting",Convert.ToBoolean(value01));
    }
    [SerializeField]
    void DisableIsAttacking()
    {
        _input.IsAttacking = false;
    }

    [SerializeField]
    void SET_CAN_DEFENSE_WHILE_ATTACKING(int value01)
    {
        animator.SetBool("CanDefenseWhileAttacking", Convert.ToBoolean(value01));
    }

    [SerializeField]
    void SetCanMove(int value01)
    {
        animator.SetBool("canMove", Convert.ToBoolean(value01));
    }

    [SerializeField]
    void SetCanRotate(int value01)
    {
        animator.SetBool("canRotate", Convert.ToBoolean(value01));
    }

    [SerializeField]
    void SetCanAttack(int value01)
    {
        animator.SetBool("canAttack", Convert.ToBoolean(value01));
    }

    [SerializeField]
    void ColliderState(int value01)
    {
        weaponHandler.WeaponCollider.enabled = Convert.ToBoolean(value01);
    }

    [SerializeField]
    void SetWeaponTag(string targetTag)
    {
        weaponHandler.tag = targetTag;
    }

    [SerializeField]
    void Play_WeaponAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            WeaponAudioSource.clip = clip;
            WeaponAudioSource.Play();
        }
    }

    [SerializeField]
    void Play_PlayerAudioClip(AudioClip clip)
    {
        if (clip != null)
        {
            PlayerAudioSource.clip = clip;
            PlayerAudioSource.Play();
        }
    }



    [SerializeField]
    void DeflectShake(int startShakeVlaue01)
    {

        Start_deflect = Convert.ToBoolean(startShakeVlaue01);
       if(Start_deflect)
       {
            StartCoroutine(Shake(deflectAnimationCurve , Start_deflect));
       }

    }


    [SerializeField]
    void BlockShake(int startShakeVlaue01)
    {

        Start_guard = Convert.ToBoolean(startShakeVlaue01);
        if (Start_guard)
        {
            StartCoroutine(Shake(GuardAnimationCurve , Start_guard));
        }


    }

    [SerializeField]
    void TimeScale(float value)
    {
        Time.timeScale = value;
    }

    IEnumerator Shake(AnimationCurve targetCurve , bool startShaking)
    {
        Vector3 startPos = cameraTransform.position;

        float elapsedTime = 0;

        while(elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float strength = targetCurve.Evaluate(elapsedTime / shakeDuration);
            cameraTransform.position =  startPos + UnityEngine.Random.insideUnitSphere * strength;


            yield return null;
        }

        cameraTransform.position = startPos;
        startShaking = false;
    }
}
