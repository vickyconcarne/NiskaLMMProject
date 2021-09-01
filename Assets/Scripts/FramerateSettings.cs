using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
public class FramerateSettings : MonoBehaviour
{
    public bool debug;
    public TextMeshProUGUI fpsText;

    public string formatedString = "{value} FPS";

    public float updateRateSeconds = 4.0F;

    int frameCount = 0;
    float dt = 0.0F;
    float fps = 0.0F;

    void Update()
    {
        if (!debug) return;
        frameCount++;
        dt += Time.unscaledDeltaTime;
        if (dt > 1.0 / updateRateSeconds)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0F / updateRateSeconds;
        }
        fpsText.text = formatedString.Replace("{value}", System.Math.Round(fps, 1).ToString("0.0"));
    }

    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
