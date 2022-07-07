using Spectre.Console;
using System.Collections.Generic;

namespace AudioSwitcher.actions
{
    public class RemoveMappingAction : AudioSwitcherAction
    {
        public RemoveMappingAction()
        {
            ActionTitle = "Remove audio mapping";
        }

        public override void Run(AudioMappingManager mapping)
        {
            string process = DisplayMappingsPrompt(mapping);

            if (string.IsNullOrEmpty(process))
            {
                return;
            }

            mapping.RemoveMapping(process);
        }

        private static string DisplayMappingsPrompt(AudioMappingManager mapping)
        {
            List<string> mappingPairs = new List<string>();

            foreach (string process in mapping.Mappings.Keys)
            {
                mappingPairs.Add($"{process} - {mapping.Mappings[process]}");
            }

            mappingPairs.Add("Cancel");

            var prompt = new SelectionPrompt<string>()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more mappings)[/]")
                .AddChoices(mappingPairs.ToArray());

            var mappingPair = AnsiConsole.Prompt(prompt);

            if (mappingPair.Equals("cancel", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return mappingPair.Split('-')[0];
        }
    }
}