using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    nromal = 0,
    crouch,
    block
}
public class ControlPlayerState : MonoBehaviour
{
    [Header(" Player State ")]
    public PlayerState CurrentState;

    private void Awake()
    {
        CurrentState = PlayerState.nromal;
    }
}
