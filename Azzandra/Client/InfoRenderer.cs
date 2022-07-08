using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class InfoRenderer
    {
        private readonly GameClient GameClient;

        private Vector2 BarSize = new Vector2(160, 6);

        protected readonly SpriteFont Font = Assets.Medifont, TitleFont = Assets.Gridfont;

        public InfoRenderer(GameClient gameClient)
        {
            GameClient = gameClient;
        }

        public void Render(Surface surface)
        {
            if (GameClient.Server == null)
                return;
            
            var user = GameClient.Server.User;

            surface.SetAsRenderTarget();
            surface.Clear();
            Surface.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            var region = surface.Region;
            TextDrawer text;

            // Draw player information
            int padding = 8;
            
            text = new TextDrawer(padding, padding, 16, Alignment.VCentered, TitleFont, Color.White, false);
            text.DrawLine(GameClient.Server.User.Name + " the " + user.Class);
            text.Font = Font;

            // Attack weapon & style:
            var weapon = user.Equipment.Items[0]?.GetNameNotNull() ?? "bare hands";
            text.DrawLine("<slate>Weapon:<r> " + weapon.CapFirst());
            var style = user.Equipment.AttackStyle;
            var speed = user.Equipment.AttackSpeed;
            var speedName = Equipment.GetSpeedName(speed);
            var rangeName = user.Equipment.AttackRange + (user.Equipment.AttackRange == 1 ? " tile" : " tiles");
            text.DrawLine("<slate>Style:<r> " + style + ", " + speedName + ", " + rangeName);
            //text.DrawLine("<slate>Speed:<r> " + speedName.CapFirst() + " (" + speed + ")");
            //text.DrawLine("<slate>Range:<r> " + user.Equipment.AttackRange);

            int hdiv = region.Width / 2 + 16 + 8;

            // Get player values:
            int hp = 1, hpFull = 1;
            int sp = 1, spFull = 1;
            int hunger = 0, hungerFull = 1;
            List<StatusEffect> se = new List<StatusEffect>();
            var player = user.Player;
            if (player != null)
            {
                hp = player.Hp;
                hpFull = player.GetFullHp();
                sp = player.Sp;
                spFull = player.GetFullSp();
                hunger = player.Hunger;
                hungerFull = player.GetFullHunger();
                se = player.StatusEffects;
            }

            // Effects:
            if (se.Count > 0)
            {
                //text.MoveLine();
                text.Draw("Status: ");
                for (int j = 0; j < se.Count; j++)
                {
                    var eff = se[j];
                    var str = eff.ToColorString();
                    var length = Util.GetStringWidth(TextFormatter.FormatString(str).FirstOrDefault(s => !TextFormatter.IsFormatCode(s)), Font);
                    if (text.CurrentPos.X + length >= hdiv - 5)
                        text.MoveLine();
                    text.Draw(str);
                    if (j < se.Count - 1)
                        text.Draw("<r>, ");
                }
                //text.DrawLine(se.GetRange(0, Math.Min(se.Count, 2)).Stringify3(s => "<purple>" + s.ToString() + s.GetRomanLevel() + "<r>"));
                //if (se.Count >= 3)
                //    text.DrawLine(se.GetRange(2, Math.Min(se.Count - 2, 2)).Stringify3(s => "<purple>" + s.ToString() + s.GetRomanLevel() + "<r>"));
            }


            // Right side:
            text.SetPosition(hdiv, 8);
            text.LastColor = Color.White;


            // Draw player values:
            int value, barSize = 10;

            value = CalculateShownValue(hp, hpFull, barSize);
            text.DrawLine("HP: " + hp + "/" + hpFull + " " + CreateValueDisplayString(value, barSize, "<lime>", "<red>") + "<r>");

            value = CalculateShownValue(sp, spFull, barSize);
            text.DrawLine("SP: " + sp + "/" + spFull + " " + CreateValueDisplayString(value, barSize, "<yellow>", "<red>") + "<r>");

            value = CalculateShownValue(hunger, hungerFull, barSize);
            text.DrawLine("Hunger: " + hunger + "/" + hungerFull); // + " " + CreateValueDisplayString(value, barSize, "<red>", "<yellow>") + "<r>"



            //var equipment = user.Equipment;
            //var accSkill =
            //    equipment.AttackStyle == Style.Ranged ? user.Stats.GetLevel(SkillID.Ranged)
            //    : equipment.AttackStyle == Style.Magic ? user.Stats.GetLevel(SkillID.Magic)
            //    : user.Stats.GetLevel(SkillID.Attack);
            //var dmgSkill =
            //    equipment.AttackStyle == Style.Ranged ? (user.Stats.GetLevel(SkillID.Ranged) + user.Stats.GetLevel(SkillID.Strength))/2
            //    : equipment.AttackStyle == Style.Magic ? user.Stats.GetLevel(SkillID.Magic)
            //    : (user.Stats.GetLevel(SkillID.Attack) + user.Stats.GetLevel(SkillID.Strength)) / 2;
            //var parSkill = (user.Stats.GetLevel(SkillID.Attack) + user.Stats.GetLevel(SkillID.Defense)) / 2;

            var stats = user.Stats;
            int i = 0;
            foreach (var skill in stats.Skills)
            {
                text.DrawLine("<slate>" + skill.ShortName.ToUpper() + ":<r> " + skill.Level);
                i++;
                if (i == 3)
                    text.SetPosition(region.Width / 4 * 3, 32 + 8 + 16);
            }

            Surface.SpriteBatch.End();
            Surface.GraphicsDevice.SetRenderTarget(null);
        }

        private string CreateStatString(int level, int bonus)
        {
            return (level + bonus) + " <dkslate>(" + bonus + "+" + level + ")<r>";
        }

        /// <summary>
        /// Calculates size of visible fraction of specified value relation.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="fullValue"></param>
        /// <param name="scaledFullValue"></param> What the specified full value should be scaled to.
        /// <returns></returns>
        private int CalculateShownValue(int currentValue, int fullValue, int scaledFullValue)
        {
            return (currentValue <= 0) ? 0 : Math.Min(scaledFullValue, Math.Max(1, (int)((float)currentValue / fullValue * 10f)));
        }

        private string CreateValueDisplayString(int value1, int value2, string color1, string color2)
        {
            string str = color1;
            char c = '*';

            for (int i = 0; i < value1; i++) str += c;
            str += color2;
            for (int i = 0; i < value2 - value1; i++) str += c;

            return str;
        }


        private void DrawBar(Vector2 pos, int value, int fullValue, Color emptyColor, Color fullColor)
        {
            //draw empty bar
            var barRect = new Rectangle(pos.ToPoint(), BarSize.ToPoint());
            Display.DrawRect(barRect, emptyColor);
            Display.DrawOutline(barRect, Color.White);

            //calculate full bar width
            float amt = (float)value / (float)fullValue * BarSize.X;
            int filledWidth = amt <= 0 ? 0 : amt <= 1 ? 1 : (int)amt;

            var filledSize = new Vector2(filledWidth, BarSize.Y);
            var filledRect = new Rectangle(pos.ToPoint(), filledSize.ToPoint());
            Display.DrawRect(filledRect, fullColor);
            Display.DrawInline(filledRect, Color.Black);
        }
    }
}
