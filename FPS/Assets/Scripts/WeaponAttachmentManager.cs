using UnityEngine;
using System.Collections;

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
    }

    public void HandleWeaponRotation()
    {
        mousePosition = Input.mousePosition;

        relativeToMiddlePointPos = mousePosition - (Vector2)secondaryWeaponMiddlePoint.position;

        targetNormalizedPos = new Vector2(relativeToMiddlePointPos.x / secondaryWeaponEdges.x, relativeToMiddlePointPos.y / secondaryWeaponEdges.y);

        anim.SetFloat("AttachmentMenuX", targetNormalizedPos.x);
        anim.SetFloat("AttachmentMenuY", targetNormalizedPos.y);

    }
    
    public void OpenAttachmentMenu() 
    {
        pCon.currentWeapon.transform.GetChild(pCon.currentWeapon.transform.childCount - 1).gameObject.SetActive(true);

        AttachmentButton[] attachmentButtons = pCon.currentWeapon.GetComponentsInChildren<AttachmentButton>();

        for (int i = 0; i < attachmentButtons.Length; i++)
        {
            attachmentButtons[i].CheckIfEnabled();
        }
    }

    public void CloseAttachmentMenu()
    {
        StartCoroutine(CloseAndWaitAtttachmentMenu());
    }

    IEnumerator CloseAndWaitAtttachmentMenu()
    {
        Animator[] attachmentButtons = pCon.currentWeapon.GetComponentsInChildren<Animator>();

        for (int i = 0; i < attachmentButtons.Length; i++)
        {
            attachmentButtons[i].SetTrigger("FadeOut");
        }

        yield return new WaitForSeconds(.5f);
        pCon.currentWeapon.transform.GetChild(pCon.currentWeapon.transform.childCount - 1).gameObject.SetActive(false);
        yield break;
    }
}
