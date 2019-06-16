using UnityEngine;
using UnityEngine.UI;

public class ScreenMessageLine : MonoBehaviour
{
    public string weaponName = "Empty";

    Animator anim;
    Text text;
    #region Singelton
    public static ScreenMessageLine instance;
    private void Awake()
    {
        instance = this;

        text = GetComponent<Text>();
        anim = GetComponent<Animator>();
    }
    #endregion

    private void FixedUpdate()
    {
        if (weaponName == "Empty")
        {
            anim.SetBool("ShowInteraction", false);
            return;
        }

        anim.SetBool("ShowInteraction", true);
        text.text = "Pick Up " + weaponName;
    }

    public void NoWeaponEquipped() 
    {
        text.text = "Weapon slot is empty";

        anim.SetBool("ShowText", true);
    }
    public void NoAmmo()
    {
        text.text = "No Ammo";

        anim.SetBool("ShowText", true);
    }
    public void HideNoWeaponEquipped()
    {
        anim.SetBool("ShowText", false);
    }
}
