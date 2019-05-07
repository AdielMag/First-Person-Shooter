using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool aim, fire, reload; // Commands states.
    [HideInInspector]
    public bool attachmentMenu;

    #region Camera Rotation Variables

    public float cameraRotationSpeed = 5;
    public float mouseSensitivity = 2;

    Vector3 rotationSmoothVelocity; // Used for SmoothDamp function.
    Vector3 cameraCurrentRotation;
    Vector3 meshCurrentRotation;    // diffrent rotation for swivel - same roation as the camera but a bit slower.

    float pitch, yaw;

    #endregion

    #region Shooting Variables
    [Header("Weapon Variables")]
    public int ammoCount;
    public int maxAmmo;
    public float fireRate = 1;
    float lastTimeShot;

    Ray middleScreenRay; // Used for raycasting from middle of the screen.
    Ray weaponRay;       // Used for raycasting from the weapon to check near obstacles.
    float spread , maxSpread = 10f; // maxSpread is based on animation! - be carefull.

    public bool weaponIsEquiped;
    public Weapon.FireMode weaponFireModes;
    Weapon.FireMode currentFireMode;    // single fire or automatic - if can.
    bool pulledTrigger;                 // Shot Once - used to check for single fire.

    [HideInInspector]
    public Weapon currentWeapon;
    int mainWeapon = 1, secondaryWeapon = 2;
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
        
        // Fire rate
        if (fire)
        { switch (currentFireMode)
            {
                case Weapon.FireMode.Automatic:
                    if (lastTimeShot < Time.time)
                        lastTimeShot = Time.time + (.6f / fireRate);
                    else
                        fire = false;
                    break;
                case Weapon.FireMode.Burst:
                    // Currently not doing anything.
                    break;
                case Weapon.FireMode.Single:
                    if (pulledTrigger)
                        fire = false;
                    break;
            }
        }

        anim.SetBool("Aim", aim);
        anim.SetBool("Fire", fire);
        anim.SetBool("Reload", reload);
        anim.SetFloat("Vertical", currentSpeed);
        anim.SetBool("AttachmentsMenu", attachmentMenu);

    }

    private void PlayerMovement()
    {
        // If there's input for moving.
        if (movingInput != Vector3.zero)
        {
            targetSpeed = isRunning ? runSpeed : walkSpeed;
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

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.N))
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
        }

        if (!attachmentMenu)
        {
            if (!Input.GetMouseButton(0)) pulledTrigger = false;    // Used for FireMode.Single - to check if lifted the mouseButton.
            if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(true, mainWeapon);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(false, secondaryWeapon);

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

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

    private void HandleCameraRotation()
    {

        // Clamp pitch.
        pitch = Mathf.Clamp(pitch, -40, 55);

        // Set orientation:
        cameraCurrentRotation = Vector3.SmoothDamp(cameraCurrentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, cameraRotationSpeed);
        camera.eulerAngles = cameraCurrentRotation;

        // Set orientation for hands (to swivel):
        meshCurrentRotation = Vector3.Lerp(meshCurrentRotation, cameraCurrentRotation, Time.deltaTime * 25);
        camera.transform.GetChild(0).eulerAngles = meshCurrentRotation;
    }

    public void Fire()
    {
        if (!weaponIsEquiped)
            return;

        pulledTrigger = true;

        // Handle ammo and reloading
        if (ammoCount > 1)
            ammoCount--;
        else
        {
            ammoCount--;
            anim.SetBool("NoAmmo",true);
        }

        // Handle raycasting
        middleScreenRay = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        spread = (maxSpread / 10) * crossHair.CurrentSpread;

        // Rotate fire direction by spread.
        middleScreenRay.direction = Quaternion.AngleAxis(Random.Range(-spread,spread), new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * camera.forward;

        objPooler.SpawnFromPool(muzzleFlash, weaponMuzzle.rotation, Vector3.zero, weaponMuzzle);

        if (Physics.Raycast(middleScreenRay, out RaycastHit hit))
        {
            // Cast a ray to see if their is an obstacle near the weapon - if there is one, shoot that!
            weaponRay = new Ray(weaponMuzzle.position, weaponMuzzle.forward);
            weaponRay.direction = Quaternion.AngleAxis(Random.Range(-spread, spread), new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0)) * weaponMuzzle.forward;

            if (Physics.Raycast(weaponRay, out RaycastHit newHit, 3))
                objPooler.SpawnFromPool(bulletImpact, newHit.point, Quaternion.identity);
            else
                objPooler.SpawnFromPool(bulletImpact, hit.point, Quaternion.identity);
        }

        crossHair.AddSpread(5);
    }

    public void Reload()
    {
        ammoCount = maxAmmo;
        anim.SetBool("NoAmmo", false);
        reload = false;
    }

    public void ChangeWeapon(bool main, int weaponNumTag)
    {
        if (weaponNumTag == 0)
            return;

        if (main)
            mainWeapon = weaponNumTag;
        else
            secondaryWeapon = weaponNumTag;

        currentWeapon = weapons[weaponNumTag - 1].GetComponent<Weapon>();
        anim.SetInteger("WeaponNumTag", weaponNumTag);
    }

    public void EquipWeapon(int weaponNumTag)
    {
        weaponNumTag -= 1;


        currentWeapon.gameObject.SetActive(true);
        weaponMuzzle = currentWeapon.transform.GetChild(0);

        muzzleFlash = currentWeapon.muzzleFlash;
        bulletImpact = currentWeapon.bulletImpact;
        fireRate = currentWeapon.fireRate;

        weaponFireModes = currentWeapon.capableFireModes;
        currentFireMode = weaponFireModes;
    }
    public void UnequipWeapon(int weaponNumTag)
    {
        foreach (GameObject weapon in weapons)
            weapon.SetActive(false);

        weaponMuzzle = null;
    }
}
