using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttachment : MonoBehaviour
{

    public GameObject collisionLight;
    public Transform origin;
    LineRenderer lineRen;

    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        origin = transform.GetChild(1);
        lineRen = transform.GetChild(1).GetComponent<LineRenderer>();
    }

    private void Update()
    {
        ray = new Ray(origin.position, origin.forward);
        if (Physics.Raycast(ray, out hit,6)) 
        {
            collisionLight.SetActive(true);
            lineRen.SetPosition(1, new Vector3(0, 0, hit.distance));
            collisionLight.transform.position = origin.position + origin.forward * hit.distance;
        }
        else 
        {
            collisionLight.SetActive(false);
        }
    }
}
