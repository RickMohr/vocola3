using System;
using System.Drawing; // Point
using System.Windows.Forms; // Cursor, Screen
using Vocola;

namespace Library
{

    /// <summary>Functions to control the pointing device (e.g. a mouse, touchpad, trackball, etc.).</summary>
    /// <remarks>Functions such as <see cref="MoveTo(int,int)">MoveTo</see> which position the pointer do so relative to
    /// a particular rectangle such as the bounds of the foreground window or the screen. Window-relative positioning is
    /// the default because it tends to be used in more commands than screen-relative positioning. Several functions
    /// have two versions, one implicitly relative to the foreground window rectangle (e.g. <see
    /// cref="MoveTo(int,int)">MoveTo</see>) and one relative to a specified named rectangle (e.g. <see
    /// cref="MoveTo(int,int,string)">MoveTo</see>). 
    /// </remarks>
    public class Pointer : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // Click

        /// <summary>Clicks the left button associated with the pointing device.</summary>
        /// <remarks>In a Vocola command, the pseudo-keystroke <c>{LeftButton}</c> may be used as
        /// an alternative to calling this function, with the advantage that modifier keys such as
        /// <c>Shift</c> and <c>Control</c> may be added.</remarks>
        /// <example><code title="Click the left button">
        /// Touch = Pointer.Click();</code>
        /// Saying "Touch" clicks the left button at the current pointer location.
        /// </example>
        /// <seealso cref="Click(int)">Click(button)</seealso>
        /// <seealso cref="Click(int, int)">Click(button, count)</seealso>
        [VocolaFunction]
        static public void Click()
        {
            Win.ButtonClick(1, 1);
        }

        /// <summary>Clicks a button associated with the pointing device.</summary>
        /// <param name="button">A number specifying which button(s) to
        /// click. Use 1 for the left button, 2 for the right button, 4 for the middle button,
        /// or add the numbers to click multiple buttons simultaneously.</param>
        /// <remarks>In a Vocola command, the pseudo-keystrokes <c>{LeftButton}</c>, <c>{RightButton}</c>,
        /// and <c>{MiddleButton}</c> may be used as alternatives to calling this function, with the advantage that
        /// modifier keys such as <c>Shift</c> and <c>Control</c> may be added.</remarks>
        /// <example><code title="Click the right button">
        /// Touch Both = Pointer.Click(3);</code>
        /// Saying "Touch Both" clicks the left and right buttons simultaneously.
        /// </example>
        /// <seealso cref="Click()">Click()</seealso>
        /// <seealso cref="Click(int, int)">Click(button, count)</seealso>
        [VocolaFunction]
        static public void Click(int button)
        {
            Win.ButtonClick(button, 1);
        }

        /// <summary>Clicks a button associated with the pointing device a specified number of times.</summary>
        /// <param name="button">A number specifying which button(s) to
        /// click. Use 1 for the left button, 2 for the right button, 4 for the middle button,
        /// or add the numbers to click multiple buttons simultaneously.</param>
        /// <param name="count">A number specifying how many times to click the button(s).</param>
        /// <remarks>In a Vocola command, the pseudo-keystrokes <c>{LeftButton}</c>, <c>{RightButton}</c>,
        /// and <c>{MiddleButton}</c> may be used as alternatives to calling this function, with the advantage that
        /// modifier keys such as <c>Shift</c> and <c>Control</c> may be added.</remarks>
        /// <example><code title="Double-click the left button">
        /// Touch Double = Pointer.Click(1,2);</code>
        /// Saying "Touch Double" double-clicks the left button at the current pointer location.
        /// </example>
        /// <seealso cref="Click()">Click()</seealso>
        /// <seealso cref="Click(int)">Click(button)</seealso>
        [VocolaFunction]
        static public void Click(int button, int count)
        {
            Win.ButtonClick(button, count);
        }

        // ---------------------------------------------------------------------
        // DragBy
        // DragByScreenPercent

