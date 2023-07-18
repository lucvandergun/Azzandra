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
    public class SpellsInterface : TabInterface
    {
        protected override string Title => "= Spellbook =";
        public List<LearnedSpell> Spells => GameClient.Server?.User.LearnedSpells ?? null;
        private User User => GameClient.Server?.User;
        private SpellData HoverSlot;
        public SpellData SelectedSlot
        {
            get
            {
                var targetingMode = GameClient.InputHandler.TargetingMode;
                ISpellEffect effect = targetingMode.InboundAction is ActionAffect a && a.Affect is Spell s ? (ISpellEffect)s.SpellEffect
                    : targetingMode.InboundAction is ActionVectorSpell v ? (ISpellEffect)v.SpellEffect : null;
                if (effect == null) return null;
                return Spells.Select(id => id.SpellData).FirstOrDefault(d => d.GetSpellEffect()?.GetType() == effect.GetType());
            }
        }

        private ButtonDark CastButton;

        public SpellsInterface(GameClient gameClient) : base(gameClient)
        {
            //CastButton = new ButtonDark(new Vector2(84, 24), "Cast")
            //{
            //    //OnClick = () => CastSpell()
            //};
        }

        public void CastSpell(SpellData spellData)
        {
            var selected = SelectedSlot;

            // If no spell selected: return
            if (spellData == null)
            {
                GameClient.Server?.User.ThrowError("The spell to cast is null.");
                return;
            }

            var effect = spellData.GetSpellEffect();
            if (effect == null)
            {
                GameClient.Server?.User.ThrowError("The spell id \'" + spellData.ID + "\' is not known!");
                return;
            }

            // If current spell does not need any target
            if (effect is SpellEffectAcute a)
            {
                GameClient.Server.SetPlayerAction(new ActionSpellAcute(User.Player, a, User.GetSpellLevel(spellData.ID)));
                DeselectSpell();
            }

            // If current spell was already being targeted and there is a target
            var targetingMode = GameClient.InputHandler.TargetingMode;
            if (spellData == selected && targetingMode.HasTarget(GameClient.Server))
            {
                targetingMode.PerformTargetAction(GameClient.InputHandler);
                return;
            }

            // Setup a new targeting mode:
            if (effect is SpellEffect se)
            {
                var it = new TargetingMode.InstanceTargeting(true);
                var spell = new Spell(GameClient.Server, 2, 6, User.Player.GetSpc().Round() + User.GetSpellCastBoost(spellData.ID), se, User.GetSpellLevel(spellData.ID));
                it.InboundAction = new ActionAffect(User.Player, null, spell);
                GameClient.InputHandler.TargetingMode = it;

            }
            if (effect is SpellEffectVector sev)
            {
                var tt = new TargetingMode.TileTargeting();
                tt.InboundAction = new ActionVectorSpell(User.Player, Vector.Zero, sev, User.GetSpellLevel(spellData.ID));
                GameClient.InputHandler.TargetingMode = tt;
            }
        }

        //public override void OnResize(Point region)
        //{
        //    base.OnResize(region);
        //    SubArea.SetSize(SubArea.Region.X, SubArea.Region.Y, SubArea.Region.Width, SubArea.Region.Height - 32);
        //}

        protected override void DrawAdditional(Surface surface)
        {
            // Draw tooltip
            DrawToolTip(surface, HoverSlot, HoverSlot == SelectedSlot);

            //CastButton.Render(surface, new Vector2(surface.Width - 64, surface.Height - 16), GameClient.Engine.GraphicsDevice, GameClient.Engine.SpriteBatch, true);
        }

        protected override void RenderSubArea(Rectangle outerRegion, bool isHoverSurface)
        {
            var surface = SubArea;
            var absoluteSurfacePos = new Vector2(outerRegion.X, outerRegion.Y) + surface.Position;

            GameClient.Engine.GraphicsDevice.SetRenderTarget(surface.Display);
            GameClient.Engine.GraphicsDevice.Clear(Color.Black);
            GameClient.Engine.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            if (Spells == null)
                return;

            HoverSlot = null;
            var selected = SelectedSlot;

            // Render item slots
            int slotHeight = 20;
            var startOffset = new Vector2(0, 0);
            var slotSize = new Vector2(surface.Width, slotHeight);
            var slotOffset = new Vector2(0, slotHeight);
            var stringOffset = new Vector2(4, slotSize.Y / 2);

            var format = new TextFormat(Color.White, Font, Alignment.VCentered, false);
            var text = new TextDrawer(startOffset, 16, format);

            // Update area scroll offset:
            SubArea.FullRenderSize = new Vector2(surface.Width, slotHeight * Spells.Count);
            if (isHoverSurface) SubArea.UpdateScrollPosition();
            startOffset -= SubArea.RenderOffset;


            // Render spell slots
            for (int i = 0; i < Spells.Count; i++)
            {
                // Get spell data
                var learnedSpell = Spells[i];
                var spell = learnedSpell.SpellData;
                if (spell == null) continue;

                // Draw position:
                Vector2 pos = startOffset + slotOffset * i;

                // Keyboard Handling:
                if (spell != null && GameClient.KeyboardFocus == GameClient.Focus.General && Input.IsKeyPressed[Input.GetKeyBind(i)] && GameClient.DisplayHandler.MouseInterface == null)
                {
                    // Perform default option directly with SHIFT:
                    if (Input.IsKeyDown[Keys.LeftShift] || Input.IsKeyDown[Keys.RightShift])
                    {
                        ActivateSpell(spell);
                    }
                    // Open up item menu otherwise:
                    else
                    {
                        new SpellMenu(GameClient, learnedSpell, absoluteSurfacePos + pos + slotOffset, Display.MakeRectangle(absoluteSurfacePos + pos, slotSize), outerRegion, false); 
                    }
                }

                // Determine whether is hovered slot
                bool hover = isHoverSurface && Input.MouseHover(absoluteSurfacePos + pos, slotSize);

                if (hover)
                {
                    HoverSlot = spell;

                    if (Input.IsMouseLeftPressed)
                    {
                        ActivateSpell(spell);
                    }
                    else if (Input.IsMouseRightPressed)
                    {
                        new SpellMenu(GameClient, learnedSpell, Input.MousePosition, Display.MakeRectangle(absoluteSurfacePos + pos, slotSize), outerRegion);
                    }
                }

                // Draw slot text
                text.SetPosition(pos + stringOffset);
                text.Draw(Input.GetKeyBindString(i) + ". ", Color.LightSlateGray);
                text.Draw(spell.Name.CapFirst(), spell.GetStringColor());
                text.DrawLine(" <r>- SP: " + spell.SpellPoints + "");
                //text.DrawLine("<slate>" + spell.Desc);

                // Draw hover slot bounds
                if (spell == HoverSlot || spell == selected)
                {
                    var color = spell == selected ? Color.White  : new Color(191, 191, 191);
                    Display.DrawInline(Display.MakeRectangle(pos, slotSize), color);
                }
            }

            GameClient.Engine.SpriteBatch.End();
            GameClient.Engine.GraphicsDevice.SetRenderTarget(null);
        }

        public void ActivateSpell(SpellData spell)
        {
            // Cast the current spell if sufficient spellpoints:
            if (User.Player.Sp < spell.SpellPoints && !User.IsCheatMode)
            {
                User.Player.User.ShowMessage("<rose>You don't have enough spell points to cast that spell!");
                DeselectSpell();
            }
            else
            {
                CastSpell(spell);
            }
        }


        public void DeselectSpell()
        {
            // Reset targeting mode to default
            GameClient.InputHandler.TargetingMode = GameClient.InputHandler.DefaultTargetingMode;
        }


        /// <summary>
        /// Creates a tooltip string based on current hover slot
        /// </summary>
        /// <param name="pos"></param>
        private void DrawToolTip(Surface surface, SpellData spell, bool isSelected)
        {
            //Return if no hover slot
            if (spell == null) return;

            //Get item action & name
            var castOnTarget = isSelected && GameClient.InputHandler.TargetingMode.HasTarget(GameClient.Server);
            string action = castOnTarget ? "cast on target" : "cast";
            string name = spell.Name;

            //Capitalize
            if (action == null) name = name.CapFirst();
            else action = action.CapFirst();

            //Start drawing
            int titleLength = Util.GetStringWidth(Title, TitleFont);
            var pos = TitleOffset + new Vector2(titleLength + 16, -1);

            var textDrawer = new TextDrawer(pos, 16, Alignment.VCentered, Font, Color.White);

            if (action != null)
                textDrawer.Draw("<aqua>" + action + " <r>");

            textDrawer.Draw(name);
        }
    }
}
