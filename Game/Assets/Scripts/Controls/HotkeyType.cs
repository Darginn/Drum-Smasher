using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controls
{
    /// <summary>
    /// Decides how a hotkey will be checked for, e.g.: Only when the main key was pressed this frame
    /// </summary>
    public enum HotkeyType
    {
        OnKey,
        OnKeyDown,
        OnKeyUp
    }
}
