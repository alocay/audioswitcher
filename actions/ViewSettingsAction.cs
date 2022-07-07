using AudioSwitcher.settings;
using Spectre.Console;

namespace AudioSwitcher.actions
{
    public class ViewSettingsAction : AudioSwitcherAction
    {
        private SettingsManager settingsManager;

        public ViewSettingsAction(SettingsManager settingsManager)
        {
            ActionTitle = "View settings";
            this.settingsManager = settingsManager;
        }

        public override void Run(AudioMappingManager mapping)
        {
            var table = new Table();
            table.AddColumn("Setting");
            table.AddColumn("Value");
            
            foreach (var setting in settingsManager.GetAllSettings())
            {
                if (setting == Setting.Cancel)
                {
                    continue;
                }

                TableExtensions.AddRow(table, new string[] { settingsManager.GetSettingTitle(setting), settingsManager.GetSettingValueString(setting) });
            }

            AnsiConsole.Write(table);
        }
    }
}


