using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSwitcher.settings
{
    public class SettingsChangedEventArgs : EventArgs
    {
        public Setting Setting { get; set; }

        public SettingsChangedEventArgs(Setting setting)
        {
            Setting = setting;
        }
    }
}
