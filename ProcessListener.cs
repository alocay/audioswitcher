using AudioSwitcher.settings;
using System;
using System.Management;
using System.Runtime.InteropServices;

namespace AudioSwitcher
{
    public class ProcessListener
    {
        private ManagementEventWatcher startWatch;
        private EventArrivedEventHandler arrivedEventHandler;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private WinEventDelegate procDelegate;
        private IntPtr hhook = IntPtr.Zero;
        private SettingsManager settingsManager;

        public ProcessListener(SettingsManager settings)
        {
            settingsManager = settings;
            settingsManager.SettingChanged += Settings_SettingChanged;
            procDelegate = new WinEventDelegate(WinEventProc);
        }

        private void Settings_SettingChanged(object sender, EventArgs e)
        {
            if (settingsManager.Settings.ShouldSwitchOnlyOnProcessStart)
            {
                if (startWatch == null)
                {
                    SetupProcessStartListener();
                }

                if (hhook != IntPtr.Zero)
                {
                    SetupProcessFocusListener();
                }
            }
        }

        public void Listen()
        {
            if (settingsManager.Settings.ShouldSwitchOnlyOnProcessStart)
            {
                SetupProcessStartListener();
            }
            else
            {
                SetupProcessFocusListener();
            }
        }

        public void Stop()
        {
            StopProcessStartListener();
            StopProcessFocusListener();
        }

        private void SetupProcessStartListener()
        {
            startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            arrivedEventHandler = new EventArrivedEventHandler(StartWatch_EventArrived);
            startWatch.EventArrived += arrivedEventHandler;
            startWatch.Start();
        }

        private void StopProcessStartListener()
        {
            if (startWatch != null)
            {
                startWatch.Stop();
                startWatch.EventArrived -= arrivedEventHandler;
                startWatch = null;
            }
        }

        private void SetupProcessFocusListener()
        {
            hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                    procDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        private void StopProcessFocusListener()
        {
            if (hhook != IntPtr.Zero)
            {
                UnhookWinEvent(hhook);
            }
        }

        private void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            bool found = false;
            foreach (var processName in AudioSwitcherApplication.AudioMapping.Mappings.Keys)
            {
                if (e.NewEvent.Properties["ProcessName"].Value.ToString().StartsWith(processName, StringComparison.InvariantCultureIgnoreCase))
                {
                    AudioDeviceUtils.SetPlaybackDevice(AudioSwitcherApplication.AudioMapping.Mappings[processName].Id);
                    found = true;
                }
            }

            if (!found)
            {
                SwitchToFallback();
            }
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            bool found = false;
            var foregroundProcess = GetForegroundProcessName();

            foreach (var processName in AudioSwitcherApplication.AudioMapping.Mappings.Keys)
            {
                if (foregroundProcess.StartsWith(processName, StringComparison.InvariantCultureIgnoreCase))
                {
                    AudioDeviceUtils.SetPlaybackDevice(AudioSwitcherApplication.AudioMapping.Mappings[processName].Id);
                    found = true;
                }
            }

            if (!found)
            {
                SwitchToFallback();
            }
        }

        private string GetForegroundProcessName()
        {
            IntPtr hwnd = GetForegroundWindow();

            if (hwnd == null)
                return null;

            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);

            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.Id == pid)
                {
                    return p.ProcessName;
                }
            }

            return null;
        }

        private void SwitchToFallback()
        {
            string deviceId = settingsManager.Settings.FallbackPlaybackDevice;

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                return;
            }

            AudioDeviceUtils.SetPlaybackDevice(deviceId);
        }
    }
}
