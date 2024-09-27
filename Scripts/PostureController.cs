using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PostureController : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager;
    [SerializeField] GameObject PostureBarHightLight;
    [SerializeField] Animator PostureBarHighLightAnimator;


    [SerializeField] GameObject postureBar;
    [SerializeField] RectTransform RT;
    [SerializeField] Image image;


    [SerializeField] float CurrentWidth = 0;
    [SerializeField] float MaxWidth = 560;
    [SerializeField] float lerpS = 0.02f;
    [SerializeField] bool FirstTimeGotBroken = true;

    private void Start()
    {
        FirstTimeGotBroken = true;
        PostureBarHighLightAnimator = PostureBarHightLight.GetComponent<Animator>();    
    }
    private void Update()
    {
        PostureHandler();
    }

    private void PostureHandler()
    {

        CurrentWidth = Mathf.Clamp(CurrentWidth, 0, MaxWidth);
        RT.sizeDelta = Vector2.Lerp(RT.sizeDelta, new Vector2(CurrentWidth, 64), lerpS);
        if(CurrentWidth > 0)
        {
            transform.localScale = Vector3.one;
        }

        if(CurrentWidth == MaxWidth && FirstTimeGotBroken)
        {
            OnPostureBroken();
            playerManager.AbleToDeathBlow = true;
            FirstTimeGotBroken = false;
        }

        if(playerManager.executedDeathBlow)
        {
            CurrentWidth = 0;
            transform.localScale = Vector3.zero;
            playerManager.AbleToDeathBlow = false;
            FirstTimeGotBroken = true;
        }
        StartCoroutine(HidePostureAfterDamage());
    }
    private void OnPostureBroken()
    {
      PostureBarHightLight.SetActive(true);
      PostureBarHighLightAnimator.SetTrigger("Display_postureBarHighlight"); 
    }
    public void IncreasePosture(float amount)
    {
        CurrentWidth += amount;
    }


    IEnumerator HidePosture()
    {
        yield return new WaitForSeconds(2);
        transform.localScale = Vector3.zero;
    }

    IEnumerator HidePostureAfterDamage()
    {
        float initialV = CurrentWidth;
        yield return new WaitForSeconds(5);
        if(initialV == CurrentWidth && playerManager.AbleToDeathBlow == false)
        {
            CurrentWidth = 0;
            transform.localScale = Vector3.zero;
        }
    }

}
