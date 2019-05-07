using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public float spreadSensetivity, spreadSpeedMultiplier;
    float fireSpeedMultiplier;
    float currentSpread, targetSpread;

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
        GetInput(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude);

        targetSpread += targetSpread > 2.5f ? cameraRotationVelocity : cameraRotationVelocity + movementVelocity;
        targetSpread = Mathf.Clamp(targetSpread, 0, 10);


        fireSpeedMultiplier = Mathf.Lerp(fireSpeedMultiplier, 1, Time.deltaTime * 100);
        targetSpread = Mathf.Lerp(targetSpread, 0, Time.deltaTime * spreadSpeedMultiplier * fireSpeedMultiplier);
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * spreadSensetivity);

        anim.SetFloat("Spread", currentSpread);
    }

    public void GetInput(float cameraRotationInput, float movementInput)
    {
        cameraRotationVelocity = cameraRotationInput;
         //new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")).magnitude;
        movementVelocity = movementInput;
         //new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;
    }

    public void AddSpread(float amount)
    {
        fireSpeedMultiplier = 5;
        targetSpread = 1 + targetSpread + amount;
    }
}
