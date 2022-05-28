using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    public float targetSize = 4.5f;
    private float refRatio
    {
        get { return (1920f / 1080); }
    }
    void Start()
    {
        Camera  cam= Camera.main;
        var size = cam.orthographicSize;
        var currentRatio = ((float) (Screen.height)) / Screen.width;
        var ratio = currentRatio * refRatio;
        cam.orthographicSize = targetSize * ratio;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
