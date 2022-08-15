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
    public class StoryScene : IScene
    {
        public readonly Engine Engine;
        private Surface Surface;
        private string SaveFile;

        private Button BackButton, SkipButton, NextButton;
        private Vector2 ButtonSize = new Vector2(96, 48);
        private Vector2 TextSize = new Vector2(400, 200);

        private int StageIndex;
        private string[] Texts;
        private const int AMT_OF_TEXTS = 8;
        private string CurrentText => StageIndex >= Texts.Length ? "Null" : Texts[StageIndex];

        private MenuBackgroundRenderer Bg;

        public StoryScene(Engine engine, Point screenSize, string saveFile)
        {
            Engine = engine;
            Surface = new Surface(0, 0, screenSize.X, screenSize.Y, Engine);
            SaveFile = saveFile;

            StageIndex = 0;
            Texts = new string[]
            {
                "The Caverns of Azzandra was once the greatest mining facility of the continent. For decades, it's ores and gems were gathered and spread far across the lands as their purity was unparalleled.",
                "But, due to the extent of the excessive mining the upper levels were quickly depleted and the workers went ever deeper. They wouldn't stop as their tyrant king's lust for money and power were insatiable.\nYou must have sensed what happens next...",
                "Their greed became their own destruction when they descended to depths they shouldn't have. They discovered they had done something irreversible.",
                "In ancient times the archaic demon Azzandra was terrorizing the world in an attempt to eliminate all that is good. After a long battle he was put to sleep under a spell in the very same caverns, deep below the surface. Over the ages life had been good, and the people had forgotten that such horror once existed.",
                "Azzandra was immobilized, but as he was so powerful, he could still slowly alter his environment to his desires. He set up a trap by means of growing the most precious treasures from the earth around him, so that someone would eventually come along and break the seal that had been his barrier.",
                "This inevitably happened and he awoke. There was now nothing but his diminished strength standing between him and the surface world.",
                "The king was easily startled and fled, never to be seen again. Over the years his kingdom decayed, unable to function without its ruler. And without a centralized force, there was no courage left.",
                "No-one dared setting foot inside the caverns, while Azzandra was commencing his return. Now, his bane is almost anew. Someone should face him or all that history has come to will vanish forever!"
            };

            // Set back button
            BackButton = new Button(ButtonSize, "Previous")
            {
                OnClick = () => {
                    if (StageIndex == 0)
                        Engine.SetScene(new MenuScene(engine, screenSize));
                    else
                        StageIndex--;
                },
                Text = () =>
                {
                    return StageIndex == 0 ? "Return to\nMenu" : "Previous";
                }
            };

            // Set skip button
            SkipButton = new Button(ButtonSize, "Skip")
            {
                OnClick = () => {
                    StartGame();
                },
            };

            // Set next button
            NextButton = new Button(ButtonSize, "Next")
            {
                OnClick = () => {
                    if (StageIndex == AMT_OF_TEXTS - 1)
                    {
                        StartGame();
                    }
                    else
                        StageIndex++;
                },
                Text = () =>
                {
                    return StageIndex == AMT_OF_TEXTS - 1 ? "Start" : "Next";
                }
            };

            Bg = new MenuBackgroundRenderer(engine, new Rectangle(Point.Zero, screenSize));
        }

        private void StartGame()
        {
            var game = new GameClient(Engine, Engine.ScreenSize, SaveFile);
            game.LoadGame();
            Engine.SetScene(game);
        }

        public void Update(GameTime gameTime)
        {
            Bg.Update();
        }

        public void OnResize(Point screenSize)
        {
            Surface.SetSize(screenSize.X, screenSize.Y);
            Bg.OnResize(new Rectangle(Point.Zero, screenSize));
        }

        public RenderTarget2D Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb)
        {
            gd.SetRenderTarget(Surface.Display);
            gd.Clear(Color.Black);

            Bg.Render(gd, sb);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            var rect = new Rectangle((Surface.Size / 2 - TextSize / 2).ToPoint(), TextSize.ToPoint());
            Display.DrawRect(rect, Color.Black);
            Display.DrawInline(rect, new Color(31, 31, 31), 2);

            Display.DrawStringCentered(new Vector2(rect.Center.X, rect.Top + 19), "Story", Assets.Gridfont, Color.Goldenrod);

            var lines = Util.SeparateString(CurrentText, Assets.Medifont, (int)TextSize.X - 32);
            var pos = Surface.Size / 2 - new Vector2(0, (lines.Length - 1) * 16 / 2);
            var text = new TextDrawer(pos, 16, new TextFormat(Color.White, Assets.Medifont, Alignment.Centered, true));
            foreach (var line in lines)
                text.DrawLine(line);

            Display.DrawStringCentered(new Vector2(rect.Right - 32, rect.Bottom - 18), "("+(StageIndex+1)+"/"+Texts.Length+")" , Assets.Medifont);
            //Display.DrawStringCentered(new Vector2(screenWidth / 2, screenHeight / 2), CurrentText, Assets.Medifont, true);

            BackButton.Render(Surface, new Vector2(Surface.Width / 2 - 350, Surface.Height - 64), gd, sb, true);
            if (StageIndex < Texts.Length - 1)
                SkipButton.Render(Surface, new Vector2(Surface.Width / 2 + 350 - 128, Surface.Height - 64), gd, sb, true);
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
