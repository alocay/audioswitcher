using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioSwitcher.settings
{
    public class AudioSwitcherSettings
    {
        public bool ShouldSwitchOnlyOnProcessStart { get; set; } = false;
        public string FallbackPlaybackDevice { get; set; }
        public bool UseFallbackDevice { get; set; } = true;
    }
}
