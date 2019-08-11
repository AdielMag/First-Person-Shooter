using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform currentWeaponIcon, secondWeaponIcon;
    public Slider hpBar, staminaBar;

    public Transform ammoIndicator;

    PlayerController pCon;

    public static  UIManager instance;
    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        pCon = PlayerController.instance;

   //     hpBar.maxValue = hpBar.value = pCon.maxHealth;
   //     staminaBar.maxValue = staminaBar.value = pCon.maxStamina;
    }

    public void SetWeaponsIcons(int currentWeapon, int secondWeapon)
    {
        if (currentWeapon == 0)
            return;

        for (int i = 0; i < currentWeaponIcon.childCount; i++)
            currentWeaponIcon.GetChild(i).gameObject.SetActive(false);

        currentWeaponIcon.GetChild(currentWeapon - 1).gameObject.SetActive(true);

        if (secondWeapon == 0)
            return;

        for (int i = 0; i < secondWeaponIcon.childCount; i++)
            secondWeaponIcon.GetChild(i).gameObject.SetActive(false);

        secondWeaponIcon.GetChild(secondWeapon - 1).gameObject.SetActive(true);
    }

    public void SetAmmoCounter(bool main,int currentMagAmmo, int reservedAmmo)
    {
        Transform indicatorParent = ammoIndicator.GetChild(main ? 0 : 1);
        indicatorParent.gameObject.SetActive(true);

        ammoIndicator.GetChild(main ? 1 : 0).gameObject.SetActive(false);

        for (int i = 0; i < indicatorParent.childCount; i++)
        {
            if (i < currentMagAmmo)
                indicatorParent.GetChild(i).GetComponent<Image>().color = Color.white;

            else
            {
                Color color = Color.white;
                color.a = .3f;

                indicatorParent.GetChild(i).GetComponent<Image>().color = color;
            }
        }

        ammoIndicator.GetChild(2).GetComponent<Text>().text = currentMagAmmo + "/" + reservedAmmo;
    }
}
