using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTimer : MonoBehaviour
{
    public bool destroyObject;
    public float timeToWait;

    float spawnTime;

    private void OnEnable()
    {
        spawnTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (spawnTime + timeToWait < Time.time)
        {
            if (destroyObject)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
