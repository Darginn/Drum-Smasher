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
        public GameObject PlayAlert;
        public GameObject FailAlert;
        public GameObject SettingsAlert;

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
                PlayAlert.SetActive(true);
                return;
            }

            _sceneActionActive = true;

            Logger.Log("LoadedChart: " + (LoadedChart == null));

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
            string path = EditorUtility.OpenFilePanel("Select Chart", Application.dataPath, "chart");

            if (path.Length < 0)
                return;

            Logger.Log("Loading path");
            LoadedChart = Charts.ChartFile.Load(path);

            if (LoadedChart == null)
            {
                FailAlert.SetActive(true);
                Logger.Log("Chart failed to load");
                return;
            }

            if (LoadedChart != null)
                Logger.Log("Chart loaded");
            else
                Logger.Log("Failed to load chart");

            _sceneActionActive = false;
        }

        public void Settings()
        {
            SettingsAlert.SetActive(true);
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
