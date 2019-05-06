using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentButton : MonoBehaviour
{
    public enum AttachmentType {Muzzle,Scope,Grip}
    public float ScopeNumTag;
    public AttachmentType attachmentType;
    public GameObject attachment;

    Animator anim;
    PlayerController pCon;

    private void Start()
    {
        anim = GetComponent<Animator>();
        pCon = PlayerController.instance;
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
                    break;
                case AttachmentType.Scope:
                    pCon.GetComponent<Animator>().SetFloat("Scope", ScopeNumTag);
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
                    break;
                case AttachmentType.Scope:
                    pCon.GetComponent<Animator>().SetFloat("Scope", 0);
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
}
