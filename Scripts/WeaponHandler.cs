using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class WeaponHandler : MonoBehaviour, IWeapon
{
    [Header("References ")]
    [SerializeField] ControlPlayerState playerState;
    private AnimatorHandler animatorHandler;
    private AudioSource WeaponAudioSource;


    [Header("Katana Asset")]
    [SerializeField] private WeaponData weaponData;
    public ParticleSystem weaponParticleSystem;
    public GameObject ParticleSystemParent;
    

    [Header("Weapon Values")]
    public string WeaponName;
    public float WeaponDamage = 0;
    public float MaxPosture = 0;
     
    public string current_tag;
    public BoxCollider WeaponCollider;



    [Header("Weapon Collider")]
    [SerializeField] float Base_X;
    [SerializeField] float Base_Y;
    [SerializeField] float Base_Z;  

    [SerializeField] float Block_X;
    [SerializeField] float Block_Y;
    [SerializeField] float Block_Z;

    // Events
    public delegate void Combo();
    public event Combo OnBlock;
    public event Combo OnDeflect;

    public bool Hitable
    {
        get;
        private set;
    }

    void Awake()
    {
        Init();
        WeaponAudioSource = GetComponent<AudioSource>();
    }
   

    private void Start()
    {
        Hitable = animatorHandler.animator.GetBool("CanGetDamage");
        WeaponCollider.enabled = false; 
    }
    void Update()
    {

    }


    void Init()
    {
        animatorHandler = GetComponentInParent<AnimatorHandler>();
        this.WeaponName = weaponData.WeaponName;
        this.WeaponDamage = weaponData.WeaponDamage;
        this.MaxPosture = weaponData.MaxPostureAmount;
        WeaponCollider = this.GetComponent<BoxCollider>();
    }

  

   
   public void HandleDeflection()
   {     
          OnDeflect?.Invoke();
          Debug.Log("Weapon Deflection");  
    }

    public void HandleBlocking()
    {
        
            OnBlock?.Invoke();
            Debug.Log("Blocking");
        

    }

    
   public void PlayWeaponParticles(Vector3 hitPosition)
   {
        GameObject ParticlesObject = Instantiate(ParticleSystemParent, hitPosition, Quaternion.identity);
        weaponParticleSystem = ParticlesObject.GetComponent<ParticleSystem>();
        PointLightDef pointLightAnimator = weaponParticleSystem.GetComponentInChildren<PointLightDef>();

        pointLightAnimator.AnimatePointLightIntensity();
        weaponParticleSystem.Play();


        Destroy(ParticlesObject, 1.5f);
        Destroy(pointLightAnimator, 1.5f);
   }

}
