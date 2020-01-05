using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRenderer : MonoBehaviour
{
    public SpriteData spriteData;
    [System.Serializable]
    public class SpriteData
    {
        public NoteSpriteData[] notes;
    }
    [System.Serializable]
    public class NoteSpriteData
    {
        public Sprite normal, finisher;
    }
}
