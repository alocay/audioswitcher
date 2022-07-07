using System;

namespace AudioSwitcher.actions
{
    public abstract class AudioSwitcherAction
    {
        public string ActionTitle { get; set; }

        public abstract void Run(AudioMappingManager mapping);

        public bool IsSameAction(string title)
        {
            return ActionTitle.Equals(title.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
