using ManagedWinapi.Windows;
using System;
using System.Collections.Generic;
using System.Drawing; // Point
using System.Text;
using System.Windows.Forms; // Screen
using Vocola;

namespace Library
{

    /// <summary>Functions to manipulate the foreground window.</summary>
    public class Window : VocolaExtension
    {

/*
        // ---------------------------------------------------------------------
        // MoveTo

        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveTo(int dx, int dy)
        {
            MoveTo(dx, dy, "Window");
        }

        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveTo(int dx, int dy, string rectangleName)
        {
            Point p = Pointer.GetOffsetCornerPoint(rectangleName, dx, dy);
            Win.SetForegroundWindowPosition(p.X, p.Y);
        }
*/

        // ---------------------------------------------------------------------
        // MoveToNextScreen

        /// <summary>Moves the foreground window to another display screen.</summary>
        /// <remarks>If the computer has more than one display screen, moves the foreground window to the next screen in
        /// numerical order.</remarks>
        /// <example><code title="Move window to other screen">
        /// Other Screen = Window.MoveToNextScreen();</code>
        /// On a computer with two display screens, saying "Other Screen" moves the foreground window to the other screen.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveToNextScreen()
        {
            Win.MoveForegroundWindowToNextScreen();
        }

        // ---------------------------------------------------------------------
        // MoveToScreenEdge

        /// <summary>Moves the foreground window to an edge of the screen's work area.</summary>
        /// <param name="edge">Named edge of screen's inner rectangle: <c>Top</c>, <c>Right</c>, <c>Bottom</c>, <c>Left</c>, 
        /// <c>Top Right</c>, <c>Bottom Right</c>, <c>Bottom Left</c>, or <c>Top Left</c>. Case insensitive.</param>
        /// <remarks>The screen's inner rectangle (or "work area") is the part not occupied by the taskbar.</remarks>
        /// <example><code title="Move foreground window to a screen edge">
        /// Slam (Top | Bottom | Left | Right | Top Left | Bottom Left | Top Right | Bottom Right)
        ///     = Window.MoveToScreenEdge($1);</code>
        /// Saying for example "Slam Top Left" moves the foreground window to the upper left corner of the work area.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveToScreenEdge(string edge)
        {
            edge = edge.ToLower();
            Rectangle windowBox = Win.GetForegroundWindowRect();
            Screen screen = GetScreenContaining(windowBox.Location);
            Rectangle screenBox = screen.WorkingArea;

            int x = windowBox.Left;
            int y = windowBox.Top;

            if (edge.IndexOf("left") >= 0)
                x = screenBox.Left;
            else if (edge.IndexOf("right") >= 0)
                x = screenBox.Right - windowBox.Width;

            if (edge.IndexOf("top") >= 0)
                y = screenBox.Top;
            else if (edge.IndexOf("bottom") >= 0)
                y = screenBox.Bottom - windowBox.Height;

            Win.SetForegroundWindowPosition(new Point(x, y));
        }
        
        static private Screen GetScreenContaining(Point p)
        {
            foreach (Screen screen in Screen.AllScreens)
                if (screen.Bounds.Contains(p))
                    return screen;
            return Screen.PrimaryScreen;
        }

    }

}
