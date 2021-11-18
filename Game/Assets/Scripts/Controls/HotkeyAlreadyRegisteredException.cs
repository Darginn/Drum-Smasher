using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controls
{
    public class HotkeyAlreadyRegisteredException : Exception
    {
        public HotkeyAlreadyRegisteredException(HotkeyType keyId) : base($"Hotkey '{keyId}' already exists")
        {

        }
    }
}
