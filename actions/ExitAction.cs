using AudioSwitcher.settings;
using System;

namespace AudioSwitcher.actions
{
    public class ExitAction : AudioSwitcherAction
    {
        private ProcessListener listener;
        private SettingsManager settingsManager;

        public ExitAction(ProcessListener listener, SettingsManager settingsManager)
        {
            this.listener = listener;
            this.settingsManager = settingsManager;
            ActionTitle = "Exit";
        }

        public override void Run(AudioMappingManager mapping)
        {
            listener.Stop();
            mapping.SaveMappings();
            settingsManager.SaveSettings();
            Environment.Exit(0);
        }
    }
}
