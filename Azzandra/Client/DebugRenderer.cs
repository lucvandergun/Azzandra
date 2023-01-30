using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DebugRenderer
    {
        private readonly GameClient GameClient;

        protected readonly SpriteFont Font = Assets.Medifont, TitleFont = Assets.Gridfont;

        public DebugRenderer(GameClient gameClient)
        {
            GameClient = gameClient;
        }

        public void Render(Surface surface, GameTime gameTime)
        {
            surface.SetAsRenderTarget();
            surface.Clear(Color.Black * 0f);
            Surface.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            

            var region = surface.Region;
            var display = GameClient.DisplayHandler;
            var lm = GameClient.Server.LevelManager;
            var level = GameClient.Server.LevelManager.CurrentLevel;

            // Left side:
            var text = new TextDrawer(4, 8, 16, Alignment.VCentered, Assets.Medifont, Color.White, true);
            text.ResetColorOnCall = true;

            //engine data
            text.DrawLine("Version: <rose>" + Engine.GAME_VERSION.CapFirst());
            text.DrawLine("Game seed: " + lm.GameSeed);
            var time = gameTime.TotalGameTime;
            text.DrawLine("Up time: " + time.Hours + ":" + time.Minutes + ":" + time.Seconds);
            text.DrawLine("FPS: " + Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds, 2));
            text.DrawLine("Turns: " + GameClient.Server.AmtTurns);
            if (GameClient.IsDevMode)
            {
                text.DrawLine("Delay: " + GameClient.Server.TickDelay);
                //text.DrawLine("CanInit: " + GameClient.InputHandler.CanInit);
            }
            

            var player = GameClient.Server.User.Player;
            if (player != null)
            {
                text.MoveLine();
                text.DrawLine("<lime>Player:");
                text.DrawLine("Location: " + player.Position);
                //var area = player.Level.IsInMapBounds(player.X, player.Y) ? player.Level.AreaReferences[player.X, player.Y] : -1;
                //text.DrawLine("Area: " + area);
                //text.DrawLine("Movement: " + player.Movement);
                //text.DrawLine("CanInit: " + InputHandler.CanInit);
                //text.DrawLine("Action: " + TurnAction?.GetType().Name);
                text.DrawLine("Attack timer: " + player.AttackTimer);
                var target = player.Target != null ? player.Target.ToString() : "None";
                text.DrawLine("TargetingMode: " + GameClient.InputHandler.TargetingMode);
                text.DrawLine("Target: " + target);
                text.DrawLine("Action: " + player.PrevAction);
                var swap = GameClient.Server.User.Equipment.WeaponSwap.Select((w, i) => w != null ? (w.ToString() + ((w is Items.Weapon wep && i == 1) ? " (offhand)" : "")) : null).Where(s => s != null).Stringify2();// (.Stringify2(i => i == null ? "none" : i.ToString())
                text.DrawLine("Swap: " + (swap == "" ? "none" : swap));
                if (GameClient.IsDevMode)
                {
                    text.DrawLine("Action Potential: " + player.ActionPotential + "/" + player.GetInitiative());

                    if (level.AreaReferences != null && level.IsInMapBounds(player.X, player.Y))
                    {
                        var areaID = level.AreaReferences[player.X, player.Y];
                        var area = level.GetAreaFromID(areaID);
                        if (area != null)
                            text.DrawLine("Area: " + areaID + " (" + area.GetType().Name + ")");
                        else
                            text.DrawLine("Area: none");
                    }
                }
                
            }

            /*
            text.MoveLine();
            text.DrawLine("Mouse position: " + Input.MousePosition);
            */

            var hoverPos = display.ViewHandler.HoverPos;
            if (hoverPos != null)
            {
                text.MoveLine();
                text.DrawLine("<aqua>Mouse hover:");
                text.DrawLine("Location: " + hoverPos.Value.ToString());
                //var area = player.Level.IsInMapBounds(hoverPos.Value.X, hoverPos.Value.Y) ? player.Level.AreaReferences[hoverPos.Value.X, hoverPos.Value.Y] : -1;
                //text.DrawLine("Area: " + area);
                var tile = GameClient.IsLighted
                    ? lm.CurrentLevel.GetTile(hoverPos.Value)
                    : GameClient.Server.User.VisibilityHandler.GetMemoryTile(hoverPos.Value.X, hoverPos.Value.Y);
                text.DrawLine("Ground: " + tile.Ground.ID);
                text.DrawLine("Object: " + tile.Object.ID);
                text.DrawLine("Light level: " + lm.CurrentLevel.GetTileLightness(hoverPos.Value));

                // Show hover instance, otherwise target instance:
                if (GameClient.IsDevMode)
                {
                    Instance inst = null;
                    var hoverInst = display.ViewHandler.HoverInstance;
                    if (hoverInst != null)
                    {
                        inst = hoverInst;
                        text.Draw("<ltgray>Hover");
                    }
                    else if (GameClient.Server.User.Target != null)
                    {
                        inst = GameClient.Server.User.Target;
                        text.Draw("<ltgray>Target");
                    }

                    if (inst != null)
                    {
                        text.DrawLine(" <ltgray>Instance:<r> " + inst.ToString().CapFirst());
                        text.DrawLine(" ActionPotential: " + inst.ActionPotential + "/" + inst.GetInitiative());
                        text.DrawLine(" TimeSinceLastTurn: " + inst.TimeSinceLastTurn);

                        if (inst is NPC npc)
                        {
                            if (inst is Enemy enemy)
                            {
                                text.DrawLine(" Target: " + enemy.Target?.Instance.ToString().CapFirst() ?? "None");
                                text.DrawLine(" Action: " + enemy.PrevAction ?? "None");
                            }
                                
                            //text.DrawLine(" Base pos: " + npc.BasePosition ?? "None");
                            //text.DrawLine(" DeathTimer: " + npc.DeathTimer);
                        }
                    }
                }
            }



            // Right side:
            text = new TextDrawer(region.Right - 4 - 16, 8, 16, Alignment.VCenteredRight, Assets.Medifont, Color.White, true);
            text.ResetColorOnCall = true;

            // Level data
            text.DrawLine("");
            text.DrawLine("<purple>Level:");
            text.DrawLine("Depth: " + level.Depth);
            text.DrawLine("Temp: " + level.Temp);
            text.DrawLine("Seed: " + level.Seed);

            if (GameClient.IsDevMode)
            {
                text.DrawLine("Instances: " + level.ActiveInstances.Count);
                text.DrawLine("Entities: " + level.ActiveInstances.Count(i => i is Entity));
                text.DrawLine("Items: " + level.ActiveInstances.Count(i => i is GroundItem));

                if (level.Areas != null)
                {
                    text.DrawLine("Caves: " + level.Areas.Count(a => a is Generation.Cavern));
                    text.DrawLine("Rooms: " + level.Areas.Count(a => a is Generation.Room));
                    text.DrawLine("Pathways: " + level.Areas.Count(a => a is Generation.Pathway));
                }
                text.DrawLine("Difficulty: " + level.DifficultyPointsUsed + "/" + level.DifficultyPoints);
                text.DrawLine("Benefit rem.: " + lm.BenefitValue);
            }
            Surface.SpriteBatch.End();
            surface.EndRenderTarget();
        }
    }
}