        /// <summary>Drags the pointer by a specified number of pixels.</summary>
        /// <param name="dx">Number of pixels to drag horizontally.</param>
        /// <param name="dy">Number of pixels to drag vertically.</param>
        /// <remarks>Drags the pointer from the current position to a new position, <paramref name="dx"/>
        /// pixels away horizontally and <paramref name="dy"/> away vertically.
        /// <para>See <see cref="DragByScreenPercent"/> for an example.</para></remarks>
        [VocolaFunction]
        static public void DragBy(int dx, int dy)
        {
            Win.DragToPoint(Cursor.Position, Cursor.Position + new Size(dx, dy));
        }

        /// <summary>Drags the pointer by a specified number of screen percentage units.</summary>
        /// <param name="widthPercent">Number of screen percentage units to drag horizontally.</param>
        /// <param name="heightPercent">Number of screen percentage units to drag vertically.</param>
        /// <remarks>The screen percentage coordinate system is useful for commands
        /// where you speak the coordinates of a pointer position or offset. It has (0,0) at the upper left corner
        /// of the screen and (100,100) at the lower right corner of the screen.
        /// <para>This function drags the pointer from the current position to a new position, <paramref name="dx"/>
        /// screen percentage units away horizontally and <paramref name="dy"/> away vertically.</para></remarks>
        /// <example><code title="Resize window vertically">
        /// Window (Top | Bottom) 1..99 (Up='-' |  Down='+')
        ///     = MoveToEdge($1) DragByScreenPercent(0, $3$2);</code>
        /// Saying for example "Window Bottom 3 Up" resizes the current window by dragging the bottom edge up 3
        /// screen percentage units. <c>$1</c> is "Bottom", so <c>MoveToEdge(Bottom)</c> moves the pointer to
        /// the bottom edge of the current window. <c>$3</c> is "-" and <c>$2</c> is "3", so <c>DragByScreenPercent(0, -3)</c>
        /// drags the pointer upwards 3 screen percentage units.
        /// </example>
        [VocolaFunction]
        static public void DragByScreenPercent(int widthPercent, int heightPercent)
        {
            Point p = GetPointAsScreenPercent(widthPercent, heightPercent);
            Win.DragToPoint(Cursor.Position, Cursor.Position + new Size(p.X, p.Y));
        }

/*
        /// <summary>Drags the pointer from the specified point to the current pointer position.</summary>
        /// <param name="x">Horizontal coordinate to drag from.</param>
        /// <param name="y">Vertical coordinate to drag from.</param>
        [VocolaFunction]
        static public void DragFrom(int x, int y)
        {
            Point from = GetOffsetCornerPoint("Window", x, y);
            Win.DragToPoint(from, Cursor.Position);
        }
*/

        // ---------------------------------------------------------------------
        // GetOffset

        /// <summary>Returns the current pointer position as an offset in pixels from the nearest corner of the current window.</summary>
        /// <returns>The current pointer position, using the syntax "(x,y)"</returns>
        /// <remarks>When writing a command which positions the pointer you can place the pointer in an interesting location,
        /// use this function to discover its coordinates, and use them as arguments to <see cref="MoveTo(int,int)"/>.</remarks>
        /// <example><code title="Get current pointer offset">
        /// Get Pointer = Clipboard.SetText(Pointer.GetOffset());</code>
        /// This command gets the pointer offset in pixels from the nearest window corner and copies it to the clipboard.
        /// </example>
        /// <seealso cref="GetOffset(string)">GetOffset(rectangleName)</seealso>
        [VocolaFunction]
        static public string GetOffset()
        {
            return GetOffsetFromNearestBoxCorner(Win.GetForegroundWindowRect());
        }

