
﻿using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher.Debug
{
    public class FPSCounter : MonoBehaviour
    {
        public int AvgFrameRate;
        public int MaxFrameRate;
        public Text FPSText;
        public Text MAXFPSText;

        public void Update()
        {
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            AvgFrameRate = (int)current;
            FPSText.text = AvgFrameRate.ToString() + " FPS";
            if (MaxFrameRate < AvgFrameRate)
                MaxFrameRate = AvgFrameRate;
            MAXFPSText.text = "Max: " + MaxFrameRate.ToString() + " FPS";
        }
    }
}