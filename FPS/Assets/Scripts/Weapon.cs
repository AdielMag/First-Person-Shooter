using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool mainWeapon;
    public int damage;

    public enum FireMode {Automatic,Burst,Single,Shotgun}
    public FireMode capableFireModes;

    [Header("Need to be EXACTLY the same as the one in 'ObjectPooler'")]
    public string muzzleFlash;
    public string bulletImpact;

    [Header("Attachments Menu")]
    public GameObject attachmentMenu;
}