        /// <summary>Returns the current pointer position as an offset in pixels from the nearest corner of the
        /// specified rectangle.</summary>
        /// <param name="rectangleName">Name of rectangle from whose nearest corner the pointer offset
        /// should be measured. Case insensitive. Choices: 
        /// <list type="bullet">
        /// <item><c>Window</c> - Current window's bounding rectangle, as with <see cref="GetOffset()">GetOffset()</see>.</item>
        /// <item><c>WindowInner</c> - Current window's inner rectangle (or "client area"), i.e. the part not occupied
        /// by the title bar, toolbars, etc.</item> 
        /// <item><c>Screen</c> - Screen's bounding rectangle.</item>
        /// <item><c>ScreenInner</c> - Screen's inner rectangle (or "work area"), i.e. the part not occupied by the
        /// taskbar, docked windows, etc.</item> 
        /// <item><c>Screen2</c> - Bounding rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// <item><c>ScreenInner2</c> - Inner rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// </list>
        /// </param>
        /// <returns>The current pointer position, using the syntax "(x,y)"</returns>
        /// <remarks>When writing a command which positions the pointer you can place the pointer in an interesting location,
        /// use this function to discover its coordinates, and use them as arguments to <see cref="MoveTo(int,int,string)"/>.</remarks>
        /// <example><code title="Get current pointer offset">
        /// Get Pointer = Clipboard.SetText(Pointer.GetOffset(WindowInner));</code>
        /// This command gets the pointer offset in pixels from the nearest corner of the current
        /// window's inner rectangle and copies it to the clipboard.
        /// </example>
        /// <seealso cref="GetOffset()">GetOffset()</seealso>
        [VocolaFunction]
        static public string GetOffset(string rectangleName)
        {
            return GetOffsetFromNearestBoxCorner(GetBox(rectangleName));
        }

