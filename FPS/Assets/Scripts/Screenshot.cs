using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    int photoNum;

    public void TakeScreenShot()
    {
        ScreenCapture.CaptureScreenshot("Screenshot " + photoNum, 10);
        photoNum++;
    }
}
