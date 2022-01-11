using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSChecker : MonoBehaviour
{
    private Text fpsText;
    private float deltaTime = 0.0f;
    private float msec;
    private float fps;

    private void Start()
    {
        fpsText = gameObject.GetComponent<Text>();
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;

        fpsText.text = "FPS: " + fps.ToString() + ", mSec: " + msec.ToString();
    }
}
