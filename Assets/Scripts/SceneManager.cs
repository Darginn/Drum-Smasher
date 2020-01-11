using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SManager = UnityEngine.SceneManagement.SceneManager;

namespace DrumSmasher
{
    public static class SceneManager
    {
        public static Scene CurrentScene
        {
            get
            {
                if (_currentScene == null)
                    _currentScene = SManager.GetActiveScene();

                return _currentScene;
            }
        }
        public static Scene LastScene;

        public static bool IsLoading { get; private set; }

        #region preset scene names
        public const string SCENE_TITLE = "TitleScreen";
        public const string SCENE_TAIKO = "Main";
        #endregion

        private static string _nextScene;
        private static Scene _currentScene;

        public static void SwitchScene(string nextScene)
        {
            if (IsLoading)
            {
                Logger.Log("Cannot switch scene while already loading", LogLevel.ERROR);
                return;
            }
            else if (string.IsNullOrEmpty(nextScene))
            {
                Logger.Log("Could not switch to next scene, 'nextScene' is null or empty!", LogLevel.ERROR);
                return;
            }

            Logger.Log("Switchting scene");

            IsLoading = true;
            //Scene curScene = CurrentScene;

            //if (curScene != null)
            //{
            //    _nextScene = nextScene;
            //    UnloadScene(curScene);
            //    return;
            //}

            LoadScene(nextScene);
        }

        private static void UnloadScene(Scene scene)
        {
            Logger.Log("Unloading scene");

            AsyncOperation unloadAO = SManager.UnloadSceneAsync(scene);
            unloadAO.completed += OnUnloadComplete;
        }

        private static void LoadScene(string scene)
        {
            Logger.Log("Loading scene");

            AsyncOperation loadAO = SManager.LoadSceneAsync(scene);
            loadAO.completed += OnLoadComplete;
        }

        private static void OnLoadComplete(AsyncOperation obj)
        {
            Logger.Log("Loaded Scene");
            IsLoading = false;
            _nextScene = null;
        }

        private static void OnUnloadComplete(AsyncOperation obj)
        {
            Logger.Log("Unloaded Scene");

            if (!string.IsNullOrEmpty(_nextScene))
                LoadScene(_nextScene);
        }
    }
}
