using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TileDisplayManager
    {
        public Tile Tile { get; set; }
        public TileDisplay TileDisplayGround { get; set; }
        public TileDisplay TileDisplayObject { get; set; }
        public AnimationManager AnimationManager { get; set; }

        public TileDisplayManager(Tile tile)
        {
            Tile = tile;
            TileDisplayGround = TileDisplay.Get(tile.Ground.ID);
            TileDisplayObject = TileDisplay.Get(tile.Object.ID);

            var objTexture = TileDisplayObject?.Texture;
            var grndTexture = TileDisplayGround?.Texture;
            var tex = objTexture ?? grndTexture;

            AnimationManager = new AnimationManager(tex);
            AnimationManager.AmtOfLoops = 0;
            AnimationManager.RenderFire = () => TileDisplayGround.RenderFire || TileDisplayObject.RenderFire;

            
        }

        public void Play()
        {
            // For lava/sulphur: 
            if (Tile.Ground.ID == BlockID.Lava || Tile.Ground.ID == BlockID.Sulphur)
            {
                AnimationManager.AmtOfLoops = -1;
                AnimationManager.FrameStage = Util.Random.Next(AnimationManager.Animation.AmtOfFrames);
            }
        }
        public void Stop()
        {
            AnimationManager.Stop();
        }

        public void Update()
        {
            if (Tile.Ground.ID == BlockID.Water && Util.Random.NextDouble() < 0.002d)
            {
                AnimationManager.AmtOfLoops = 1;
            }

            AnimationManager.Update();
        }


        /// <summary>
        /// Draw the the animation/sprite at the desired location (centered)
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pos"></param>
        public void Draw(SpriteBatch sb, ViewHandler vh, Vector2 pos, Color? colorEffect = null, float lightness = 1f)
        {
            // Draw background
            var bg = TileDisplayGround.BGColor ?? vh.BGColor;
            bg = bg.ChangeBrightness(-1 + lightness);
            DrawTileTexture(sb, pos, Assets.TileBasic, bg);

            // Draw tile marker if Debugging:
            if (vh.GameClient.IsDebug && Tile.Marker > 0)
            {
                int i = Math.Max(0, Math.Min(Tile.Marker - 1, vh.MarkerColors.Length - 1));
                DrawTileTexture(sb, pos, Assets.TileBasic, vh.MarkerColors[i] * 0.5f);
            }

            // Draw tile texture
            AnimationManager.Draw(sb, pos, null, lightness);
        }

        private void DrawTileTexture(SpriteBatch sb, Vector2 pos, Texture2D tex, Color col, int deg = 0)
        {
            if (tex == null)
                return;

            var tileRad = new Vector2(16 / 2);
            float scale = ViewHandler.GRID_SIZE / 16f;
            sb.Draw(tex, pos, tex.Bounds, col, MathHelper.ToRadians(deg), tileRad, scale, SpriteEffects.None, 0f);
        }
    }
}
