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
    public class GameCreationScene : IScene
    {
        public readonly Engine Engine;
        private Surface Surface;

        public Button BackButton { get; private set; }
        public Button NextButton { get; private set; }

        protected Vector2 ButtonSize = new Vector2(96, 48);

        // Stage handlers
        private int StageIndex;  // the current stage
        private CreationStageName NameStage;
        private CreationStageClass ClassStage;
        private CreationStageStats StatsStage;
        private IGameCreationStage[] Stages; // this holds the order
        public IGameCreationStage CurrentStage => Stages.Length >= StageIndex + 1 ? Stages[StageIndex] : null;


        public GameCreationScene(Engine engine, Point screenSize)
        {
            Engine = engine;
            Surface = new Surface(0, 0, screenSize.X, screenSize.Y, Engine);

            StageIndex = 0;
            NameStage = new CreationStageName(engine, this);
            ClassStage = new CreationStageClass(engine, this);
            StatsStage = new CreationStageStats(engine, this);
            Stages = new IGameCreationStage[]
            {
                NameStage,
                ClassStage,
                StatsStage
            };

            // Set back button
            BackButton = new Button(ButtonSize, "Back")
            {
                OnClick = () => {
                    if (StageIndex == 0)
                        Engine.SetScene(new MenuScene(engine, screenSize));
                    else
                        StageIndex--;
                },
                Text = () =>
                {
                    return StageIndex == 0 ? "Return to\nMenu" : "Back";
                }
            };

            // Set next button
            NextButton = new Button(ButtonSize, "Confirm")
            {
                CanInteract = () => CurrentStage.CanContinue(),
                OnClick = () => {
                    if (CurrentStage.CanContinue())
                    {
                        if (StageIndex == Stages.Length - 1)
                        {
                            CreateGameFile();
                        }
                        else
                            StageIndex++;
                    }
                },
                Text = () =>
                {
                    return StageIndex == Stages.Length - 1 ? "Create\nGame" : "Confirm";
                }
            };
        }

        /// <summary>
        /// Creates the gamefile with the current player settings and saves it to the disk.
        /// </summary>
        private void CreateGameFile()
        {
            var saveFile = "save1.dat";
            var game = new GameClient(Engine, Engine.ScreenSize, saveFile);
            game.CreateNewGame(NameStage.GetName(), ClassStage.GetClass(), StatsStage.GetLevels(), NameStage.GetSeed());
            game.SaveGame();
            
            Engine.SetScene(new StoryScene(Engine, Engine.ScreenSize, saveFile));
        }

        public void Update(GameTime gameTime)
        {
            CurrentStage?.Update(gameTime);
        }

        public void OnResize(Point screenSize)
        {
            Surface.SetSize(screenSize.X, screenSize.Y);
        }

        public RenderTarget2D Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb)
        {
            gd.SetRenderTarget(Surface.Display);
            gd.Clear(Color.Black);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            CurrentStage?.Render(gameTime, gd, sb, Surface);

            BackButton.Render(Surface, new Vector2(Surface.Width / 2 - 350, Surface.Height - 64), gd, sb, true);
            NextButton.Render(Surface, new Vector2(Surface.Width / 2 + 350, Surface.Height - 64), gd, sb, true);

            sb.End();
            gd.SetRenderTarget(null);

            return Surface.Display;
        }

        public void Exit()
        {
            // Nothing happens here.
        }
    }
}
