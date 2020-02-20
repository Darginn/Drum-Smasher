using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DrumSmasher
{
    public class SongListManager : MonoBehaviour
    {
        public GameObject ButtonPrefab;
        public GameObject ListContent;

        SongScanning _scan;
        public List<SongScanning.SongInfo> SongList;

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
                InstantiatePrefab("No Songs Found");
            }
            for (int i = 0; i < SongList.Count; i++)
            {
                InstantiatePrefab(SongList[i].DisplayArtist);
            }
        }

        void InstantiatePrefab(string display)
        {
            GameObject songButton = Instantiate(ButtonPrefab);
            songButton.transform.SetParent(ListContent.transform, false);
            songButton.GetComponentInChildren<Text>().text = display;
        }
    }
}
