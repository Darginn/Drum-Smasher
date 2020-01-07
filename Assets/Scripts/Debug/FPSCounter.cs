
﻿using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Debug
{
    public class FPSCounter : MonoBehaviour
    {
        public int AvgFrameRate;
        public Text DisplayText;

        public void Update()
        {
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            AvgFrameRate = (int)current;
            DisplayText.text = AvgFrameRate.ToString() + " FPS";
        }
    }
}