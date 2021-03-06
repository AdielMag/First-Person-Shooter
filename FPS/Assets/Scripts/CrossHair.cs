﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public float spreadSensetivity, spreadSpeedMultiplier;
    float fireSpeedMultiplier;
    float targetSpread;

    float currentHide, targetHide;

    public float CurrentSpread { get; private set; }
    public bool  Aiming { get; set; }
    public bool  OnTarget { get; set; }
    public bool  CanInteract { get; set; }


    float cameraRotationVelocity, movementVelocity;

    Animator anim;

    #region Singelton
    static public CrossHair instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        targetSpread += targetSpread > 4f ? cameraRotationVelocity : cameraRotationVelocity + movementVelocity;
        targetSpread = Mathf.Clamp(targetSpread, 0, 10);


        fireSpeedMultiplier = Mathf.Lerp(fireSpeedMultiplier, 1, Time.deltaTime * 100);
        targetSpread = Mathf.Lerp(targetSpread, 0, Time.deltaTime * spreadSpeedMultiplier * fireSpeedMultiplier);
        CurrentSpread = Mathf.Lerp(CurrentSpread, targetSpread, Time.deltaTime * spreadSensetivity);

        targetHide = Aiming ? .5f : OnTarget ? -.5f : 0;
        currentHide = Mathf.Lerp(currentHide, targetHide, Time.deltaTime * 20);

        CanInteract = Aiming || OnTarget ? false : CanInteract;

        anim.SetFloat("Spread", CurrentSpread);
        anim.SetFloat("Hide", currentHide);
        anim.SetBool("CanInteract", CanInteract);
    }

    public void GetInput(float cameraRotationInput, float movementInput)
    {
        cameraRotationVelocity = cameraRotationInput;
        movementVelocity = movementInput;
    }

    public void AddSpread(float amount)
    {
        fireSpeedMultiplier = 5;
        targetSpread = 1 + targetSpread + amount;
    }
}
