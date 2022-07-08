using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CreationStageName : IGameCreationStage
    {
        public readonly Engine Engine;
        public readonly GameCreationScene GameCreationScene;

        private Vector2 inputFieldSize = new Vector2(256, 32);

        public InputField NameInput;
        public InputField SeedInput;

        public CreationStageName(Engine engine, GameCreationScene gcs)
        {
            Engine = engine;
            GameCreationScene = gcs;

            NameInput = new InputField(Engine, inputFieldSize, "Dorrane", "Dorrane")
            {
                OnEnter = () => SeedInput.SetFocussed(),
                //NextItem = () => SeedInput
            };

            SeedInput = new InputField(Engine, inputFieldSize, null, "E.g. 1337")
            {
                OnEnter = () => GameCreationScene.NextButton.OnClick?.Invoke()
                //NextItem = () => GameCreationScene.NextButton
            };
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public string GetName()
        {
            return NameInput.GetText();
        }
        public int? GetSeed()
        {
            if (int.TryParse(SeedInput.GetText(), out var p))
                return p;
            else return null;
        }

        public void Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb, Surface surface)
        {
            // Naming part
            Display.DrawStringCentered(new Vector2(surface.Width / 2, surface.Height / 3 - 24), "Choose your name:", Assets.Gridfont);
            NameInput.Render(surface, new Vector2(surface.Width / 2, surface.Height / 3), gd, sb, true);

            // Seed part
            Display.DrawStringCentered(new Vector2(surface.Width / 2, surface.Height / 2), "Enter a seed (optional):", Assets.Gridfont);
            Display.DrawStringCentered(new Vector2(surface.Width / 2, surface.Height / 2 + 24), "(Any number.)", Assets.Medifont);
            SeedInput.Render(surface, new Vector2(surface.Width / 2, surface.Height / 2 + 24 + 30), gd, sb, true);
        }

        public bool CanContinue()
        {
            return true;
        }
    }
}
