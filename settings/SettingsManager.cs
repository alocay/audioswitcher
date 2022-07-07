using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;

namespace AudioSwitcher.settings
{
    public class SettingsManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string settingsFile = "settings.json";

        public AudioSwitcherSettings Settings { get; set; }

        public event EventHandler SettingChanged;

        public void SaveSettings()
        {
            try
            {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
                {
                    var stream = isoStore.CreateFile(settingsFile);

                    using (StreamWriter file = new StreamWriter(stream))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(file, Settings);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error saving the settings", ex);
            }
        }

        public AudioSwitcherSettings LoadSettings()
        {
            try
            {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null))
                {
                    if (!isoStore.FileExists(settingsFile))
                    {
                        Settings = new AudioSwitcherSettings();
                        return Settings;
                    }

                    var stream = isoStore.OpenFile(settingsFile, FileMode.Open);

                    using (StreamReader file = new StreamReader(stream))
                    {
                        string json = file.ReadToEnd();
                        Settings = JsonConvert.DeserializeObject<AudioSwitcherSettings>(json);
                        return Settings;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error loading the settings", ex);
                Settings = new AudioSwitcherSettings();
                return Settings;
            }
        }

        public List<string> GetAllSettingTitles()
        {
            return Enum.GetValues(typeof(Setting)).Cast<Setting>().Select(s => GetSettingTitle(s)).ToList();
        }

        public List<Setting> GetAllSettings()
        {
            return Enum.GetValues(typeof(Setting)).Cast<Setting>().ToList();
        }

        public string GetSettingTitle(Setting setting)
        {
            switch (setting)
            {
                case Setting.SwitchOnProcessStart:
                    return "Switch playback device only on process start";
                case Setting.FallbackDevice:
                    return "Change fallback device when no mapping is found";
                case Setting.UseFallbackDevice:
                    return "Switch to fallback playback device when no mapping is found";
            }

            return null;
        }

        public string GetPlaybackSwitchSettingTitle(PlaybackSwitchOption option)
        {
            switch (option)
            {
                case PlaybackSwitchOption.ProcessStart:
                    return "Only switch on program start";
                case PlaybackSwitchOption.ProcessFocus:
                    return "Only switch when program is in the foreground (has focus)";
            }

            return string.Empty;
        }

        public string GetUseFallbackOptionTitle(UseFallbackDeviceOptions option)
        {
            switch (option)
            {
                case UseFallbackDeviceOptions.UseFallback:
                    return "Switch to fallback playback device";
                case UseFallbackDeviceOptions.DontUseFallback:
                    return "Don't switch to fallback playback device";
            }

            return string.Empty;
        }

        public string GetSettingValueString(Setting setting)
        {
            switch(setting)
            {
                case Setting.SwitchOnProcessStart:
                    return Settings.ShouldSwitchOnlyOnProcessStart.ToString();
                case Setting.FallbackDevice:
                    var devices = AudioDeviceUtils.GetPlaybackDevices();
                    string name = devices.Find(d => d.Id.Equals(Settings.FallbackPlaybackDevice))?.Name;
                    return string.IsNullOrWhiteSpace(name) ? "None" : name;
                case Setting.UseFallbackDevice:
                    return Settings.UseFallbackDevice.ToString();
            }

            return string.Empty;
        }

        public void UpdatePlaybackSwitchSetting(PlaybackSwitchOption option)
        {
            Settings.ShouldSwitchOnlyOnProcessStart = option == PlaybackSwitchOption.ProcessStart;
            SettingChanged?.Invoke(this, EventArgs.Empty);
            SaveSettings();
        }

        public void UpdateFallbackDeviceSetting(string deviceId)
        {
            Settings.FallbackPlaybackDevice = deviceId;
            SettingChanged?.Invoke(this, EventArgs.Empty);
            SaveSettings();
        }

        public void UpdateUseFallbackDeviceSetting(UseFallbackDeviceOptions option)
        {
            Settings.UseFallbackDevice = option == UseFallbackDeviceOptions.UseFallback;
            SettingChanged?.Invoke(this, EventArgs.Empty);
            SaveSettings();
        }
    }
}
