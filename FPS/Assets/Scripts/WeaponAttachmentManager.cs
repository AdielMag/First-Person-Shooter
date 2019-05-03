using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponAttachmentManager : MonoBehaviour
{
    public RectTransform mainWeaponMiddlePoint, secondaryWeaponMiddlePoint;

    Vector2 mousePosition;
    Vector2 relativeToMiddlePointPos;

    Vector2 mainWeaponEdges, secondaryWeaponEdges;
    Vector2 targetNormalizedPos;

    Animator anim;
    PlayerController pCon;

    private void Start()
    {
        anim = GetComponent<Animator>();
        pCon = GetComponent<PlayerController>();

     //  mainWeaponEdges = new Vector2(mainWeaponMiddlePoint.rect.width / 2, mainWeaponMiddlePoint.rect.height / 2);
        secondaryWeaponEdges =  new Vector2(secondaryWeaponMiddlePoint.rect.width / 2, secondaryWeaponMiddlePoint.rect.height / 2);
    }

    private void Update()
    {
        if (!pCon.attachmentMenu)
            return;

        HandleWeaponRotation();
        // The attachemnt menu! (by the atachments that can be attached - check in weapon script)
        // Change deapth Of Field! - post proccecing thingy
 
    }

    public void HandleWeaponRotation()
    {
        mousePosition = Input.mousePosition;

        relativeToMiddlePointPos = mousePosition - (Vector2)secondaryWeaponMiddlePoint.position;

        targetNormalizedPos = new Vector2(relativeToMiddlePointPos.x / secondaryWeaponEdges.x, relativeToMiddlePointPos.y / secondaryWeaponEdges.y);

        anim.SetFloat("AttachmentMenuX", targetNormalizedPos.x);
        anim.SetFloat("AttachmentMenuY", targetNormalizedPos.y);

    }
}
