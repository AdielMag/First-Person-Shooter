using UnityEngine;

public class InteractiveItem : MonoBehaviour
{
    public enum ItemType{Weapon,Door}
    public ItemType itemType;

    [Header("If Weapon")]
    public bool mainWeapon;
    public int weaponNumTag;

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
