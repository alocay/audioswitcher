using Spectre.Console;

namespace AudioSwitcher.actions
{
    public class ViewMappingsAction : AudioSwitcherAction
    {
        public ViewMappingsAction()
        {
            ActionTitle = "View mappings";
        }

        public override void Run(AudioMappingManager mapping)
        {
            var table = new Table();
            table.AddColumn("Program");
            table.AddColumn("Playback Device");

            if (mapping.Mappings.Count == 0)
            {
                table.AddEmptyRow();
            }
            else
            {
                foreach (var map in mapping.Mappings)
                {
                    TableExtensions.AddRow(table, new string[] { map.Key, map.Value.Name });
                }
            }

            AnsiConsole.Write(table);
        }
    }
}