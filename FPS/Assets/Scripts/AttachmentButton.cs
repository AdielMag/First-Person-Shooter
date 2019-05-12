using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentButton : MonoBehaviour
{
    public enum AttachmentType {Muzzle,Scope,Grip}
    public int scopeNumTag;
    public AttachmentType attachmentType;
    public GameObject attachment;
    
    Animator anim;
    PlayerController pCon;
    Weapon thisWeapon;

    private void Start()
    {
        anim = GetComponent<Animator>();
        pCon = PlayerController.instance;
        thisWeapon = GetComponentInParent<Weapon>();
    }

    public void Equip_UnEquip() 
    {
        // Check if the object is enabled. if so - disable it.
        if (attachment.activeInHierarchy)
        {
            HandleAttachmentTypeChanges(false);
            attachment.SetActive(false);
            anim.SetBool("Enabled", false);
        }

        // The object is not activated.
        else
        {
            // Disable all other attachments of this type.
            foreach (Transform attachmentObj in attachment.transform.parent)
            {
                attachmentObj.gameObject.SetActive(false);
            }
            // Turn all animator controlles bool off.
            foreach (Transform attachmentObj in transform.parent)
            {
                attachmentObj.GetComponent<Animator>().SetBool("Enabled", false);
            }
            // Enable attachment gameobject
            attachment.SetActive(true);
            // Turn this animator bool on.
            anim.SetBool("Enabled", true);

            HandleAttachmentTypeChanges(true);
        }
    }

    void HandleAttachmentTypeChanges(bool enable) 
    {
        if (enable) 
        {
            switch (attachmentType) 
            {
                case AttachmentType.Muzzle:
                    pCon.weaponMuzzle = attachment.transform.GetChild(0);
                    thisWeapon.silenced = true;
                    break;
                case AttachmentType.Scope:
                    thisWeapon.scopeNumTag = scopeNumTag * 5;
                    pCon.GetComponent<Animator>().SetFloat("Scope", thisWeapon.scopeNumTag);
                    thisWeapon.scopeSniper = scopeNumTag == 2 ? true : false;
                    break;
                case AttachmentType.Grip:
                    break;
            }
        }
        else 
        {
            switch (attachmentType)
            {
                case AttachmentType.Muzzle:
                    pCon.weaponMuzzle = PlayerController.instance.currentWeapon.transform.GetChild(0);
                    thisWeapon.silenced = false;
                    break;
                case AttachmentType.Scope:
                    thisWeapon.scopeNumTag = 0;
                    pCon.GetComponent<Animator>().SetFloat("Scope", thisWeapon.scopeNumTag);
                    thisWeapon.scopeSniper = false;
                    break;
                case AttachmentType.Grip:
                    break;
            }
        }
    }

    public void CheckIfEnabled() 
    {
        if (attachment.activeSelf)
            anim.SetBool("Enabled", true);

    }

    public void HandleHovering(bool isHovering ) 
    {
        anim.SetBool("Hovering", isHovering); 
    }
}
