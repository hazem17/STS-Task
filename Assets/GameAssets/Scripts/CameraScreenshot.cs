using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenshot : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        ScreenCapture.CaptureScreenshot("SomeLevel.png", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
