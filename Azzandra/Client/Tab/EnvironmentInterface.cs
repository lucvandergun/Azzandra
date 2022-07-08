using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class EnvironmentInterface : TabInterface
    {
        protected override string Title => "= " + ((!GameClient.IsDebug || !GameClient.IsDevMode) ? "Instances in sight" : "Active instances") + " =";

        // Drawing variables:
        private readonly Vector2 LineOffset = new Vector2(0, 16);
        private readonly Vector2 InstOffset = new Vector2(10);
        private readonly Vector2 StringOffset = new Vector2(32, 10);
        private TextFormat Format = new TextFormat(Color.White, Assets.Medifont, Alignment.VCentered);

        public EnvironmentInterface(GameClient gameClient) : base(gameClient)
        {
            
        }

        protected override void RenderSubArea(Rectangle outerRegion, bool isHoverSurface)
        {
            var surface = SubArea;
            var absoluteSurfacePos = new Vector2(outerRegion.X, outerRegion.Y) + surface.Position;

            surface.SetAsRenderTarget();
            surface.Clear();
            Surface.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            if (GameClient.Server == null)
                return;

            // Slot statistics:
            int slotHeight = 20;
            var startOffset = new Vector2(0, 0);
            var slotOffset = new Vector2(0, slotHeight);
            var slotSize = new Vector2(surface.Width, slotHeight);


            // Compile list of instances to render:
            var visibleOnly = !GameClient.IsDebug || !GameClient.IsDevMode;
            var instances = visibleOnly ? GameClient.Server.User.VisibilityHandler.VisibleEnvironmentInstances
                : GameClient.Server.LevelManager.CurrentLevel.ActiveInstances;

            // Update area scroll offset:
            SubArea.FullRenderSize = new Vector2(surface.Width, slotHeight * instances.Count);
            if (GameClient.Server.User.Target != null) SubArea.FullRenderSize += LineOffset * 2;
            if (isHoverSurface) SubArea.UpdateScrollPosition();
            startOffset -= SubArea.RenderOffset;

            var pos = startOffset;


            // Display instances
            if (instances.Count > 0)
            {
                for (int i = 0; i < instances.Count; i++)
                {
                    var inst = instances[i];

                    // Mouse hover:
                    bool hover = isHoverSurface && Input.MouseHover(absoluteSurfacePos + pos, slotSize);
                    if (hover)
                    {
                        Display.DrawInline(Display.MakeRectangle(pos, slotSize), Color.White);
                        GameClient.DisplayHandler.ViewHandler.SetHoverInstance(inst);

                        // Mouse click event - set inst as target
                        if (Input.IsMouseLeftPressed)
                        {
                            GameClient.Server.User.SetTarget(inst);
                        }
                    }

                    // Render instance info
                    RenderInstanceInfo(surface, inst, ref pos, inst == GameClient.Server.User.Target, GameClient.IsDebug && GameClient.IsDevMode);
                    pos += slotOffset;
                }
            }
            else
            {
                TextFormatter.DrawString(pos + InstOffset, "<dkslate>None", Format);
            }

            Surface.SpriteBatch.End();
            Surface.GraphicsDevice.SetRenderTarget(null);
        }


        private string GetDistanceString(Instance inst)
        {
            return "<gray>(" + inst.TileDistanceTo(GameClient.Server.User.Player) + "m)<r>";
        }

        private string GetInitialString(Instance inst, bool isDebug)
        {
            var str = inst.Name.CapFirst() + " ";
            if (isDebug) str = "<slate>[ID: " + inst.ID + "]<r> " + str;
            return str;
        }


        private void RenderInstanceInfo(Surface surface, Instance inst, ref Vector2 pos, bool isTarget, bool isDebug)
        {
            // Render instance picture
            var symbol = inst.GetSymbol();
            Display.DrawInstanceString(pos + InstOffset, symbol.Char, Assets.Gridfont, symbol.Color, 1f);

            // Show target box if this inst is user's target
            if (inst == GameClient.Server.User.Target || inst == GameClient.DisplayHandler.ViewHandler.HoverInstance)
            {
                var color = inst == GameClient.Server.User.Target ? Color.Yellow : Color.White;
                Display.DrawTexture(pos + InstOffset - new Vector2(8), Assets.Target16, color);
            }

            // Render type-specific information
            if (inst is Entity cb)
                RenderCombatantInfo(surface, cb, ref pos, isTarget, isDebug);
            else
                RenderDefaultInstanceInfo(surface, inst, ref pos, isTarget, isDebug);
        }

        private void RenderCombatantInfo(Surface surface, Entity entity, ref Vector2 pos, bool isTarget, bool isDebug)
        {
            // Render name & hp
            var hpDisplay = TextFormatter.GetValueRatioColor(entity.Hp, entity.GetFullHp()) + TextFormatter.GetValueRatioColor(entity.Hp, entity.GetFullHp()) + entity.Hp + "/" + entity.GetFullHp() + "<r> ";
            var str = GetInitialString(entity, isDebug);
            if (entity.IsAttackable()) str += hpDisplay;
            str += GetDistanceString(entity);

            TextFormatter.DrawString(pos + StringOffset, str, Format);

            // Draw status effects
            if (entity.StatusEffects.Count > 0)
            {
                var effects = entity.StatusEffects.Stringify2(e => e.ToString());
                pos += LineOffset;
                TextFormatter.DrawString(pos + StringOffset, " <gray>" + effects, Format);
            }

            // Draw selected info
            if (isTarget && isDebug)
            {
                pos += LineOffset;
                TextFormatter.DrawString(pos + StringOffset, " <gray>Action: " + entity.PrevAction, Format);
                pos += LineOffset;
                TextFormatter.DrawString(pos + StringOffset, " <gray>Parent: " + entity.Parent?.ID, Format);
                pos += LineOffset;
                TextFormatter.DrawString(pos + StringOffset, " <gray>Children: " + entity.Children.Stringify(i => i.ID + ""), Format);

                if (entity is Enemy enemy)
                {
                    pos += LineOffset;
                    var str2 = " <gray>Target: " + enemy.Target;
                    TextFormatter.DrawString(pos + StringOffset, str2, Format);
                    
                    pos += LineOffset;
                    TextFormatter.DrawString(pos + StringOffset, " <gray>RetreatTimer: " + enemy.HitTimer, Format);
                }
            }
        }


        private void RenderDefaultInstanceInfo(Surface surface, Instance inst, ref Vector2 pos, bool isTarget, bool isDebug)
        {
            var str = GetInitialString(inst, isDebug) + " " + GetDistanceString(inst);
            TextFormatter.DrawString(pos + StringOffset, str, Format);
        }
    }
}
