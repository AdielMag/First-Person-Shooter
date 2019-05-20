﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool aim, fire, reload; // Commands states.
    [HideInInspector]
    public InteractiveItem interactive;
    [HideInInspector]
    public bool attachmentMenu;
    bool pressedAttachmentMenuOnce;

    #region Camera Rotation Variables

    public float cameraRotationSpeed = 5;
    public float mouseSensitivity = 2;

    Vector3 rotationSmoothVelocity; // Used for SmoothDamp function.
    Vector3 cameraCurrentRotation;
    Vector3 meshCurrentRotation;    // diffrent rotation for swivel - same roation as the camera but a bit slower.

    Vector2 mouseInput;
    float pitch, yaw;

    #endregion

    #region Shooting Variables
    [Header("Weapon Variables")]
    public int ammoCount;
    public int maxAmmo;
    float lastTimeShot;

    public LayerMask allButPlayerLayerMask, notPlayerOrInteractible;
    Ray middleScreenRay; // Used for raycasting from middle of the screen.
    Ray weaponRay;       // Used for raycasting from the weapon to check near obstacles.
    float spread, maxSpread = 10f; // maxSpread is based on animation! - be carefull.
    public Vector2 sniperScopeOffset = new Vector2(-200, 150);

    public bool weaponIsEquiped;
    public Weapon.FireMode weaponFireModes;
    Weapon.FireMode currentFireMode;    // single fire or automatic - if can.
    bool pulledTrigger;                 // Shot Once - used to check for single fire.

    [HideInInspector]
    public Weapon currentWeapon;
    int mainWeapon, secondaryWeapon;
    [HideInInspector]
    public string muzzleFlash, bulletImpact;
    public Transform weaponSlot;
    [HideInInspector]
    public GameObject[] weapons;
    [HideInInspector]
    public Transform weaponMuzzle; // Used to shoot 'weaponRay'.
    #endregion

    #region Walking Varaibles
    [Header("Movement Variables")]
    public float speedSmoothMultiplier = 7;
    public float walkSpeed = 2, runSpeed = 6;
    float currentSpeed, targetSpeed;

    Vector3 movingInput;
    bool isRunning;

    Vector3 movingDirection;

    #endregion

    Animator anim;
    Transform camera;
    Rigidbody rgb;
    Camera playerCamera;
    ObjectPooler objPooler;
    WeaponAttachmentManager weaponAttachM;
    CrossHair crossHair;
    ScreenMessageLine scrMsgLine;

    #region Singelton
    static public PlayerController instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    void Start()
    {
        anim = GetComponent<Animator>();
        rgb = GetComponent<Rigidbody>();
        camera = transform.GetChild(0);
        playerCamera = camera.GetComponent<Camera>();
        objPooler = ObjectPooler.instance;
        weaponAttachM = GetComponent<WeaponAttachmentManager>();
        crossHair = CrossHair.instance;
        scrMsgLine = ScreenMessageLine.instance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ammoCount = maxAmmo;

        // Make array with al lthe weapons.
        weapons = new GameObject[weaponSlot.childCount];
        for (int i = 0; i < weaponSlot.childCount; i++)
        {
            weapons[i] = weaponSlot.GetChild(i).gameObject;
        }
    }

    void FixedUpdate()
    {
        HandleInput();
        HandleCameraRotation();
        PlayerMovement();

        crossHair.GetInput(new Vector2(mouseInput.x, mouseInput.y).magnitude, new Vector2(movingInput.x, movingInput.z).magnitude * currentSpeed);

        // Fire rate:
        if (fire)
        {
            switch (currentFireMode)
            {
                case Weapon.FireMode.Automatic:
                    // Do nothing.
                    break;
                case Weapon.FireMode.Burst:
                    // Not used here - check in fire method.
                    break;
                case Weapon.FireMode.Single:
                    if (pulledTrigger)
                        fire = false;
                    break;
            }
        }

        // Cross hair aiming variable:
        crossHair.Aiming = aim;

        // Cross hair OnTarget variable:
        spread = aim ? 0 : maxSpread / 10 * crossHair.CurrentSpread;

        if (Physics.SphereCast(camera.position, spread / 5f, camera.forward, out RaycastHit hit, notPlayerOrInteractible))
            crossHair.OnTarget = hit.transform.tag == "Enemy";
        else
            crossHair.OnTarget = false;

        if (Physics.Raycast(camera.position, camera.forward, out RaycastHit interactHit, 2))
        {
            crossHair.CanInteract = interactHit.transform.tag == "Interactive";
            interactive = interactHit.transform.GetComponent<InteractiveItem>();

            if (interactive && interactive.itemType == InteractiveItem.ItemType.Weapon)
                scrMsgLine.weaponName = interactive.WeaponName();
            else
            {
                scrMsgLine.weaponName = "Empty";
                crossHair.CanInteract = false;
                interactive = null;
            }
        }
        else
        {
            scrMsgLine.weaponName = "Empty";
            crossHair.CanInteract = false;
            interactive = null;
        }

        anim.SetBool("Aim", aim);
        anim.SetBool("Fire", fire);
        anim.SetBool("Reload", reload);
        anim.SetFloat("Vertical", currentSpeed);
        anim.SetBool("AttachmentsMenu", attachmentMenu);

    }

    void PlayerMovement()
    {
        // If there's input for moving.
        if (movingInput != Vector3.zero)
        {
            targetSpeed = isRunning ? reload ? walkSpeed : runSpeed : walkSpeed;
        }
        else
        {
            targetSpeed = 0;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedSmoothMultiplier);

        // Direction relative to camera.
        movingDirection = camera.TransformVector(movingInput);
        movingDirection.y = 0;

        rgb.velocity = movingDirection * currentSpeed;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.N) && pressedAttachmentMenuOnce)
        {
            if (weaponIsEquiped)
            {
                attachmentMenu = !attachmentMenu;

                if (attachmentMenu)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            pressedAttachmentMenuOnce = false;
        }
        else
            pressedAttachmentMenuOnce = true;

        if (!attachmentMenu)
        {
            if (Input.GetAxis("Fire1") == 0) pulledTrigger = false;    // Used for FireMode.Single - to check if lifted the mouseButton.
            if (Input.GetAxis("WeaponSlot1") != 0) ChangeWeapon(true, mainWeapon);
            if (Input.GetAxis("WeaponSlot2") != 0) ChangeWeapon(false, secondaryWeapon);
            if (Input.GetAxis("Interact") != 0 && interactive) PickUpWeapon();

            mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            yaw += mouseInput.x * mouseSensitivity;
            pitch -= mouseInput.y * mouseSensitivity;

            isRunning = aim ? false : Input.GetKey(KeyCode.LeftShift) ? true : false;

            if (weaponIsEquiped)
            {
                aim = Input.GetMouseButton(1) && !reload ? true : false;
                fire = Input.GetMouseButton(0) && !reload ? true : false;
                reload = Input.GetKey(KeyCode.R) && ammoCount != maxAmmo ? true : reload ? true : false;
            }
        }

        movingInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

    }

    void HandleCameraRotation()
    {

        // Clamp pitch.
        pitch = Mathf.Clamp(pitch, -40, 75);

        // Set orientation:
        cameraCurrentRotation = Vector3.SmoothDamp(cameraCurrentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, cameraRotationSpeed);
        camera.eulerAngles = cameraCurrentRotation;

        // Set orientation for hands (to swivel):
        meshCurrentRotation = Vector3.Lerp(meshCurrentRotation, cameraCurrentRotation, Time.deltaTime * 30);
        camera.transform.GetChild(0).eulerAngles = meshCurrentRotation;
    }

    public void Fire()
    {
        if (!weaponIsEquiped)
            return;

        pulledTrigger = true;
        crossHair.AddSpread(10);

        // Handle ammo and reloading
        if (ammoCount > 1)
            ammoCount--;
        else
        {
            ammoCount--;
            anim.SetBool("NoAmmo",true);
        }
        currentWeapon.currentAmmo = ammoCount;

        // Handle raycasting
        middleScreenRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Rotate fire direction by spread.
        if(!currentWeapon.scopeSniper || !aim) 
            middleScreenRay.direction = Quaternion.AngleAxis(Random.Range(-spread,spread), new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * camera.forward;
        else
            middleScreenRay.direction = Quaternion.AngleAxis(Random.Range(-spread, spread), new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * currentWeapon.scopeCamera.transform.forward;

        muzzleFlash = !currentWeapon.silenced ? currentWeapon.muzzleFlash : currentWeapon.silencedMuzzleFlash;

        objPooler.SpawnFromPool(muzzleFlash, weaponMuzzle.rotation, Vector3.zero, weaponMuzzle);

        if (Physics.Raycast(middleScreenRay, out RaycastHit hit, notPlayerOrInteractible))
        {

            // Cast a ray to see if their is an obstacle near the weapon - if there is one, shoot that!
            weaponRay = new Ray(weaponMuzzle.position, weaponMuzzle.forward);
            // Rotate fire direction by spread.
            weaponRay.direction = Quaternion.AngleAxis(Random.Range(-spread, spread), new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * weaponMuzzle.forward;

            Vector2 hitmarkOffset = currentWeapon.scopeSniper && aim ? sniperScopeOffset : Vector2.zero;

            if (Physics.Raycast(weaponRay, out RaycastHit newHit, 3, notPlayerOrInteractible))
            {
                objPooler.SpawnFromPool(bulletImpact, newHit.point, Quaternion.identity);
                if (newHit.transform.tag == "Enemy")
                    objPooler.SpawnFromPool("UI Hit Mark", playerCamera.WorldToScreenPoint(newHit.point) + transform.up * hitmarkOffset.y + transform.right * hitmarkOffset.x, Quaternion.identity);
            }
            else
            {
                objPooler.SpawnFromPool(bulletImpact, hit.point, Quaternion.identity);
                if (hit.transform.tag == "Enemy")
                    objPooler.SpawnFromPool("UI Hit Mark", playerCamera.WorldToScreenPoint(hit.point) + transform.up * hitmarkOffset.y + transform.right * hitmarkOffset.x, Quaternion.identity);
            }

        }
    }

    public void Reload()
    {
        ammoCount = currentWeapon.currentAmmo = maxAmmo;
        anim.SetBool("NoAmmo", false);
        reload = false;
    }

    public void PickUpWeapon() 
    {
        ChangeWeapon(interactive.mainWeapon, interactive.weaponNumTag);
        interactive.gameObject.SetActive(false);
    }

    public void ChangeWeapon(bool main, int weaponNumTag)
    {
        if (weaponNumTag == 0)
        {
            scrMsgLine.NoWeaponEquipped();
            return;
        }

        if (main)
            mainWeapon = weaponNumTag;
        else
            secondaryWeapon = weaponNumTag;

        currentWeapon = weapons[weaponNumTag - 1].GetComponent<Weapon>();
        anim.SetInteger("WeaponNumTag", weaponNumTag);
    }

    public void DrawWeapon(int weaponNumTag)
    {
        weaponNumTag -= 1;


        currentWeapon.gameObject.SetActive(true);
        weaponMuzzle = currentWeapon.transform.GetChild(0);

        muzzleFlash = currentWeapon.muzzleFlash;
        bulletImpact = currentWeapon.bulletImpact;

        weaponFireModes = currentWeapon.capableFireModes;
        currentFireMode = weaponFireModes;

        ammoCount = currentWeapon.currentAmmo;
        maxAmmo = currentWeapon.maxAmmo;

        anim.SetFloat("Scope", currentWeapon.scopeNumTag);
        reload = false;
    }
    public void SheathWeapon(int weaponNumTag)
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        weaponMuzzle = null;
    }
}
