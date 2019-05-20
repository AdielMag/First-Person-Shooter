﻿using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    public enum ItemType{Weapon,Door}
    public ItemType itemType;
    
    [Header("If Weapon")]
    public bool mainWeapon;
    public int weaponNumTag;

    Vector3 middle;

    private void Start()
    {
        middle = transform.GetChild(0).position;
    }

    public void DoorBehaviour(Vector3 playerPosition)
    {
        if ((middle - playerPosition).z > 0 )
        {
            transform.RotateAround(Vector3.up,0.02f);
        }
        else
            transform.RotateAround(Vector3.up, -0.02f);
    }

    public string WeaponName()
    {
        switch (weaponNumTag)
        {
            case 1:
                return "M4";
            case 2:
                return "Swat Pistol";
            case 3:
                return "Ak47";
            case 4:
                return "Bandit Pistol";
        }

        return "Empty";
    }
}
