using UnityEngine;

namespace Assets.Scripts.TaikoGame
{
    public static class ActiveTaikoSettings
    {
        public static bool IgnoreNoteColors { get; set; }
        public static bool IsAutoplayActive { get; set; }

        public static SoundConductor Sound { get; set; }

        public static float NoteOffset { get; set; }

        public static Color NoteColorRed { get; set; } = Color.red;
        public static Color NoteColorBlue { get; set; } = Color.blue;
        public static Color NoteColorYellow { get; set; } = Color.yellow;

        public static Vector3 NoteScaleNormal { get; set; } = new Vector3(1f, 1f, 1f);
        public static Vector3 NoteScaleSmall { get; set; } = new Vector3(.75f, .75f, .75f);
        public static Vector3 NoteScaleBig { get; set; } = new Vector3(1.25f, 1.25f, 1.25f);
    }
}
