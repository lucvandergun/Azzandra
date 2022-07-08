using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    class Option
    {
        CheckBox CheckBox;
        string Title;
        string[] Text;
        Vector2 LineOffset = new Vector2(0, 16);

        public Option(CheckBox cb, string title, string[] text)
        {
            CheckBox = cb;
            Title = title;
            Text = text;
        }

        public void Render(Surface surface, Vector2 pos, GraphicsDevice gd, SpriteBatch sb)
        {
            CheckBox.Render(surface, pos - new Vector2(48, 0), gd, sb, true);
            pos -= Text.Length * LineOffset / 2;

            Display.DrawStringVCentered(pos, Title, Assets.Gridfont, true);
            pos += LineOffset;

            foreach (var line in Text)
            {
                Display.DrawStringVCentered(pos, line, Assets.Medifont, new Color(192, 192, 192), true);
                pos += LineOffset;
            }
        }
    }
    
    public class SettingsRenderer
    {
        public readonly Engine Engine;
        public readonly Settings Settings;

        private readonly Option[] Options;
        private readonly Button BackButton;
        private Vector2 ButtonSize = new Vector2(192, 32);
        private Vector2 ButtonOffset = new Vector2(0, 56);

        public SettingsRenderer(Engine engine, Settings settings)
        {
            Engine = engine;
            Settings = settings;

            var inputToggle = new Action(() => {
                Settings.DisplayInput = !Settings.DisplayInput;
                if (Engine.CurrentScene is GameClient gc)
                {
                    gc.OnResize(Engine.ScreenSize);
                    gc.DisplayHandler.TabHandler.CurrentTab = 0;
                    gc.DisplayHandler.EnvironmentInterface.OnResize(Engine.ScreenSize);
                }
                });
            Options = new Option[]
            {
                new Option(
                    new CheckBox(
                        () => {
                            Settings.AutoScaling = !Settings.AutoScaling;
                            Engine.UpdateDisplay();
                        },
                        () => Settings.AutoScaling
                    ),
                    "Auto Scaling:",
                    new string[] {
                        "Automatically scale the screen up if the window is large enough."
                    } ),
                
                new Option(
                    new CheckBox(() => Settings.RememberClientSize = !Settings.RememberClientSize, () => Settings.RememberClientSize),
                    "Remember Client Size:",
                    new string[] {
                        "Remember the current window size to apply it on the next start-up."
                    } ),

                new Option(
                    new CheckBox(inputToggle, () => Settings.DisplayInput),
                    "Display Input:",
                    new string[] {
                        "Show input buttons for the mouse at the bottom-right section.",
                        "Disabling it will render the environment tab there instead."
                    } ),

                new Option(
                    new CheckBox(() => Settings.SlidingDiagonals = !Settings.SlidingDiagonals, () => Settings.SlidingDiagonals),
                    "Sliding Diagonals:",
                    new string[] {
                        "Set this to have diagonal movements that go into solid objects,",
                        "e.g. a wall, slide you along that object."
                    } ),

                new Option(
                    new CheckBox(() => Settings.IsDebugMode = !Settings.IsDebugMode, () => Settings.IsDebugMode),
                    "Debug Mode:",
                    new string[] {
                        "Display various debug information on the screen."
                    } ),
                
                new Option(
                    new CheckBox(() => Settings.ReQueueing = !Settings.ReQueueing, () => Settings.ReQueueing), 
                    "Re-Queueing:", 
                    new string[] {
                        "Some actions are on a delay, e.g. slower attacks. Enabling this",
                        "will have the game run until such actions can be performed."
                    } )
            };

            BackButton = new Button(ButtonSize, "Back");
        }

        public void SetBackButtonEffect(Action action)
        {
            BackButton.OnClick = action;
            BackButton.OnClick = () => { action.Invoke(); Engine.SaveClientSettings(); };
        }

        private static string GetString(bool attribute)
        {
            return attribute ? "on" : "off";
        }

        public void Render(Surface surface, GraphicsDevice gd, SpriteBatch sb)
        {
            // Render buttons.

            var pos = new Vector2(surface.Width / 2 - 128, surface.Height / 2) - Options.Length * ButtonOffset / 2;
            for (int i = 0; i < Options.Length; i++)
            {
                Options[i].Render(surface, pos + i * ButtonOffset, gd, sb);
            }

            pos = new Vector2(surface.Width / 2, surface.Height / 2) + Options.Length * ButtonOffset / 2;
            BackButton.Render(surface, pos, gd, sb, true);
        }
    }
}