        static private string GetOffsetFromNearestBoxCorner(Rectangle box)
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            if (x < (box.Left + box.Right) / 2)
                x = x - box.Left;
            else
                x = x - box.Right;
            if (y < (box.Top + box.Bottom) / 2)
                y = y - box.Top;
            else
                y = y - box.Bottom;
            return System.String.Format("({0},{1})", x, y);
        }

        // ---------------------------------------------------------------------
        // MoveBy

        /// <summary>Moves the pointer by a specified number of pixels.</summary>
        /// <param name="dx">Number of pixels to move horizontally.</param>
        /// <param name="dy">Number of pixels to move vertically.</param>
        /// <remarks>Moves the pointer from the current position to a new position, <paramref name="dx"/>
        /// pixels away horizontally and <paramref name="dy"/> away vertically.</remarks>
        /// <example><code title="Move a window">
        /// [Move] Window 1..99 (Up='-' |  Down='+')
        ///      = MoveToEdge(Top) MoveBy(0,20) DragByScreenPercent($2$1, 0);</code>
        /// Saying for example "Move Window 5 Left" moves the current window by dragging the title bar left by 5
        /// screen percentage units. <c>MoveToEdge(Top)</c> moves the pointer to the top edge of the current window.
        /// But dragging the pointer from this position would resize the window rather than moving it, so
        /// <c>MoveBy(0,20)</c> moves the pointer 20 pixels down to be over the title bar.
        /// <c>$2</c> is "-" and <c>$1</c> is "5", so <c>DragByScreenPercent(-5, 0)</c>
        /// drags the pointer left by 5 screen percentage units.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveBy(int dx, int dy)
        {
            Cursor.Position += new Size(dx, dy);
        }

        // ---------------------------------------------------------------------
        // MoveTo

        /// <summary>Move the pointer to a new position, relative to a corner of the current window.</summary>
        /// <param name="dx">Horizontal offset in pixels from edge of current window. A positive number means
        /// the offset is relative to the left edge, while a negative number means the offset is relative to the right edge.</param>
        /// <param name="dy">Vertical offset in pixels from edge of current window. A positive number means
        /// the offset is relative to the top edge, while a negative number means the offset is relative to the bottom
        /// edge.</param>
        /// <remarks>Using pixel coordinates to position the pointer is not
        /// the best way to control an application, but sometimes it's the only way.</remarks>
        /// <example><code title="Compose HTML message in Thunderbird">
        /// New Message HTML = MoveTo(110, 83) {Shift+LeftButton};</code>
        /// If the Thunderbird mailer is configured to compose messages in plain text format, the only way to compose a message
        /// in HTML format is to shift-click on the "Write" button in the toolbar. This command does that, by calling
        /// <c>MoveTo(110, 83)</c> to position the pointer over the "Write" button (you may need to adjust the coordinates)
        /// and sending <c>{Shift+LeftButton}</c> to perform the shift-click.
        /// </example>
        /// <seealso cref="MoveTo(int, int, string)">MoveTo(dx, dy, rectangleName)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveTo(int dx, int dy)
        {
            MoveTo(dx, dy, "Window");
        }

        /// <summary>Move the pointer to a new position, relative to a corner of the specified rectangle.</summary>
        /// <param name="dx">Horizontal offset in pixels from edge of specified rectangle. A positive number means
        /// the offset is relative to the left edge, while a negative number means the offset is relative to the right edge.</param>
        /// <param name="dy">Vertical offset in pixels from edge of specified rectangle. A positive number means
        /// the offset is relative to the top edge, while a negative number means the offset is relative to the bottom
        /// edge.</param>
        /// <param name="rectangleName">Name of rectangle from whose nearest corner <paramref name="dx"/> and
        /// <paramref name="dy"/> should be measured. Case insensitive. Choices:
        /// <list type="bullet">
        /// <item><c>Window</c> - Current window's bounding rectangle, as with <see cref="MoveTo(int,int)">MoveTo(dx,dy)</see>.</item>
        /// <item><c>WindowInner</c> - Current window's inner rectangle (or "client area"), i.e. the part not occupied
        /// by the title bar, toolbars, etc.</item> 
        /// <item><c>Screen</c> - Screen's bounding rectangle.</item>
        /// <item><c>ScreenInner</c> - Screen's inner rectangle (or "work area"), i.e. the part not occupied by the
        /// taskbar, docked windows, etc.</item> 
        /// <item><c>Screen2</c> - Bounding rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// <item><c>ScreenInner2</c> - Inner rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// </list>
        /// </param>
        /// <remarks>Using pixel coordinates to position the pointer is not
        /// the best way to control an application, but sometimes it's the only way.</remarks>
        /// <example><code title="Copy full pathname in Visual Studio">
        /// Copy File Name = MoveTo(5, 100, WindowInner) {RightButton}{Down_4} Wait(500) {Enter};</code>
        /// Visual Studio has no easy way to copy the full pathname of the current document by voice. But if you right-click on
        /// the little triangular cutout to the left of the leftmost tab you get a context menu with a "Copy Full Path"
        /// option. This command does that, by calling
        /// <c>MoveTo(5, 100, WindowInner)</c> to position the pointer over the cutout (you may need to adjust the coordinates)
        /// and sending keystrokes to choose the menu option.
        /// </example>
        /// <seealso cref="MoveTo(int, int)">MoveTo(dx, dy)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveTo(int dx, int dy, string rectangleName)
        {
            Cursor.Position = GetOffsetCornerPoint(rectangleName, dx, dy);
        }

#pragma warning disable 1591  // Don't complain about missing XML comments
        static public Point GetOffsetCornerPoint(string rectangleName, int dx, int dy)
        {
            Rectangle box = GetBox(rectangleName);
            Point point;
            if      (dx >= 0 && dy >= 0) point = GetEdgePoint(box, "Top Left");
            else if (dx <  0 && dy >= 0) point = GetEdgePoint(box, "Top Right");
            else if (dx <  0 && dy <  0) point = GetEdgePoint(box, "Bottom Right");
            else                         point = GetEdgePoint(box, "Bottom Left");
            point += new Size(dx, dy);
            return point;
        }
