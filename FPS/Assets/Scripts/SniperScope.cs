using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class SniperScope : MonoBehaviour
{
    Vignette vignette;

    Transform playerCameraTransform;
    public Vector2 playerCamerOffset;
    Vector2 offset;

    void Start()
    {
        GetComponent<PostProcessVolume>().profile.TryGetSettings(out vignette);
        playerCameraTransform = PlayerController.instance.transform.GetChild(0);
    }

    void Update()
    {
        Vector3 sd = (playerCameraTransform.forward - transform.forward ) * 20;
        offset = new Vector2(0, sd.y);

        vignette.center.value = new Vector2(.5f,.5f) + offset + playerCamerOffset;
    }
}
