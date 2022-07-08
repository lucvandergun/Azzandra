 using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class InputHandler
    {
        public GameClient GameClient { get; private set; }
        public Server Server => GameClient.Server;
        private readonly SpriteFont Font = Assets.Medifont, TitleFont = Assets.Gridfont;

        public Dir Dir { get; private set; }
        public bool CanInit { get; private set; } = false;
        public bool JustPressed { get; private set; } = false;

        protected bool IsShiftButton = false;
        public bool IsShift => IsShiftButton || Input.IsKeyDown[Keys.LeftShift];

        private readonly Vector2 ButtonSize = new Vector2(96, 16);
        private readonly Button ShiftButton, TargetButton, ActionButton, RestButton, SwapButton;
        private readonly Button[] Buttons;

        // === Target Handling === \\
        public Instance InstanceTarget => GameClient.Server.User.Target;
        public TargetingMode.TargetingMode TargetingMode { get; set; }
        public readonly TargetingMode.TargetingMode DefaultTargetingMode = new TargetingMode.EntityTargeting();


        public InputHandler(GameClient gameClient)
        {
            GameClient = gameClient;
            TargetingMode = DefaultTargetingMode;

            ShiftButton = new Button(ButtonSize, "Shift", ButtonFormat.Dark)
            {
                OnClick = () => {
                    IsShiftButton = !IsShiftButton;
                },
                IsSelected = () => IsShiftButton
            };
            TargetButton = new Button(ButtonSize, "Target", ButtonFormat.Dark)
            {
                OnClick = () => {
                    if (TargetingMode is TargetingMode.InstanceTargeting t)
                        t.SwitchTarget(this);
                },
            };
            ActionButton = new Button(ButtonSize, "Action", ButtonFormat.Dark)
            {
                OnClick = () => {
                    TargetingMode?.PerformTargetAction(this);
                },
                Text = () =>
                {
                    return TargetingMode.GetActionString(GameClient.Server).CapFirst();
                }
            };
            RestButton = new Button(ButtonSize, "Rest", ButtonFormat.Dark)
            {
                OnClick = () => {
                    if (Server != null) SetRestAction();
                },
            };
            SwapButton = new Button(ButtonSize, "Quick-swap", ButtonFormat.Dark)
            {
                OnClick = () => {
                    if (Server != null) Server.User.Player.Action = new ActionSwap(Server?.User.Player, Server?.User.Equipment.WeaponSwap.CreateCopy());
                },
            };

            Buttons = new Button[] { RestButton, SwapButton, ShiftButton, TargetButton, ActionButton };
        }

        public void Update()
        {
            if (GameClient.Server == null)
                return;

            if (TargetingMode != null)
            {
                TargetingMode.CheckPerformAction(this);
                TargetingMode.CheckSwitchTarget(this);
            }

            GetInput();

            if (Server != null)
            {
                // Rest action
                if (Input.IsKeyPressed[Keys.R])
                    SetRestAction();

                // Swap last-equipped
                if (Input.IsKeyPressed[Keys.Q])
                    Server.User.Player.Action = new ActionSwap(Server?.User.Player, Server?.User.Equipment.WeaponSwap.CreateCopy());
            }
        }

        private void SetRestAction()
        {
            var player = GameClient.Server.User.Player;
            if (player.Hunger >= player.GetFullHunger())
            {
                GameClient.Server.User.ShowMessage("<rose>You can't rest when you're starving!");
            }
            else
            {
                player.Action = new ActionRest(GameClient.Server.User.Player);
            }
        }


        private void GetInput()
        {
            CanInit = Dir == null;

            Dir = new Dir(
                Input.IsKeyDown[Keys.A] && !Input.IsKeyDown[Keys.D] ? -1 : Input.IsKeyDown[Keys.D] && !Input.IsKeyDown[Keys.A] ? 1 : 0,
                Input.IsKeyDown[Keys.W] && !Input.IsKeyDown[Keys.S] ? -1 : Input.IsKeyDown[Keys.S] && !Input.IsKeyDown[Keys.W] ? 1 : 0);

            var numDir = new Dir(0, 0);
            if (Input.IsKeyDown[Keys.NumPad4]) numDir -= new Dir(-1, 0);
            if (Input.IsKeyDown[Keys.NumPad6]) numDir -= new Dir(1, 0);
            if (Input.IsKeyDown[Keys.NumPad8]) numDir -= new Dir(0, -1);
            if (Input.IsKeyDown[Keys.NumPad2]) numDir -= new Dir(0, 1);
            if (Input.IsKeyDown[Keys.NumPad7]) numDir -= new Dir(-1, -1);
            if (Input.IsKeyDown[Keys.NumPad1]) numDir -= new Dir(-1, 1);
            if (Input.IsKeyDown[Keys.NumPad9]) numDir -= new Dir(1, -1);
            if (Input.IsKeyDown[Keys.NumPad3]) numDir -= new Dir(1, 1);

            Dir -= numDir;

            if (Dir.IsNull() && !(Input.IsKeyDown[Keys.Z] || Input.IsKeyDown[Keys.NumPad5]))
                Dir = null;

            // Determine whether just pressed:
            JustPressed = Input.IsKeyPressed[Keys.A] || Input.IsKeyPressed[Keys.D] || Input.IsKeyPressed[Keys.W] || Input.IsKeyPressed[Keys.S];
        }



        public void Render(Surface surface, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            graphicsDevice.SetRenderTarget(surface.Display);
            graphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            var buttonSize = new Vector2(32);
            var pos = new Vector2(surface.Width / 3, surface.Height / 2) - buttonSize / 2;
            Dir buttonDir;
            bool canActivate = GameClient.DisplayHandler.Interface == null;

            for (int i = 0; i < 9; i++)
            {
                char c = ' ';

                //start dir = up, rotate anti-clockwise
                switch (i)
                {
                    default:
                    case 0: buttonDir = new Dir(0, -1); c = 'U';  break;
                    case 1: buttonDir = new Dir(-1, -1); break;
                    case 2: buttonDir = new Dir(-1, 0); c = 'L'; break;
                    case 3: buttonDir = new Dir(-1, 1); break;
                    case 4: buttonDir = new Dir(0, 1); c = 'D'; break;
                    case 5: buttonDir = new Dir(1, 1); break;
                    case 6: buttonDir = new Dir(1, 0); c = 'R'; break;
                    case 7: buttonDir = new Dir(1, -1); break;
                    case 8: buttonDir = new Dir(0, 0); c = '.'; break;
                }

                var buttonPos = pos + buttonDir.ToFloat() * buttonSize;

                var hover = (GameClient.DisplayHandler.IsHoverSurface(surface) && Input.MouseHover(surface.Position + buttonPos, buttonSize));
                var color = hover && Input.IsMouseLeftDown && canActivate ? Color.White : new Color(127, 127, 127);

                if (canActivate && hover && (Input.IsMouseLeftDown && i != 8 || Input.IsMouseLeftPressed))
                {
                    Dir = Dir != null ? Dir + buttonDir : buttonDir;
                }

                var rect = new Rectangle(buttonPos.ToPoint(), buttonSize.ToPoint());
                Display.DrawInline(rect, color, 1);

                if (c != ' ')
                {
                    Display.DrawStringCentered(buttonPos + buttonSize / 2, c.ToString(), TitleFont);
                }

                //not activatable overlay
                if (!canActivate)
                    Display.DrawRect(rect, Color.Black * 0.5f);

            }

            var bOffset = new Vector2(0, 20);
            var bPos = new Vector2(surface.Width / 3 * 2, surface.Height / 2) - (Buttons.Length - 1) * bOffset / 2;
            foreach (var button in Buttons)
            {
                if (button == ActionButton && (!TargetingMode?.HasTarget(Server) ?? false)
                    || button == TargetButton && !(TargetingMode is TargetingMode.InstanceTargeting) )
                {

                }
                else
                {
                    button.Render(surface, bPos, graphicsDevice, spriteBatch, true);
                }  
                bPos += bOffset;
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }
    }
}
