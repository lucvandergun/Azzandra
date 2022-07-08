using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Azzandra
{
    public class StatsInterface : TabInterface
    {
        protected override string Title => "= Stats =";
        public Stats Stats => GameClient.Server?.User.Stats;
        private Skill? HoverSkill;

        public StatsInterface(GameClient gameClient) : base(gameClient)
        {
            
        }

        protected override void DrawAdditional(Surface surface)
        {
            ////draw info box for hover skill
            //if (HoverSkill != null)
            //{
            //    var skill = HoverSkill.Value;

            //    var box = new Rectangle(8, surface.Height - 52 - 8, surface.Width - 16, 52);
            //    Display.DrawOutline(box, new Color(63, 63, 63));
            //    Display.DrawRect(box, Color.Black);
            //    //var box2 = new Rectangle(box.X + 1, box.Y + 1, box.Width - 2, box.Height - 2);
            //    Display.DrawInline(box, new Color(127, 127, 127));
                
            //    var text = new TextDrawer(new Vector2(box.Left + 4, box.Top + 2), 16, new TextFormat(Color.White, Font, Alignment.TopLeft));

            //    text.DrawLine("<yellow>" + skill.Name.CapFirst() + " lvl: " + skill.Level);
            //    text.DrawLine("<r>Experience: " + Util.StringifyNumber2(skill.ExpDone));
            //    text.DrawLine("Remaining: " + Util.StringifyNumber2(skill.ExpReq - skill.ExpDone));
            //}
        }

        protected override void RenderSubArea(Rectangle outerRegion, bool isHoverSurface)
        {
            var surface = SubArea;

            GameClient.Engine.GraphicsDevice.SetRenderTarget(surface.Display);
            GameClient.Engine.GraphicsDevice.Clear(Color.Black);
            GameClient.Engine.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            
            if (Stats == null)
                return;

            int slotHeight = 20;
            var startOffset = new Vector2(0, 0);
            var slotSize = new Vector2(surface.Width, slotHeight);
            var slotOffset = new Vector2(0, slotHeight);
            var stringOffset = new Vector2(4, (slotHeight - Util.GetFontHeight(Font)) / 2);
            HoverSkill = null;
            var format = new TextFormat(Color.White, Font, Alignment.TopLeft, false);

            //display skills
            for (int i = 0; i < Stats.Skills.Length; i++)
            {
                var skill = Stats.Skills[i];
                string str = "<slate>"+skill.Name.CapFirst()+ ": <r>" + skill.Level;

                var hover = isHoverSurface && Input.MouseHover(new Vector2(outerRegion.X, outerRegion.Y) + surface.Position + startOffset + i * slotOffset, slotSize);
                if (hover)
                {
                    HoverSkill = skill;
                }

                TextFormatter.DrawString(startOffset + i * slotOffset + stringOffset, str, format);
            }

            GameClient.Engine.SpriteBatch.End();
            GameClient.Engine.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
