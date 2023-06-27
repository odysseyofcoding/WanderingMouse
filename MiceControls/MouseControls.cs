/* Thanks and credits to the whole community. As I started this, I was a noob and brute forced it with the help from community posts, different sources */
using System.Drawing;
using System.Runtime.InteropServices;

namespace MiceControls
{
    public static class MouseControls
    {
        [DllImport("User32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        //public static extern bool mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_MOVE = 0x0001;
        public static void MoveTo(float x, float y)
        {
            int min = 0;
            int max = 65535;

            int mappedX = (int)Remap(x, -1.0f, 1920, min, max);
            int mappedY = (int)Remap(y, -1.0f, 1080, min, max);

            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, mappedX, mappedY, 0, 0);
        }

        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        [Flags]
        public enum MouseFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x000000004,
            MIDDLEDOWN = 0x000000020,
            MIDDLEUP = 0x000000040,
            MOVE = 0x000000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x000000010,
        }
        public static void LeftDown(bool press)
        {
            if (press)
            {
                mouse_event((int)(MouseFlags.LEFTDOWN), 0, 0, 0, 0);
            }
            else
            {
                mouse_event((int)(MouseFlags.LEFTUP), 0, 0, 0, 0);
            }
        }
        public static void LeftDownAndMove(int posX, int posY)
        {
            MoveTo(posX, posY);
            //mouse_event((int)(MouseFlags.LEFTDOWN), 0, 0, 0, 0);
            //mouse_event((int)(MouseFlags.RIGHTUP), 0, 0, 0, 0);
        }
        public static void RightClick(int posX, int posY)
        {
            MoveTo(posX, posY);
            Thread.Sleep(500);
            mouse_event((int)(MouseFlags.RIGHTDOWN), 0, 0, 0, 0);
            mouse_event((int)(MouseFlags.RIGHTUP), 0, 0, 0, 0);
        }
        public static void LeftDownFromXyToXy(int fromX, int fromY, int toX, int toY)
        {
            MoveTo(fromX, fromY);
            mouse_event((int)(MouseFlags.LEFTDOWN), 0, 0, 0, 0);
            MoveTo(toX, toY);
            mouse_event((int)(MouseFlags.LEFTUP), 0, 0, 0, 0);
        }
        public static void LeftClickOnLocation(int posX, int posY)
        {
            MoveTo(posX, posY);
            //Thread.Sleep(200);
            mouse_event((int)(MouseFlags.LEFTDOWN), 0, 0, 0, 0);
            Thread.Sleep(1);
            mouse_event((int)(MouseFlags.LEFTUP), 0, 0, 0, 0);
            //Thread.Sleep(10);
        }
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            GetCursorPos(out POINT lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)    
            return lpPoint;
        }
    }
}