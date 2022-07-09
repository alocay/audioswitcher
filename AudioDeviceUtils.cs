using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace AudioSwitcher
{
    public static class AudioDeviceUtils
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string powershellScript = string.Empty;

        public static List<PlaybackDevice> GetPlaybackDevices()
        {
            try
            {
                using (var powershell = PowerShell.Create())
                {
                    powershell.AddScript(GetPowershellScript()).Invoke();
                    powershell.AddCommand("Get-PlaybackDevices");

                    var devices = powershell.Invoke();
                    var playbackDevices = new List<PlaybackDevice>();

                    if (devices.Count == 0)
                    {
                        return playbackDevices;
                    }

                    foreach (var device in devices)
                    {
                        playbackDevices.Add(new PlaybackDevice(
                            device.Properties["ID"].Value.ToString(),
                            device.Properties["Name"].Value.ToString(),
                            (bool)device.Properties["Default"].Value));
                    }

                    return playbackDevices;
                }
            } catch (Exception ex)
            {
                log.Error("Error querying playback devices", ex);
                return null;
            }
        }

        public static void SetPlaybackDevice(string deviceId)
        {
            try
            {
                using (var powershell = PowerShell.Create())
                {
                    powershell.AddScript(GetPowershellScript()).Invoke();
                    powershell.AddCommand("Set-PlaybackDevice").AddParameter("DeviceId", deviceId).Invoke();

                    if (powershell.HadErrors)
                    {
                        string error = "Powershell errors: ";
                        if (powershell.Streams.Error.Count > 0)
                        {
                            error = string.Concat(error, powershell.Streams.Error[0]);
                        }
                        throw new Exception(error);
                    }
                }
            } catch (Exception ex)
            {
                log.Error("Error setting playback device", ex);
            }
        }

        private static string GetPowershellScript()
        {
            if (!string.IsNullOrWhiteSpace(powershellScript))
            {
                return powershellScript;
            }

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "AudioSwitcher.AudioUtils.ps1";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                powershellScript = reader.ReadToEnd();
            }

            return powershellScript;
        }
    }
}
