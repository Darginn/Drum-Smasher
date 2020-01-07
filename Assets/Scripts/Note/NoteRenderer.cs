using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrumSmasher.Note
{
    public class NoteRenderer : MonoBehaviour
    {
        public SpriteData SpriteData;
    }
    
    [System.Serializable]
    public class SpriteData
    {
        public NoteSpriteData[] Notes;
    }

    [System.Serializable]
    public class NoteSpriteData
    {
        public Sprite Normal;
        public Sprite Finisher;
    }
}