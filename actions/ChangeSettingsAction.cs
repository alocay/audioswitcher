using AudioSwitcher.settings;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AudioSwitcher.actions
{
    public class ChangeSettingsAction : AudioSwitcherAction
    {
        private SettingsManager settingsManager;

        public ChangeSettingsAction(SettingsManager settings)
        {
            ActionTitle = "Change settings";
            settingsManager = settings;
        }

        public override void Run(AudioMappingManager mapping)
        {
            var settings = settingsManager.GetAllSettingTitles().Append("Cancel");

            var prompt = new SelectionPrompt<Setting>()
                .Title("Choose a setting to change:")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices(Enum.GetValues(typeof(Setting)).Cast<Setting>().ToArray())
                .UseConverter(settingsManager.GetSettingTitle);

            var setting = AnsiConsole.Prompt(prompt);

            switch(setting)
            {
                case Setting.SwitchOnProcessStart:
                    UpdateSwitchOnProcessStartSetting();
                    break;
                case Setting.FallbackDevice:
                    ChangeFallbackPlaybackDevice();
                    break;
                case Setting.UseFallbackDevice:
                    UpdateUseFallbackDeviceSetting();
                    break;
                case Setting.Cancel:
                    return;
            } 
        }

        private void UpdateSwitchOnProcessStartSetting()
        {
            var prompt = new SelectionPrompt<PlaybackSwitchOption>()
               .Title("Choose an option:")
               .PageSize(10)
               .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
               .AddChoices(Enum.GetValues(typeof(PlaybackSwitchOption)).Cast<PlaybackSwitchOption>().ToArray())
               .UseConverter(settingsManager.GetPlaybackSwitchSettingTitle);

            var option = AnsiConsole.Prompt(prompt);

            if (option == PlaybackSwitchOption.Cancel)
            {
                return;
            }

            settingsManager.UpdatePlaybackSwitchSetting(option);
        }

        private void ChangeFallbackPlaybackDevice()
        {
            var devices = AudioDeviceUtils.GetPlaybackDevices();
            List<string> playbackOptions = new List<string>();

            foreach (PlaybackDevice device in devices)
            {
                playbackOptions.Add(device.Name);
            }

            playbackOptions.Add("Cancel");

            var prompt = new SelectionPrompt<string>()
                .Title("Choose a fallback device:")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more playback options)[/]")
                .AddChoices(playbackOptions);

            var deviceName = AnsiConsole.Prompt(prompt);

            if (deviceName.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            settingsManager.UpdateFallbackDeviceSetting(devices.Find(d => d.Name.Equals(deviceName)).Id);
        }

        private void UpdateUseFallbackDeviceSetting()
        {
            var prompt = new SelectionPrompt<UseFallbackDeviceOptions>()
              .Title("Choose an option:")
              .PageSize(10)
              .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
              .AddChoices(Enum.GetValues(typeof(UseFallbackDeviceOptions)).Cast<UseFallbackDeviceOptions>().ToArray())
              .UseConverter(settingsManager.GetUseFallbackOptionTitle);

            var option = AnsiConsole.Prompt(prompt);

            if (option == UseFallbackDeviceOptions.Cancel)
            {
                return;
            }

            settingsManager.UpdateUseFallbackDeviceSetting(option);
        }
    }
}