#pragma warning restore 1591

        static private Rectangle GetBox(string rectangleName)
        {
            switch (rectangleName)
            {
            case "Window":
                return Win.GetForegroundWindowRect();
            case "WindowInner":
                return Win.GetForegroundWindowClientRect();
            case "Screen":
                return Screen.PrimaryScreen.Bounds;
            case "ScreenInner":
                return Screen.PrimaryScreen.WorkingArea;
            default:
            {
                int screenNumber;
                if (rectangleName.StartsWith("Screen")
                    && Int32.TryParse(rectangleName.Substring(6), out screenNumber)
                    && screenNumber > 0 && Screen.AllScreens.Length >= screenNumber)
                {
                    return Screen.AllScreens[screenNumber-1].Bounds;
                }
                else if (rectangleName.StartsWith("ScreenInner")
                    && Int32.TryParse(rectangleName.Substring(11), out screenNumber)
                    && screenNumber > 0 && Screen.AllScreens.Length >= screenNumber)
                {
                    return Screen.AllScreens[screenNumber-1].WorkingArea;
                }
                else
                    throw new VocolaExtensionException("Unknown box name: '{0}'", rectangleName);
            }
            }
        }

        static private Point GetEdgePoint(Rectangle box, string edge)
        {
            edge = edge.ToLower();
            int x, y;

            if (edge.IndexOf("left") >= 0)
                x = box.Left;
            else if (edge.IndexOf("right") >= 0)
                x = box.Right - 1;
            else
                x = (box.Left + box.Right) / 2;

            if (edge.IndexOf("top") >= 0)
                y = box.Top;
            else if (edge.IndexOf("bottom") >= 0)
                y = box.Bottom - 1;
            else
                y = (box.Top + box.Bottom) / 2;

            return new Point(x, y);
        }

        // ---------------------------------------------------------------------
        // MoveToEdge

        /// <summary>Move the pointer to an edge of the current window.</summary>
        /// <param name="edge">Named edge of current window: <c>Top</c>, <c>Right</c>, <c>Bottom</c>, <c>Left</c>, 
        /// <c>Top Right</c>, <c>Bottom Right</c>, <c>Bottom Left</c>, or <c>Top Left</c>. Case insensitive.</param>
        /// <remarks>See <see cref="DragByScreenPercent"/> for an example which also calls this function.</remarks>
        /// <seealso cref="MoveToEdge(string, string)">MoveToEdge(edge, rectangleName)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveToEdge(string edge)
        {
            Cursor.Position = GetEdgePoint(Win.GetForegroundWindowRect(), edge);
        }

        /// <summary>Move the pointer to an edge of the specified rectangle.</summary>
        /// <param name="edge">Named edge of specified rectangle: <c>Top</c>, <c>Right</c>, <c>Bottom</c>, <c>Left</c>, 
        /// <c>Top Right</c>, <c>Bottom Right</c>, <c>Bottom Left</c>, or <c>Top Left</c>. Case insensitive.</param>
        /// <param name="rectangleName">Name of rectangle whose edge is desired. Case insensitive. Choices: 
        /// <list type="bullet">
        /// <item><c>Window</c> - Current window's bounding rectangle, as with <see cref="MoveToEdge(string)">MoveToEdge(edge)</see>.</item>
        /// <item><c>WindowInner</c> - Current window's inner rectangle (or "client area"), i.e. the part not occupied
        /// by the title bar, toolbars, etc.</item> 
        /// <item><c>Screen</c> - Screen's bounding rectangle.</item>
        /// <item><c>ScreenInner</c> - Screen's inner rectangle (or "work area"), i.e. the part not occupied by the
        /// taskbar, docked windows, etc.</item> 
        /// <item><c>Screen2</c> - Bounding rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// <item><c>ScreenInner2</c> - Inner rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// </list>
        /// </param>
        /// <remarks>See <see cref="DragByScreenPercent"/> for an example which also calls <see
        /// cref="MoveToEdge(string)"/>.</remarks>
        /// <seealso cref="MoveToEdge(string)">MoveToEdge(edge)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveToEdge(string edge, string rectangleName)
        {
            Cursor.Position = GetEdgePoint(GetBox(rectangleName), edge);
        }

        // ---------------------------------------------------------------------
        // MoveToScreenPercent

        /// <summary>Moves the pointer to a new position specified in screen percentage units.</summary>
        /// <param name="widthPercent">New horizontal position, in screen percentage units.</param>
        /// <param name="heightPercent">New vertical position, in screen percentage units.</param>
        /// <remarks>The screen percentage coordinate system is useful for commands
        /// where you speak the coordinates of a pointer position or offset. It has (0,0) at the upper left corner
        /// of the screen and (100,100) at the lower right corner of the screen.</remarks>
        /// <example><code title="Click a point on the screen">
        /// &lt;n> := 0..100;
        /// &lt;n> By &lt;n> Touch = MoveToScreenPercent($2, $1) {LeftButton};</code>
        /// This command (invented by Kim Patch) allows clicking a point on the screen. It's much faster than using the
        /// mouse grid, and surprisingly accurate with a little practice. Saying for example "10 By 20 Touch" moves the pointer to
        /// a point near the upper left corner of the screen and presses the left button.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveToScreenPercent(int widthPercent, int heightPercent)
        {
            Point p = GetPointAsScreenPercent(widthPercent, heightPercent);
            MoveTo(p.X, p.Y, "Screen");
        }

        static private Point GetPointAsScreenPercent(int widthPercent, int heightPercent)
        {
            Rectangle screenBox = Screen.PrimaryScreen.Bounds;
            int x = (int)Math.Round(widthPercent  / 100.0 * screenBox.Width);
            int y = (int)Math.Round(heightPercent / 100.0 * screenBox.Height);
            return new Point(x, y);
        }

        // ---------------------------------------------------------------------
        // SavePoint
        // DragFromSavedPoint
        // MoveToSavedPoint

        static private Point NullPoint = new Point(-1, -1);
        static private Point SavedPoint = NullPoint;

        /// <summary>Saves the current pointer position.</summary>
        /// <remarks>Saves the current pointer position for a future call to <see cref="DragFromSavedPoint"/> or
        /// <see cref="MoveToSavedPoint"/>.
        /// <para>See <see cref="MoveToSavedPoint"/> for an example.</para></remarks>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void SavePoint()
        {
            SavedPoint = Cursor.Position;
        }

        /// <summary>Drags the pointer from the position stored by <see cref="SavePoint"/> to the current position.</summary>
        /// <remarks>If <see cref="SavePoint"/> was not called, 
        /// this function aborts the calling command and displays an error message in the Vocola Log window.</remarks>
        [VocolaFunction]
        static public void DragFromSavedPoint()
        {
            if (SavedPoint == NullPoint)
                throw new VocolaExtensionException("You must call SavePoint() before calling DragFromSavedPoint()");
            Win.DragToPoint(SavedPoint, Cursor.Position);
            SavedPoint = NullPoint;
        }

        /// <summary>Moves the pointer to the position stored by <see cref="SavePoint"/>.</summary>
        /// <remarks>If <see cref="SavePoint"/> was not called, 
        /// this function aborts the calling command and displays an error message in the Vocola Log window.</remarks>
        /// <example><code title="Touch and return">
        /// touchAndReturn(x,y) := SavePoint() Touch($x,$y) MoveToSavedPoint();
        /// Play That = touchAndReturn(322, 70);
        /// Stop That = touchAndReturn(476, 70);</code>
        /// The commands in this example (for the sound editor Audacity) use pointer coordinates to click buttons
        /// which lack labels and keyboard shortcuts. "Play That" clicks the "play" button and "Stop That"
        /// clicks the "stop" button. To avoid losing the current pointer position both commands use the
        /// <c>touchAndReturn</c> user function, which saves the current pointer position using
        /// <see cref="SavePoint"/>, clicks the appropriate button using <see cref="Touch(int,int)"/>, and restores the pointer
        /// position using <see cref="MoveToSavedPoint"/>.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void MoveToSavedPoint()
        {
            if (SavedPoint == NullPoint)
                throw new VocolaExtensionException("You must call SavePoint() before calling MoveToSavedPoint()");
            Cursor.Position = SavedPoint;
            SavedPoint = NullPoint;
        }

        // ---------------------------------------------------------------------
        // Touch

        /// <summary>Clicks the specified point, relative to a corner of the current window.</summary>
        /// <param name="dx">Horizontal offset in pixels from edge of current window. A positive number means
        /// the offset is relative to the left edge, while a negative number means the offset is relative to the right edge.</param>
        /// <param name="dy">Vertical offset in pixels from edge of current window. A positive number means
        /// the offset is relative to the top edge, while a negative number means the offset is relative to the bottom edge.</param>
        /// <remarks>This concise and useful function is equivalent to <see cref="MoveTo(int,int)"/>
        /// followed by <see cref="Click()">Click</see>. 
        /// <para>Using pixel coordinates to position the pointer is not
        /// the best way to control an application, but sometimes it's the only way.</para></remarks>
        /// <example><code title="Switch to a Firefox tab by number">
        /// Tab 1..9 = Touch(-20, 145) Wait(100) {Down_$1}{Enter};</code>
        /// Firefox has no menu item or keyboard shortcut for switching to a particular tab.
        /// It does however have a drop down menu of available tabs at the right edge of the tab bar.
        /// <c>Touch(-20, 145)</c> clicks that menu (you may need to adjust the coordinates). Note
        /// that -20 specifies a horizontal offset in pixels from the right edge of the window.
        /// <c>{Down_$1}</c> moves to the desired tab in the menu, and <c>{Enter}</c> selects it.
        /// </example>
        /// <seealso cref="Touch(int, int, string)">Touch(dx, dy, rectangleName)</seealso>
        [VocolaFunction]
        static public void Touch(int dx, int dy)
        {
            MoveTo(dx, dy);
            Click();
        }

        /// <summary>Clicks the specified point, relative to a corner of the specified rectangle.</summary>
        /// <param name="dx">Horizontal offset in pixels from edge of specified rectangle. A positive number means
        /// the offset is relative to the left edge, while a negative number means the offset is relative to the right edge.</param>
        /// <param name="dy">Vertical offset in pixels from edge of specified rectangle. A positive number means
        /// the offset is relative to the top edge, while a negative number means the offset is relative to the bottom edge.</param>
        /// <param name="rectangleName">Name of rectangle from whose nearest corner <paramref name="dx"/> and
        /// <paramref name="dy"/> should be measured. Case insensitive. Choices:
        /// <list type="bullet">
        /// <item><c>Window</c> - Current window's bounding rectangle, as with <see cref="Touch(int,int)">MoveTo(dx,dy)</see>.</item>
        /// <item><c>WindowInner</c> - Current window's inner rectangle (or "client area"), i.e. the part not occupied
        /// by the title bar, toolbars, etc.</item> 
        /// <item><c>Screen</c> - Screen's bounding rectangle.</item>
        /// <item><c>ScreenInner</c> - Screen's inner rectangle (or "work area"), i.e. the part not occupied by the
        /// taskbar, docked windows, etc.</item> 
        /// <item><c>Screen2</c> - Bounding rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// <item><c>ScreenInner2</c> - Inner rectangle of a second screen, if present. Additional
        /// screens, if present, may be similarly referenced by number.</item> 
        /// </list>
        /// </param>
        /// <remarks>This concise and useful function is equivalent to <see cref="MoveTo(int,int,string)"/>
        /// followed by <see cref="Click()">Click</see>. 
        /// <para>Using pixel coordinates to position the pointer is not
        /// the best way to control an application, but sometimes it's the only way.</para></remarks>
        /// <example><code title="Switch to a Firefox tab by number">
        /// Tab 1..9 = Touch(-20, 145) Wait(100) {Down_$1}{Enter};</code>
        /// Firefox has no menu item or keyboard shortcut for switching to a particular tab.
        /// It does however have a drop down menu of available tabs at the right edge of the tab bar.
        /// <c>Touch(-20, 145)</c> clicks that menu (you may need to adjust the coordinates). Note
        /// that -20 specifies a horizontal offset in pixels from the right edge of the window.
        /// <c>{Down_$1}</c> moves to the desired tab in the menu, and <c>{Enter}</c> selects it.
        /// </example>
        /// <seealso cref="Touch(int, int)">Touch(dx, dy)</seealso>
        [VocolaFunction]
        static public void Touch(int dx, int dy, string rectangleName)
        {
            MoveTo(dx, dy, rectangleName);
            Click();
        }
        
    }

}
