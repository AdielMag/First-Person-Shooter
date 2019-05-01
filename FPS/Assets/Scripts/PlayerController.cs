using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool aim, fire, reload; // Commands states.
    

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

    public bool weaponIsEquiped;
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
    Vector2 input;
    #endregion

    Animator anim;
    Transform camera;
    Camera playerCamera;
    ObjectPooler objPooler;

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
        camera = transform.GetChild(0);
        playerCamera = camera.GetComponent<Camera>();
        objPooler = ObjectPooler.instance;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ammoCount = maxAmmo;

        // Make array with al lthe weapons.
        weapons = new GameObject[weaponSlot.childCount];
        for(int i =0; i < weaponSlot.childCount; i++)
        {
            weapons[i] = weaponSlot.GetChild(i).gameObject;
        }
    }

    void FixedUpdate()
    {
        HandleCameraRotation();
        HandleInput();

        // Player movement!

        // Fire rate
        if (fire)
        {
            if (lastTimeShot < Time.time)
                lastTimeShot = Time.time + (.6f / fireRate);
            else
                fire = false;
        }

        anim.SetBool("Aim", aim);
        anim.SetBool("Fire", fire);
        anim.SetBool("Reload", reload);

    }

    private void HandleInput()
    {
        if (weaponIsEquiped)
        {
            aim = Input.GetMouseButton(1) && !reload ? true : false;
            fire = Input.GetMouseButton(0) && !reload ? true : false;
            reload = Input.GetKey(KeyCode.R) && ammoCount != maxAmmo ? true : (reload) ? true : false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(true, mainWeapon);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeapon(false, secondaryWeapon);
    }

    private void HandleCameraRotation()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Clamp pitch.
        pitch = Mathf.Clamp(pitch, -40, 55);

        // Set orientation:
        cameraCurrentRotation = Vector3.SmoothDamp(cameraCurrentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, cameraRotationSpeed);
        camera.eulerAngles = cameraCurrentRotation;

        meshCurrentRotation = Vector3.Lerp(meshCurrentRotation, cameraCurrentRotation, Time.deltaTime * 30);
        camera.transform.GetChild(0).eulerAngles = meshCurrentRotation;

        // transform.eulerAngles = new Vector3(-pitch, yaw, 0f);
    }

    public void Fire()
    {
        if (!weaponIsEquiped)
            return;

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

        objPooler.SpawnFromPool(muzzleFlash, weaponMuzzle.rotation, Vector3.zero, weaponMuzzle);

        if (Physics.Raycast(middleScreenRay, out RaycastHit hit))
        {
            // Cast a ray to see if their is an obstacle near the weapon - if there is one, shoot that!
            weaponRay = new Ray(weaponMuzzle.position, weaponMuzzle.forward);
            
            if (Physics.Raycast(weaponRay, out RaycastHit newHit, 3))
                objPooler.SpawnFromPool(bulletImpact, newHit.point, Quaternion.identity);
            else
                objPooler.SpawnFromPool(bulletImpact, hit.point, Quaternion.identity);
        }
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

        anim.SetInteger("WeaponNumTag", weaponNumTag);
    }

    public void EquipWeapon(int weaponNumTag)
    {
        weaponNumTag -= 1;

        weapons[weaponNumTag].gameObject.SetActive(true);
        weaponMuzzle = weapons[weaponNumTag].transform.GetChild(0);

        muzzleFlash = weapons[weaponNumTag].GetComponent<Weapon>().muzzleFlash;
        bulletImpact = weapons[weaponNumTag].GetComponent<Weapon>().bulletImpact;
        fireRate = weapons[weaponNumTag].GetComponent<Weapon>().fireRate;

    }
    public void UnequipWeapon(int weaponNumTag)
    {
        weapons[weaponNumTag - 1].gameObject.SetActive(false);
        weaponMuzzle = null;
    }
}
