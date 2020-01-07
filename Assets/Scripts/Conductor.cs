using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrumSmasher
{
    public class Conductor : MonoBehaviour
    {
        public float SongBpm;
        public float SecPerBeat;
        public float SongPosition;
        public float SongPositionInBeats;
        public float DspSongTime;
        public AudioSource MusicSource;
        public float Offset;

        // Start is called before the first frame update
        void Start()
        {
            MusicSource = GetComponent<AudioSource>();

            SecPerBeat = 60f / SongBpm;

            DspSongTime = (float)AudioSettings.dspTime;

            MusicSource.Play();
        }

        // Update is called once per frame
        void Update()
        {
            SongPosition = (float)(AudioSettings.dspTime - DspSongTime - Offset);

            SongPositionInBeats = SongPosition / SecPerBeat;
        }
    }
}