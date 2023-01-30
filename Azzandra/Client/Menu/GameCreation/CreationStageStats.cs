using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CreationStageStats : IGameCreationStage
    {
        private class SkillAssigner
        {
            public readonly string ID;
            public Button DecrButton, IncrButton;
            public static int MIN_LEVEL = 1, MAX_LEVEL = 4;
            public int Level = MIN_LEVEL;

            public SkillAssigner(string id)
            {
                ID = id;
            }
        }

        public readonly Engine Engine;
        public readonly GameCreationScene GameCreationScene;

        public static int MAX_POINTS = 9;
        public int PointsLeft { get; set; } = MAX_POINTS;
        private SkillAssigner[] Skills;
        private Vector2 assignButtonSize = new Vector2(16);
        private Button RandomButton;
        protected Vector2 ButtonSize = new Vector2(96, 48);

        private string[] SkillDescs = new string[]
        {
            "For better use of close-combat weapons.",
            "For better use of bows and crossbows.",
            "For better use of wands, staves, spells and more spellpoints.",
            "For blocking and parrying attacks.",
            "For avoiding getting hit by attacks.", // and spells.
            "For more health and better food side-effect tolerance."
        };

        public CreationStageStats(Engine engine, GameCreationScene gcs)
        {
            Engine = engine;
            GameCreationScene = gcs;

            var skillNames = SkillID.Names;
            Skills = new SkillAssigner[skillNames.Length];
            for (int i = 0; i < skillNames.Length; i++)
            {
                var skill = new SkillAssigner(skillNames[i]);
                skill.DecrButton = new Button(assignButtonSize, "-")
                {
                    OnClick = () =>
                    {
                        if (skill.Level > SkillAssigner.MIN_LEVEL)
                        {
                            skill.Level--;
                            PointsLeft++;
                        }
                    }
                };

                skill.IncrButton = new Button(assignButtonSize, "+")
                {
                    OnClick = () =>
                    {
                        if (PointsLeft > 0 && skill.Level < SkillAssigner.MAX_LEVEL)
                        {
                            PointsLeft--;
                            skill.Level++;
                        }
                    }
                };

                Skills[i] = skill;
            }
            RandomButton = new Button(ButtonSize, "Confirm")
            {
                OnClick = () =>
                {
                    // Reset distributed points and appoint them at random.
                    PointsLeft = MAX_POINTS;
                    foreach (var s in Skills)
                        s.Level = 1;
                    var skills = Skills.CreateCopy();
                    while (PointsLeft > 0)
                    {
                        var s = skills.Where(i => i.Level < SkillAssigner.MAX_LEVEL).ToList().PickRandom();
                        s.Level++;
                        PointsLeft--;
                    }
                },
                Text = () => "Randomize"
            };
        }
        
        public void Update(GameTime gameTime)
        {
            
        }

        public int[] GetLevels()
        {
            return Skills.Select(l => l.Level).ToArray();
        }

        public void Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb, Surface surface)
        {
            // Skills area is 14(x16) wide by 7(x24) high
            int w = 26 * 16, h = Skills.Length * 24;

            Display.DrawStringCentered(new Vector2(surface.Width / 2, surface.Height / 3 - 24), "Choose your skillset:", Assets.Gridfont);

            var pos = new Vector2((surface.Width - w) / 2, (surface.Height - h) / 2 + 32);
            var offset = new Vector2(0, 24);

            Display.DrawStringVCentered(pos + new Vector2(w - 64, 0) - offset, "Points left: " + PointsLeft, Assets.Medifont);

            var format = new TextFormat(Color.White, Assets.Medifont, Alignment.VCentered, false);

            for (int i = 0; i < Skills.Length; i++)
            {
                var skill = Skills[i];
                //TextFormatter.DrawString(pos + i * offset + new Vector2(w / 2, 0), "<slate>" + SkillDescs[i], format);
                //Display.DrawStringVCentered(pos + i * offset + new Vector2(w / 2 + 10, 0), skill.ID.CapFirst() + ":", Assets.Medifont);
                //Display.DrawStringCentered(pos + i * offset + new Vector2(w / 2 + 80, 0), skill.Level + "", Assets.Medifont);
                //skill.DecrButton.Render(surface, pos + i * offset + new Vector2(w / 2 + 110, 0), gd, sb, true);
                //skill.IncrButton.Render(surface, pos + i * offset + new Vector2(w / 2 + 130, 0), gd, sb, true);


                TextFormatter.DrawString(pos + i * offset, skill.ID.CapFirst() + ":", format);
                Display.DrawStringCentered(pos + i * offset + new Vector2(70, 0), skill.Level + "", Assets.Medifont);
                skill.DecrButton.Render(surface, pos + i * offset + new Vector2(100, 0), gd, sb, true);
                skill.IncrButton.Render(surface, pos + i * offset + new Vector2(120, 0), gd, sb, true);
                TextFormatter.DrawString(pos + i * offset + new Vector2(150, 0), "<slate>" + SkillDescs[i], format);
            }

            RandomButton.Render(surface, new Vector2(surface.Width / 2 + 350 - 128, surface.Height - 64), gd, sb, true);
        }

        public bool CanContinue()
        {
            return PointsLeft == 0;
        }
    }
}
