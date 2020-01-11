using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DrumSmasher
{
    public class TitleScreenManager : MonoBehaviour
    {
        public Charts.Chart LoadedChart;
        private bool _sceneActionActive;

        void Start()
        {

        }

        void Update()
        {

        }

        public void SwitchToTaiko()
        {
            if (_sceneActionActive)
                return;
            else if (LoadedChart == null)
            {
                EditorUtility.DisplayDialog("No chart loaded", "Please load a chart before playing", "ok");
                return;
            }

            _sceneActionActive = true;

            AsyncOperation loadAO = SceneManager.LoadSceneAsync("Main");
            loadAO.completed += ao =>
            {
                _sceneActionActive = false;
                GameManager.OnSceneLoaded(LoadedChart);
            };
        }

        public void LoadMap()
        {
            if (_sceneActionActive)
                return;

            _sceneActionActive = true;

            Logger.Log("Selecting chart");
            string path = EditorUtility.OpenFilePanel("Select Chart", @"C:\Games\DrumSmasher\Assets\Charts\Sample Song", "chart");

            if (path.Length < 0)
                return;

            Logger.Log("Loading path");
            LoadedChart = Charts.ChartFile.Load(path);

            if (LoadedChart == null)
            {
                if (EditorUtility.DisplayDialog("Error", "Failed to load chart", "Retry", "Cancel"))
                {
                    Logger.Log("Chart failed to load");
                    LoadMap();
                    return;
                }
                else
                    return;
            }

            Logger.Log("Chart loaded");

            _sceneActionActive = false;
        }

        public void Settings()
        {
            EditorUtility.DisplayDialog("Not implemented", "Settings is currently not implemented", "ok");
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        void OnApplicationExit()
        {
            Logger.Dispose();
        }
    }
}
