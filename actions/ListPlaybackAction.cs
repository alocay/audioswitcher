using Spectre.Console;

namespace AudioSwitcher.actions
{
    public class ListPlaybackAction : AudioSwitcherAction
    {
        public ListPlaybackAction()
        {
            ActionTitle = "List playback devices";
        }

        public override void Run(AudioMappingManager mapping)
        {
            var devices = AudioDeviceUtils.GetPlaybackDevices();
            var table = new Table();
            table.AddColumn("Playback Device");
            table.AddColumn("Current Output");

            foreach (PlaybackDevice device in devices)
            {
                TableExtensions.AddRow(table, new string[] { device.Name, device.IsDefault.ToString() });
            }

            AnsiConsole.Write(table);
        }
    }
}
