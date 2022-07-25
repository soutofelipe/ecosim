using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Drawing;
using GLFW;
using static ecosim_dotnetcore.OpenGL.GL;

namespace ecosim_dotnetcore.Rendering.Display
{
    class DisplayManager
    {
        public static Window window { get; set; }
        public static Vector2  windowSize { get; set; }

        public static void CreatWindow(int width, int height, string title)
        {
            windowSize = new Vector2(width, height);

            Glfw.Init();
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);

            Glfw.WindowHint(Hint.Focused, true);
            Glfw.WindowHint(Hint.Resizable, false);

            window = Glfw.CreateWindow(width, height, title, Monitor.None, Window.None);

            if (window == Window.None)
                return;

            Rectangle screen = Glfw.PrimaryMonitor.WorkArea;
            int x = (screen.Width - width) / 2;
            int y = (screen.Height - height) / 2;

            Glfw.SetWindowPosition(window, x, y);


            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);

            glViewport(0, 0, width, height);
        }
        public static void CloseWindow()
        {
            Glfw.Terminate();
        }
    }
}
