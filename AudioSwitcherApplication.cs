using System;
using Spectre.Console;
using System.Collections.Generic;
using log4net;
using AudioSwitcher.actions;
using System.Linq;
using AudioSwitcher.settings;
using Figgle;
using System.Reflection;

namespace AudioSwitcher
{
    public class AudioSwitcherApplication
    {
        public static AudioMappingManager AudioMapping;
        public static SettingsManager SettingsManager;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static List<AudioSwitcherAction> actions;

        [STAThread]
        public static void Main(string[] args)
        {
            Setup();
            RenderTitle();
            WaitForUserInput();
        }

        private static void Setup()
        {
            AudioMapping = new AudioMappingManager();
            SettingsManager = new SettingsManager();
            SettingsManager.LoadSettings();
            ProcessListener listener = new ProcessListener(SettingsManager);

            actions = new List<AudioSwitcherAction>
            {
                new ListPlaybackAction(),
                new ViewMappingsAction(),
                new AddMappingAction(),
                new RemoveMappingAction(),
                new ViewSettingsAction(SettingsManager),
                new ChangeSettingsAction(SettingsManager),
                new ExitAction(listener, SettingsManager)
            };

            listener.Listen();
        }

        private static void RenderTitle()
        {
            var titleTable = new Table();
            titleTable.Border = TableBorder.Horizontal;

            titleTable.AddColumn(FiggleFonts.Larry3d.Render("Audio Switcher"));

            AnsiConsole.Write(titleTable);
        }

        private static void WaitForUserInput()
        {
            string userChoice = "";
            while (true)
            {
                userChoice = GetUserAction();

                foreach (var action in actions)
                {
                    if (action.IsSameAction(userChoice))
                    {
                        action.Run(AudioMapping);
                    }
                }
            }
        }

        private static string GetUserAction()
        {
            var prompt = new SelectionPrompt<string>()
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more actions)[/]")
                .AddChoices(actions.Select(a => a.ActionTitle));

            return AnsiConsole.Prompt(prompt);
        }
    }
}
