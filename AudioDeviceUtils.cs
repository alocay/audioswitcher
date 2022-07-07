using log4net;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace AudioSwitcher
{
    public static class AudioDeviceUtils
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<PlaybackDevice> GetPlaybackDevices()
        {
            try
            {
                using (var powershell = PowerShell.Create())
                {
                    powershell.AddScript(@"Import-Module .\AudioUtils.ps1").Invoke();
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
                    powershell.AddScript(@"Import-Module .\AudioUtils.ps1").Invoke();
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
    }
}
