using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamManager : MonoBehaviour
{
    private WebCamTexture webcamTexture;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        //如果有后置摄像头，调用后置摄像头  
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            webcamTexture.deviceName = WebCamTexture.devices[i].name;
            break;
        }

        RawImage renderer = GetComponent<RawImage>();
        renderer.texture = webcamTexture;
        webcamTexture.Play();
    }

    public void Clear()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            Destroy(this);
        }
    }
}
