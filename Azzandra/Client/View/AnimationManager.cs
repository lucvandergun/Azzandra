using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class AnimationManager
    {
        public Animation Animation { get; private set; }
        
        public float FrameStage = 0f;
        public int GetFrameIndex() => (int)FrameStage;
        public int AmtOfLoops { get; set; }

        public Func<bool> RenderLightness = () => true;
        public Func<bool> RenderFire = () => false;
        public Func<bool> RenderFrozen = () => false;
        public Func<float> Angle = () => 0f;

        public AnimationManager(string anim)
        {
            Play(anim);
        }
        public AnimationManager(Texture2D tex)
        {
            if (tex != null)
                Play(new Animation(tex, 0.125f));
        }
        public AnimationManager(Animation anim)
        {
            Play(anim);
        }


        /// <summary>
        /// Play the requested animation
        /// </summary>
        /// <param name="anim"></param>
        public void Play(string anim, int amtOfLoops = -1)
        {
            if (anim == null)
            {
                RemoveAnimation();
                return;
            }

            var speed = anim == "cloud" ? 0.5f : 0.125f;
            Play(new Animation(Assets.GetSprite(anim), speed), amtOfLoops); //Assets.GetAnimation(anim)
        }

        /// <summary>
        /// Play the requested animation
        /// </summary>
        /// <param name="anim"></param>
        public void Play(Animation anim, int amtOfLoops = -1)
        {
            if (anim == null)
            {
                RemoveAnimation();
                return;
            }

            Animation = anim;
            FrameStage = 0f;
            AmtOfLoops = amtOfLoops;
        }

        public void Stop()
        {
            FrameStage = 0f;
            AmtOfLoops = 0;
        }

        public void RemoveAnimation()
        {
            Animation = null;
            FrameStage = 0f;
            AmtOfLoops = 0;
        }


        /// <summary>
        /// Update the current frame of the animation, if the timer is ready
        /// </summary>
        public virtual void Update()
        {
            if (Animation?.Texture.Name == "water")
            {
                if (AmtOfLoops > 0)
                {

                }
                if (FrameStage > 0.125f)
                {

                }
            }
            
            if (Animation == null || Animation.AmtOfFrames <= 1 || AmtOfLoops == 0)
                return;

            // Update the current animation index
            int currentStage = GetFrameIndex();
            FrameStage = (FrameStage + Animation.Speed) % Animation.AmtOfFrames;

            // Stop looping if out of loops:
            if (GetFrameIndex() < currentStage)
            {
                if (AmtOfLoops > 0)
                    AmtOfLoops--;
                if (AmtOfLoops == 0)
                    Stop();
            }
        }


        /// <summary>
        /// Draw the the animation/sprite at the desired location (centered)
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pos"></param>
        public void Draw(SpriteBatch sb, Vector2 pos, Color? colorEffect = null, float lightness = 1f)
        {
            var color = colorEffect ?? Color.White;

            if (RenderFire.Invoke()) color = ViewHandler.FireColor;
            else if (RenderFrozen.Invoke()) color = Color.LightBlue;
            if (RenderLightness.Invoke()) color = color.ChangeBrightness(-1f + lightness);

            Animation?.Draw(sb, pos, GetFrameIndex(), color, Angle.Invoke());
        }
    }
}
