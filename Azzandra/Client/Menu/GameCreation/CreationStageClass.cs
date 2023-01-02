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
    public class CreationStageClass : IGameCreationStage
    {
        private class ClassInfo
        {
            public string Asset { get; set; }
            public string Name { get; set; }
            public Color Color { get; set; }
            public string[] Info { get; set; }

            public string GetSymbol() => Name?.First().ToString() ?? "?";
            public Button Button { get; set; }
        }
        
        
        public readonly Engine Engine;
        public readonly GameCreationScene GameCreationScene;

        private Vector2 buttonSize = new Vector2(48);

        private ClassInfo[] Classes;
        private int SelectedClass;
        private ClassInfo CurrentClassInfo => SelectedClass >= 0 && SelectedClass < Classes.Length ? Classes[SelectedClass] : null;

        public CreationStageClass(Engine engine, GameCreationScene gcs)
        {
            Engine = engine;
            GameCreationScene = gcs;

            SelectedClass = -1;

            // List possible the classes
            Classes = new ClassInfo[]
            {
                new ClassInfo()
                {
                    Asset = "player_knight",
                    Name = "Knight",
                    Color = new Color(255, 35, 0),
                    Info = new string[]
                    {
                        "Preferring to advance in close combat, the knight is a just warrior good at facing enemies in a balanced way.",
                        "<ltgray>+20% Melee Accuracy",
                        "+20% Blocking and Parrying",
                        "Starts with a sword and shield.",
                        "Is able to cast wind blast and weaken."
                    }
                },
                new ClassInfo()
                {
                    Asset = "player_rogue",
                    Name = "Rogue",
                    Color = new Color(0, 204, 0),
                    Info = new string[]
                    {
                        "Lurking from the shadows, the rogue is able to stay out of trouble with his superior ranging and evading skill set.",
                        "<ltgray>+20% Ranged Accuracy",
                        "+20% Evasion",
                        "Starts with a bow.",
                        "Is able to cast dash and disorient."
                    }
                },
                new ClassInfo()
                {
                    Asset = "player_wizard",
                    Name = "Wizard",
                    Color = new Color(113, 200, 247),
                    Info = new string[]
                    {
                        "Having researched the very essence of matter itself, the wizard has achieved a grasp on all magical abilities.",
                        "<ltgray>+20% Magic Accuracy and Damage",
                        "+20% Spellcast and spell Resistance",
                        "Starts with a staff.",
                        "Is able to cast freeze and lightning."
                    }
                },
                new ClassInfo()
                {
                    Asset = "player_barbarian",
                    Name = "Barbarian",
                    Color = new Color(255, 215, 0),
                    Info = new string[]
                    {
                        "With the courage of a mighty bear, the barbarian charges forward fearlessly with superhuman physical prowess.",
                        "<ltgray>+20% Melee and Ranged Damage",
                        "+25% Health",
                        "Starts with a war axe.",
                        "Is able to cast charge and whirlwind."
                    }
                },
                new ClassInfo()
                {
                    Asset = "player_priest",
                    Name = "Priest",
                    Color = new Color(130, 92, 214),
                    Info = new string[]
                    {
                        "Living life by serving a higher purpose, the priest is unprecedented in remaining unaffected by malicious powers.",
                        "<ltgray>+40% spell Resistance",
                        "Starts with a mace and buckler.",
                        "Is able to cast cure and deflect."
                    }
                }
            };

            for (int i = 0; i < Classes.Length; i++)
            {
                var index = i;
                Classes[i].Button = new Button(buttonSize, null)
                {
                    OnClick = () => {
                        if (SelectedClass != index)
                            SelectedClass = index;
                    },
                    IsSelected = () => SelectedClass == index,
                    TextColor = Classes[i].Color,
                    TextColorHover = Color.White,
                    AnimationManager = new AnimationManager(Classes[i].Asset)
                };
            }
        }

        public void Update(GameTime gameTime)
        {
            Debug.WriteLine(SelectedClass);
        }

        public int GetClass()
        {
            return SelectedClass;
        }

        public void Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb, Surface surface)
        {
            Display.DrawStringCentered(new Vector2(surface.Width / 2, surface.Height / 3 - 24), "Pick your class:", Assets.Gridfont);
            Display.DrawStringCentered(new Vector2(surface.Width / 2, surface.Height / 3), "Choose wisely, as you cannot change this later! ", Assets.Medifont);

            // Render class buttons
            var offset = new Vector2(112, 0);
            var pos = new Vector2(surface.Width / 2, surface.Height / 2) - (Classes.Length - 1) * offset / 2;
            for (int i = 0; i < Classes.Length; i++)
            {
                Classes[i].Button.Render(surface, pos + i * offset, gd, sb, true);
            }

            // Render selected class info
            var info = CurrentClassInfo;
            if (info != null)
            {
                var text = new TextDrawer(
                        new Vector2(surface.Width / 2, surface.Height / 3 * 2),
                        16,
                        new TextFormat(info.Color, Assets.Gridfont, Alignment.Centered)
                    );

                text.DrawLine("- " + info.Name + " -");
                text.Font = Assets.Medifont;
                text.DefaultColor = Color.White;
                text.LastColor = Color.White;
                for (int i = 0; i < info.Info.Length; i++)
                    text.DrawLine(info.Info[i]);
            }
        }

        public bool CanContinue()
        {
            return SelectedClass != -1;
        }
    }
}
