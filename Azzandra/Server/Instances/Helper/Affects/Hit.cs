using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class Hit
    {
        public readonly Entity Owner;

        public bool IsOnPlayerTick => Owner?.IsOnPlayerTick ?? false;

        public readonly SpriteFont Font = Assets.Medifont;
        public readonly Vector2 MaxOffset = new Vector2(0, - GameClient.GRID_SIZE);
        public Vector2 Offset = Vector2.Zero;
        public Vector2 OffsetStep;
        public readonly int MaxTime = Engine.FPS / 3;
        public int Time;
        
        public Hit(Entity owner)
        {
            Owner = owner;

            OffsetStep = MaxOffset / MaxTime;
            Time = 0;
        }

        public void Update()
        {
            // Offset & timer handling:
            Time++;
            Offset += OffsetStep;
            if (Time >= MaxTime)
                Owner.Hits.Remove(this);
        }


        /// <summary>
        /// Whill draw this hit at the specified instance's position.
        /// </summary>
        /// <param name="pos">The exact absolute center of the instance to draw at.</param>
        /// <param name="server">The server object.</param>
        public void Draw(Vector2 pos, Server server)
        {
            pos += Offset;

            //float turnDelay = IsOnPlayerTick ? server.TurnDelay : server.EnemyTurnDelay;
            //// Break off! Because otherwise it appears at max pos for one frame.
            //if (turnDelay == -1)
            //    return;
            //pos += (float)(Server.TURN_SPEED - turnDelay) / Server.TURN_SPEED * MaxOffset - MaxOffset / 2;

            DrawHit(pos, server);
        }

        protected abstract void DrawHit(Vector2 pos, Server server);
    }

    public class HitDmg : Hit
    {
        public readonly Style Style;
        public readonly int Dmg;

        public HitDmg(Entity owner, Style style, int dmg) : base(owner)
        {
            Style = style;
            Dmg = dmg;
        }

        protected override void DrawHit(Vector2 pos, Server server)
        {
            Vector2 iconOffset = new Vector2(8, 8);

            Display.DrawTexture(pos - iconOffset, Assets.Hit, GetHitColor());
            Display.DrawStringCentered(pos, Dmg.ToString(), Font, true);
        }

        private Color GetHitColor()
        {
            switch (Style)
            {
                default: return Color.Red;
                case Style.Fire: return Color.Orange;
                case Style.Poison: return Color.Green;
                case Style.Ice: return Color.LightBlue;
            }
        }
    }

    public class HitString : Hit
    {
        public readonly string Text;

        public HitString(Entity owner, string text) : base(owner)
        {
            Text = text;
        }

        protected override void DrawHit(Vector2 pos, Server server)
        {
            Display.DrawStringCentered(pos, Text, Font, true);
        }
    }
}
