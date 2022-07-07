using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AudioSwitcher.actions
{
    public class AddMappingAction : AudioSwitcherAction
    {
        public AddMappingAction()
        {
            ActionTitle = "Add/update audio mapping";
        }

        public override void Run(AudioMappingManager mapping)
        {
            PlaybackDevice device = DisplayPlayDevicesPrompt();

            if (device == null)
            {
                return;
            }
            
            string program = GetProgram();

            if (string.IsNullOrWhiteSpace(program))
            {
                return;
            }

            mapping.AddOrUpdateMapping(program, device.Id, device.Name);
        }

        private static PlaybackDevice DisplayPlayDevicesPrompt()
        {
            var devices = AudioDeviceUtils.GetPlaybackDevices();
            List<string> playbackOptions = new List<string>();

            foreach (PlaybackDevice device in devices)
            {
                playbackOptions.Add(device.Name);
            }

            playbackOptions.Add("Cancel");

            var prompt = new SelectionPrompt<string>()
                .Title("Choose a playback device:")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more playback options)[/]")
                .AddChoices(playbackOptions);

            var deviceName = AnsiConsole.Prompt(prompt);

            return devices.Find(d => d.Name.Equals(deviceName));
        }

        private static string GetProgram()
        {
            string option = GetProgramSelectionOption();

            if (option.Equals("cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            switch (option.ToLower().Trim())
            {
                case "select the executable":
                    return GetProgramExeFileName();
                case "choose from a filtered process list":
                    string filter = GetProcessFilter();
                    return DisplayRunningProcessesPrompt(filter);
                case "input the name":
                    return DisplayGetProgramNamePrompt();
            }

            return null;
        }

        private static string GetProgramSelectionOption()
        {
            var prompt = new SelectionPrompt<string>()
                .Title("Choose an option to select the program:")
                .PageSize(5)
                .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                .AddChoices("Select the executable", "Choose from a filtered process list", "Input the name", "Cancel");

            return AnsiConsole.Prompt(prompt);
        }

        private static string DisplayGetProgramNamePrompt()
        {
            var prompt = new TextPrompt<string>("Program to map to (empty to cancel): ")
                .AllowEmpty();
            return AnsiConsole.Prompt(prompt);
        }

        private static string GetProcessFilter()
        {
            var prompt = new TextPrompt<string>("Program name case-insensitive filter (empty for all): ")
                .AllowEmpty();
            return AnsiConsole.Prompt(prompt);
        }

        private static string DisplayRunningProcessesPrompt(string filter)
        {
            var processes = Process.GetProcesses().ToList();
            var processChoices = new List<string>() { "Cancel" };
            var filteredProcessNames = string.IsNullOrWhiteSpace(filter)
                ? processes.Select(p => $"{p.ProcessName} - {p.Id}")
                : processes.Where(p => p.ProcessName.ToLower().Trim().Contains(filter)).Select(p => $"{p.ProcessName} - {p.Id}");
            processChoices = processChoices.Concat(filteredProcessNames).ToList();

            var prompt = new SelectionPrompt<string>()
               .PageSize(30)
               .MoreChoicesText("[grey](Move up and down to reveal more playback options)[/]")
               .AddChoices(processChoices);

            var choice = AnsiConsole.Prompt(prompt);
            return choice.Split('-')[0];
        }

        private static string GetProgramExeFileName()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            string filePath = fd.FileName;
            FileInfo file = new FileInfo(filePath);
            return file.Name.Replace(file.Extension, "");
        }
    }
}
