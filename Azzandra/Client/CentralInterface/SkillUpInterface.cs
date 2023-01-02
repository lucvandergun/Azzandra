using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class SkillUpInterface : Interface
    {
        Stats Stats => GameClient.Server?.User.Stats;

        public override bool DisableControls => true;

        private readonly Button ConfirmButton;
        private Vector2 ButtonSize = new Vector2(84, 24);
        private Vector2 ButtonOffset = new Vector2(0, 42);

        private int SelectedSkill = -1; // Skill_id / absolute skill-index
        private readonly Shrine Shrine;
        private readonly int[] SkillChoices;
        private readonly int[] LevelUpAmts;

        public SkillUpInterface(GameClient gameClient, Shrine shrine, int[] choices) : base(gameClient)
        {
            Shrine = shrine;
            SkillChoices = choices;
            LevelUpAmts = GetLevelUpAmounts(choices);
            ConfirmButton = new Button(ButtonSize, "Confirm", new ButtonFormat.SimpleDark())
            {
                OnClick = () =>
                {
                    if (SelectedSkill >= 0 && SelectedSkill < Stats.AMT_OF_SKILLS)
                    {
                        GameClient.Server.User.ShowMessage("<tan>You drink the water from the shrine, you feel invigorated.");
                        var amt = LevelUpAmts[SkillChoices.ToList().IndexOf(SelectedSkill)];
                        Stats.IncreaseLevel(SelectedSkill, amt);
                        Shrine.UseUp();
                    }
                    Close();
                },
                Text = () => SelectedSkill >= 0 && SelectedSkill < Stats.AMT_OF_SKILLS ? "Confirm" : "Cancel"
            };
            ConfirmButton.SetFocussed();
        }

        public override void OnResize(Point screenSize)
        {
            var size = new Vector2(23 * 16, 17 * 16);
            Surface?.SetSize((screenSize.ToVector2() - size) / 2, size);
        }

        public int[] GetLevelUpAmounts(int[] skills)
        {
            var levels = skills.Select(i => GameClient.Server.User.Stats.GetLevel(i));

            // == Lowest skills are +3, others +2
            //int lowest = levels.Min();
            //return levels.Select(l => l == lowest ? 3 : 2).ToArray();

            // == Skills at least 3 lower than max are +2, else +1
            int highest = levels.Max();
            int maxDiff = levels.Select(l => highest - l).Max();
            int thresh = 3;
            if (maxDiff < thresh)
            {
                return levels.Select(l => (l >= Skill.MAX_LEVEL) ? 0 : 1)
                             .ToArray();
                //var amts = new int[skills.Length];
                //amts.Populate(1);
                //return amts;
            }
            else
            {
                return levels.Select(l => (l >= Skill.MAX_LEVEL) ? 0 : (highest - l) >= thresh ? 1 : 1).ToArray();
            }
        }

        public override void Render(GraphicsDevice gd, SpriteBatch sb)
        {
            var region = Surface.Region;

            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.White * 0f);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            Display.DrawRect(Display.MakeRectangle(new Vector2(0), region.Size.ToVector2()), Color.Black);
            var lineRegion = new Rectangle(7, 7, region.Width - 14, region.Height - 14);
            Display.DrawInline(lineRegion, new Color(31, 31, 31), 2);

            int slotHeight = 20;
            var slotSize = new Vector2(region.Width, slotHeight);
            var slotOffset = new Vector2(0, 20);
            var stringOffset = new Vector2(4, slotHeight / 2);
            var format = new TextFormat(Color.White, Font, Alignment.VCentered, false);
            var titleFormat = new TextFormat(Color.White, TitleFont, Alignment.VCentered, false);

            bool isHoverSurface = GameClient.DisplayHandler.IsHoverSurface(Surface);

            var text = new TextDrawer(new Vector2(16 + 4, 16 + 10), 16, titleFormat);
            text.DrawLine("<spring>= Shrine =");
            text.Format = format;
            text.Skip();
            text.DrawLine("<white>The shrine is attuned to the following skills:");
            text.DrawLine("<slate>You may choose to attune with one of them.");
            text.Skip();

            if (Stats == null) return;

            // Display skills
            var startOffset = text.Pos - new Vector2(0, slotHeight / 2);
            text.LineH = slotHeight;
            for (int i = 0; i < SkillChoices.Length; i++)
            {
                int id = SkillChoices[i];
                var skill = Stats.Skills[id];
                string str = skill.Name.CapFirst() + ": " + skill.Level + " (+" + LevelUpAmts[i] + ")";
                string pre = "<slate>" + (i + 1) + ".<r> ";

                if (GameClient.KeyboardFocus == GameClient.Focus.Interface && Input.IsKeyPressed[Util.IntToKey(i + 1)])
                    SelectedSkill = id;

                var hover = isHoverSurface && Input.MouseHover(Surface.Position + startOffset + i * slotOffset, slotSize);
                if (hover && skill.Level < Skill.MAX_LEVEL)
                {
                    str = "<aqua>" + str;
                    if (Input.IsMouseLeftPressed)
                    {
                        SelectedSkill = SelectedSkill == id ? -1 : id;
                    }
                }

                if (id == SelectedSkill) str = "<yellow>" + str + "*";
                else if (skill.Level >= Skill.MAX_LEVEL) str = "<slate>" + str;

                //TextFormatter.DrawString(startOffset + i * slotOffset + stringOffset, pre + str, format);
                text.DrawLine(pre + str);
            }

            ConfirmButton.Render(Surface, new Vector2(region.Width - 64, region.Height - 32), gd, sb, true);
            

            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        public override void Destroy()
        {
            
        }
    }
}
