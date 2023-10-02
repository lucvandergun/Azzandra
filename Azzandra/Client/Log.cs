using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Azzandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Azzandra
{
    public class Message
    {
        public int Turn, Count;
        public readonly string Text;
        public readonly bool Filter;

        public Message(int turn, string text, bool filter = false)
        {
            Turn = turn;
            Text = text;
            Filter = filter;
            Count = 1;
        }

        public override string ToString()
        {
            return Text + (Count > 1 ? " (" + Count + ")" : "");
        }
    }
    
    public class Log
    {
        private readonly GameClient GameClient;
        
        private readonly List<Message> ListOfMessages;

        private const int MAX_LINES = 50;
        private int ScrollPos = 0;
        private bool IsSnapped => ScrollPos == MaxScrollPos(ListOfMessages?.Count ?? 0);
        private readonly int LineH = 16;
        private int RegionHeight => GameClient.DisplayHandler?.LogSurface.Height ?? 100;
        private int RegionWidth => GameClient.DisplayHandler?.LogSurface.Width - 100 ?? 450;
        private int MinScrollPos => 0;
        private int MaxScrollPos(int msgCount) => msgCount * LineH - RegionHeight;


        private bool IsFilter = false;

        public Log(GameClient gameClient)
        {
            GameClient = gameClient;
            ListOfMessages = new List<Message>();
        }

        public void Add(string msg, bool filter = false)
        {
            var turn = GameClient.Server?.AmtTurns ?? 0;
            bool isSnapped = IsSnapped;

            // If same as previous, increase previous count by 1:
            if (ListOfMessages.Count > 0)
            {
                var last = ListOfMessages.Last();
                if (last.Text == msg)
                {
                    last.Count++;
                    return;
                }
            }
            

            // Split string if drawn string length exceeds surface width
            int maxLength = RegionWidth;
            var lines = Util.SeparateString(msg, Assets.Medifont, maxLength);
            string lastFormat = null;

            // Add string segments to the list of messages
            foreach (var line in lines)
            {
                // Automatically scroll if "snapped" to last message
                if (ListOfMessages.Count < MAX_LINES && IsSnapped)
                {
                    ScrollPos += LineH;
                }
                //// Adjust scroll position to view the same lines when message count exceeds max lines
                //else if (!IsSnapped && ListOfMessages.Count >= MAX_LINES)
                //{
                //    ScrollPos = Math.Max(0, ScrollPos - LineH);
                //}

                // Paste last formatting code to the front of a non-starting line.
                var newLine = line;
                if (lastFormat != null)
                    newLine = lastFormat + newLine;

                if (lines.Length > 1)
                    lastFormat = TextFormatter.FormatString(newLine).FirstOrDefault(f => f.First() == '<');

                ListOfMessages.Add(new Message(turn + 1, newLine, filter));
            }

            while (ListOfMessages.Count > MAX_LINES)
            {
                ListOfMessages.RemoveAt(0);
            }

            // Snap to last msg if snapped before:
            if (isSnapped)
                ScrollPos = MaxScrollPos(ListOfMessages.Count);
        }

        public void Clear()
        {
            ListOfMessages.Clear();
        }

        public void Render(Surface surface)
        {
            var region = surface.Region;
            var font = Assets.Medifont;

            // Toggle filter
            if (Input.IsKeyPressed[Keys.F])
            {
                //IsFilter = !IsFilter;
            }

            // Select message list based on current criteria (Filtering, Showing debug msgs, etc)
            var messages = CompileShownMessages();

            // Handling scroll input
            if (GameClient.DisplayHandler.IsHoverSurface(surface) && Input.ScrollDirection != 0)
            {
                ScrollPos = Math.Max(MinScrollPos, Math.Min(MaxScrollPos(messages.Count), ScrollPos - Input.ScrollDirection * 20));
            }
                

            // Draw log section
            int msgStart = ScrollPos / LineH;
            int visibleCount = (region.Height / LineH) + 1;
            var text = new TextDrawer(4, msgStart * LineH - ScrollPos + 8, LineH, Alignment.VCentered, font, Color.White);
            text.ResetColorOnCall = true;

            var currentTurn = GameClient.Server?.AmtTurns ?? 0;

            var messageCount = messages.Count;
            for (int i = msgStart; i < messageCount && i < msgStart + visibleCount; i++)
            {
                var msg = messages[i];

                // Determine whether to display msg:
                if (IsFilter && msg.Filter)
                    continue;

                // Calculate message color blend intensity
                Color? color = null;// CalculateColor(msg.Turn, currentTurn);
                text.DrawLine(msg.ToString(), color);
            }

            // Draw scroll bar
            if (messageCount > visibleCount - 1)
            {
                int maxHeight = RegionHeight - 20;
                int fullLogHeight = messageCount * LineH;

                float displayFactor = (float)region.Height / fullLogHeight;
                int barHeight = Math.Min(maxHeight, (int)(displayFactor * maxHeight));

                float startFactor = (float)ScrollPos / fullLogHeight;
                int barY = Math.Min(maxHeight - barHeight, (int)Math.Ceiling(startFactor * maxHeight));

                var totalSize = new Vector2(1, maxHeight);
                var barSize = new Vector2(1, barHeight);
                var startPos = new Vector2(region.Width - 4 - barSize.X, (region.Height - maxHeight) / 2);
                var offset = new Vector2(0, barY);

                Display.DrawRect(startPos, totalSize, DisplayHandler.LineColor);
                Display.DrawRect(startPos + offset, barSize, new Color(191, 191, 191));
            }
            


            // Debug text
            if (GameClient.IsDevMode && GameClient.IsDebug)
            {
                var py = 0;
                var debug = new string[] {
                    "Count: " + messageCount,
                    "Shown: " + (msgStart) + " - " + Math.Min(msgStart + visibleCount - 1, messageCount - 1),
                    "Snap: " + IsSnapped,
                    "Pos: " + ScrollPos,
                    "Scroll: " + Input.ScrollDirection,
                    "Filtering: " + IsFilter
                };

                foreach (var msg in debug)
                {
                    var blend = py * 1f / debug.Count();
                    var color = (Color.White).BlendWith(Color.Aqua, blend);

                    Display.DrawString(region.Right - 120, py * LineH, msg, font, color);
                    py++;
                }
            }
        }

        private Color? CalculateColor(int messageTurn, int currentTurn)
        {
            if (messageTurn >= currentTurn)
                return null;
            else return messageTurn == currentTurn - 1 ? new Color(191, 191, 191) : Color.Gray;
        }

        private List<Message> CompileShownMessages()
        {
            var list = ListOfMessages.CreateCopy();

            // Filtering
            if (IsFilter)
                list = list.Where(m => !m.Filter).ToList();

            // Debug

            return list;
        }
    }
}
