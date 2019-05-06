using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentButton : MonoBehaviour
{
    public GameObject attachment;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Equip_UnEquip() 
    {
        // Check if the object is enabled. if so - disable it.
        if (attachment.activeInHierarchy)
        {
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

        }
    }

    public void CheckIfEnabled() 
    {
        if (attachment.activeSelf)
            anim.SetBool("Enabled", true);

    }
}
