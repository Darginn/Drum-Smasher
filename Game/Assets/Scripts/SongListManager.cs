﻿using DrumSmasher.Assets.Scripts.Charts;
using DrumSmasher.Assets.Scripts.Game.Mods;
using DrumSmasher.Assets.Scripts.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DrumSmasher.Assets.Scripts
{
    public class SongListManager : MonoBehaviour
    {
        public Charts.Chart LoadedChart;

        public GameObject ListContent;
        public GameObject ErrorPanel;

        bool _sceneActionActive; 

        SongScanning _scan;
        public List<SongScanning.SongInfo> SongList;

        [SerializeField] ModPanel _modPanel;

        public void Start()
        {
            _scan = FindObjectOfType<SongScanning>();
            if (SongList == null)
            {
                _scan.RefreshSongs();
                SongList = _scan.Songs;
            }

            if (SongList.Count == 0)
            {
                ErrorPanel.GetComponentInChildren<Text>().text = "No charts found!";
                ErrorPanel.SetActive(true);
            }

            for (int i = 0; i < SongList.Count; i++)
            {
                InstantiatePrefab(SongList[i].DisplayName, SongList[i].chart, SongList[i].ChartDirectory);
            }

            TitleScreenSettings tss = (TitleScreenSettings)SettingsManager.SettingsStorage["TitleScreen"];
            Application.targetFrameRate = tss.Data.FPSMenu;
            QualitySettings.vSyncCount = 0;
            Logger.Log($"Set FPS limit to {Application.targetFrameRate} and VSYNC {(QualitySettings.vSyncCount <= 0 ? "false" : "true")}");
        }

        public void InstantiatePrefab(string display, Charts.Chart chart, System.IO.DirectoryInfo chartDirectory)
        {
            GameObject songButton = Instantiate(Resources.Load ("Prefabs/SongListButton")) as GameObject;
            songButton.transform.SetParent(ListContent.transform, false);
            songButton.GetComponentInChildren<Text>().text = display;
            songButton.GetComponent<Button>().onClick.AddListener(() => LoadMap(chart, chartDirectory));
        }

        public void LoadMap(Charts.Chart chart, System.IO.DirectoryInfo chartDirectory)
        {
            if (_sceneActionActive)
                return;

            if (ErrorPanel.activeSelf)
                return;

            _sceneActionActive = true;

            if (chart == null)
            {
                ErrorPanel.GetComponentInChildren<Text>().text = "No charts in Charts folder.";
                ErrorPanel.SetActive(true);
                Logger.Log("No chart selected");
                return;
            }

            if (chart != null)
            {
                Logger.Log("Chart loaded, Switching to Taiko");
                LoadedChart = chart;
                _sceneActionActive = false;
                SwitchToTaiko(chartDirectory);
            }
            else
            { 
                ErrorPanel.GetComponentInChildren<Text>().text = "Failed to load Chart";
                ErrorPanel.SetActive(true);
                Logger.Log("Failed to load chart");
            }
            _sceneActionActive = false;
        }

        public void GoBack()
        {
            if (_sceneActionActive)
                return;

            SceneManager.LoadScene("TitleScreen");
        }

        public void SwitchToTaiko(System.IO.DirectoryInfo chartDirectory)
        {
            if (_sceneActionActive)
                return;

            if (ErrorPanel.activeSelf)
                return;

            _sceneActionActive = true;

            Logger.Log("LoadedChart: " + (LoadedChart != null));

            AsyncOperation loadAO = SceneManager.LoadSceneAsync("Main");
            loadAO.completed += ao =>
            {
                _sceneActionActive = false;
                GameManager.OnSceneLoaded(LoadedChart, chartDirectory, _modPanel.GetMods());
            };
        }
    }
}
