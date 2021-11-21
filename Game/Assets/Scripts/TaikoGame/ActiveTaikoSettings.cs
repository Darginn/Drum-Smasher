using UnityEngine;

namespace Assets.Scripts.TaikoGame
{
    public static class ActiveTaikoSettings
    {
        public static bool IgnoreNoteColors { get; set; }
        public static bool IsAutoplayActive { get; set; }

        public static float NoteOffset { get; set; }

        public static Color NoteColorRed { get; set; }
        public static Color NoteColorBlue { get; set; }
        public static Color NoteColorYellow { get; set; }

        public static Vector3 NoteScaleNormal { get; set; }
        public static Vector3 NoteScaleSmall { get; set; }
        public static Vector3 NoteScaleBig { get; set; }

        public static int NoteSpeed { get; set; }

        static ActiveTaikoSettings()
        {
            Reset();
        }

        public static void Reset()
        {
            IgnoreNoteColors = false;
            IsAutoplayActive = false;

            NoteOffset = 0;

            NoteColorRed = Color.red;
            NoteColorBlue = Color.blue;
            NoteColorYellow = Color.yellow;

            NoteScaleNormal = new Vector3(1f, 1f, 1f);
            NoteScaleSmall = new Vector3(.75f, .75f, .75f);
            NoteScaleBig = new Vector3(1.25f, 1.25f, 1.25f);

            NoteSpeed = 0;
        }
    }
}
