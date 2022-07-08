using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Azzandra
{
    static class Input
    {
        public static bool IsCapsLock;

        public static Dictionary<Keys, bool> IsKeyDown { get; private set; } = new Dictionary<Keys, bool>();
        public static Dictionary<Keys, bool> IsKeyPressed { get; private set; } = new Dictionary<Keys, bool>();

        public static bool IsMouseLeftDown { get; private set; }
        public static bool IsMouseRightDown { get; private set; }
        public static bool IsMouseLeftPressed { get; private set; }
        public static bool IsMouseRightPressed { get; private set; }
        public static bool IsMouseLeftReleased { get; private set; }
        public static bool IsMouseRightReleased { get; private set; }

        public static int ScrollDirection { get; private set; }
        private static int CurrentScrollValue = 0;


        public static KeyboardState KeyboardState { get; private set; }
        public static KeyboardState KeyboardStatePrev { get; private set; }
        public static MouseState MouseState { get; private set; }
        public static MouseState MouseStatePrev { get; private set; }


        public static Keys KeyLeft { get; private set; } = Keys.A;
        public static Keys KeyRight { get; private set; } = Keys.D;
        public static Keys KeyUp { get; private set; } = Keys.W;
        public static Keys KeyDown { get; private set; } = Keys.S;

        public static void Update()
        {
            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();

            //keyboard keys
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                IsKeyDown[key] = KeyboardState.IsKeyDown(key);
                IsKeyPressed[key] = KeyboardState.IsKeyDown(key) && !KeyboardStatePrev.IsKeyDown(key);
            }

            //mouse buttons
            IsMouseLeftDown = MouseState.LeftButton == ButtonState.Pressed;
            IsMouseRightDown = MouseState.RightButton == ButtonState.Pressed;
            IsMouseLeftPressed = MouseState.LeftButton == ButtonState.Pressed && MouseStatePrev.LeftButton != ButtonState.Pressed;
            IsMouseRightPressed = MouseState.RightButton == ButtonState.Pressed && MouseStatePrev.RightButton != ButtonState.Pressed;
            IsMouseLeftReleased = MouseState.LeftButton != ButtonState.Pressed && MouseStatePrev.LeftButton == ButtonState.Pressed;
            IsMouseRightReleased = MouseState.RightButton != ButtonState.Pressed && MouseStatePrev.RightButton == ButtonState.Pressed;

            //mouse scrolling
            ScrollDirection = Math.Sign(MouseState.ScrollWheelValue - CurrentScrollValue);
            CurrentScrollValue = MouseState.ScrollWheelValue;

            //set previous states
            KeyboardStatePrev = KeyboardState;
            MouseStatePrev = MouseState;
            IsCapsLock = IsKeyDown[Keys.CapsLock];
        }

        public static Vector2 MousePosition => new Vector2(MouseState.X, MouseState.Y) / Engine.Scale;

        public static bool MouseHover(int x, int y, int w, int h)
        {
            int mouseX = MouseState.X / Engine.Scale;
            int mouseY = MouseState.Y / Engine.Scale;

            if (mouseX >= x && mouseY >= y && mouseX < x + w && mouseY < y + h)
            {
                return true;
            }
            else return false;
        }
        public static bool MouseHover(Rectangle rect)
        {
            int mouseX = MouseState.X / Engine.Scale;
            int mouseY = MouseState.Y / Engine.Scale;

            if (mouseX >= rect.Left && mouseY >= rect.Top && mouseX < rect.Right && mouseY < rect.Bottom)
            {
                return true;
            }
            else return false;
        }

        public static bool MouseHover(Vector2 start, Vector2 size)
        {
            int mouseX = MouseState.X / Engine.Scale;
            int mouseY = MouseState.Y / Engine.Scale;

            return mouseX > start.X && mouseY > start.Y && mouseX < start.X + size.X && mouseY < start.Y + size.Y;
        }
    }
}
