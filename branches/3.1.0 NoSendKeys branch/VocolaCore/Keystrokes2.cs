using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Vocola
{

    public class Keystrokes
    {

        private List<Keystroke> TheKeystrokes;

        // ---------------------------------------------------------------------
        // Entry points

        static public void SendSystemKeys(string keys)
        {
            Keystrokes ks = new Keystrokes(keys);
            ks.SendSystemKeys();
        }

        static public void SendKeys(string keys)
        {
            Keystrokes ks = new Keystrokes(keys);
            ks.SendKeys();
        }

        static public void SendText(string text)
        {
            Keystrokes ks = new Keystrokes(text.ToCharArray());
            ks.SendKeys();
        }

        static public void SendTextUsingSeparateThread(string text)
        {
            // Launch thread to send text, hopefully avoiding duplicated keystrokes bug
            Thread thread = new Thread(new ParameterizedThreadStart(SendText));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "Send Keys Thread";
            thread.Start(text);
        }

        static private void SendText(object text)
        {
            SendText((string)text);
        }

        // ---------------------------------------------------------------------
        // Constructor -- convert "keys" string to keystroke objects

        private Keystrokes(string keys)
        {
            TheKeystrokes = ParseKeystrokes(keys);
        }

        private Keystrokes(Char[] chars)
        {
            TheKeystrokes = new List<Keystroke>();
            foreach (Char c in chars)
                TheKeystrokes.Add(new Keystroke(c));
        }

        static private List<Keystroke> ParseKeystrokes(string keys)
        {
            List<Keystroke> keystrokes = new List<Keystroke>();
            while (keys != "")
            {
                if (keys.StartsWith("{"))
                {
                    // Look for a valid key name delimited by braces
                    int startIndex = 1;
                    while (true)
                    {
                        int rightBraceIndex = keys.IndexOf("}", startIndex);
                        if (rightBraceIndex == -1)
                        {
                            // No right brace found -- add a "{" character
                            keystrokes.Add(new Keystroke('{'));
                            keys = keys.Substring(1);
                            break;
                        }
                        string keyText = keys.Substring(1, rightBraceIndex - 1);
                        Keystroke keystroke = ParseKeystroke(keyText);
                        if (keystroke != null)
                        {
                            // Valid key name found -- add it
                            keystrokes.Add(keystroke);
                            keys = keys.Substring(rightBraceIndex + 1);
                            break;
                        }
                        startIndex = rightBraceIndex + 1;
                    }
                }
                else
                {
                    // Just a regular character
                    keystrokes.Add(new Keystroke(keys[0]));
                    keys = keys.Substring(1);
                }
            }
            return keystrokes;
        }

        static public Keystroke ParseKeystroke(string keyText)
        {
            Keystroke keystroke = new Keystroke(keyText);
            keyText = ParseModifierKeys(keyText, keystroke);
            try
            {
                keyText = ParseKeystrokeCount(keyText, keystroke);
                return ParseKeystrokeName(keyText, keystroke);
            }
            catch
            {
                // It's not well-formed
                return null;
            }
        }

        static private string ParseModifierKeys(string keyText, Keystroke keystroke)
        {
            while (true)
            {
                string keyTextLower = keyText.ToLower();
                if (keyTextLower.StartsWith("shift+"))
                {
                    keystroke.Shift = true;
                    keyText = keyText.Substring(6);
                }
                else if (keyTextLower.StartsWith("ctrl+"))
                {
                    keystroke.Control = true;
                    keyText = keyText.Substring(5);
                }
                else if (keyTextLower.StartsWith("alt+"))
                {
                    keystroke.Alternate = true;
                    keyText = keyText.Substring(4);
                }
                else if (keyTextLower.StartsWith("win+"))
                {
                    keystroke.Windows = true;
                    keyText = keyText.Substring(4);
                }
                else
                    return keyText;
            }
        }

        static private char[] keystrokeCountDelimiters = new char[] { ' ', '_' };

        static private string ParseKeystrokeCount(string keyText, Keystroke keystroke)
        {
            if (keyText.StartsWith(" ") || keyText.StartsWith("_"))
            {
                // Keystroke is a space or underscore
                string[] parts = keyText.Substring(1).Split(keystrokeCountDelimiters);
                if (parts.Length == 2 && parts[0] == "")
                {
                    ReallyParseKeystrokeCount(parts[1], keystroke);
                    keyText = keyText.Substring(0,1);
                }
            }
            else
            {
                // Keystroke is anything else
                string[] parts = keyText.Split(keystrokeCountDelimiters);
                if (parts.Length == 2)
                {
                    ReallyParseKeystrokeCount(parts[1], keystroke);
                    keyText = parts[0];
                }
            }
            return keyText;
        }

        static private void ReallyParseKeystrokeCount(string countText, Keystroke keystroke)
        {
            if (countText.ToLower() == "hold")
                keystroke.Up = false;
            else if (countText.ToLower() == "release")
                keystroke.Down = false;
            else
                keystroke.Count = UInt32.Parse(countText);
        }

        static private Keystroke ParseKeystrokeName(string keyName, Keystroke keystroke)
        {
            if (keyName.Length == 1)
            {
                // It's a single character
                keystroke.UpdateForCharacter(keyName[0]);
                return keystroke; 
            }
            if (keystroke.AddVirtualKeyCode(keyName))
            {
                // It's a Vocola key name
                return keystroke; 
            }
            else if (keyName.StartsWith("U+"))
            {
                // It's a unicode hex character, e.g. {U+2014} for Em Dash
                char c = (char)UInt16.Parse(keyName.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
                keystroke.UpdateForCharacter(c);
                return keystroke; 
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Keystroke keystroke in TheKeystrokes)
                sb.Append(keystroke);
            return sb.ToString();
        }

        // ---------------------------------------------------------------------
        // Send Keys

        private void SendKeys()
        {
            Trace.WriteLine(LogLevel.Low, "    Keystrokes: '{0}'", this);
            StringBuilder sb = new StringBuilder();
            foreach (Keystroke keystroke in TheKeystrokes)
            {
                if (keystroke.VirtualKeyCode != 0)
                {
                    // Don't use SendKeys for this keystroke.
                    // Send any accumulated keys and then use SendSystemKeys
                    if (sb.Length > 0)
                    {
                        ReallySendKeys(sb.ToString());
                        sb.Remove(0, sb.Length);
                    }
                    SendSystemKeys(keystroke);
                }
                else
                    // Use SendKeys for this keystroke
                    AppendKeystrokeForSendKeys(keystroke, sb);
            }
            if (sb.Length > 0)
                ReallySendKeys(sb.ToString());
        }

        static private void AppendKeystrokeForSendKeys(Keystroke k, StringBuilder sb)
        {
            if (k.Count == 0)
                return;
            if (k.Shift)
                sb.Append('+');
            if (k.Control)
                sb.Append('^');
            if (k.Alternate)
                sb.Append('%');
            bool useBraces = (k.Count > 1);
            if (useBraces)
                sb.Append('{');
            sb.Append(k.Char);
            if (k.Count > 1)
                sb.Append(String.Format(" {0}", k.Count));
            if (useBraces)
                sb.Append('}');
        }

        static private void ReallySendKeys(string keys)
        {
            Trace.WriteLine(LogLevel.Low, "    SendKeys Keystrokes: '{0}'", keys);
            try
            {
                System.Windows.Forms.SendKeys.Flush();
                System.Windows.Forms.SendKeys.SendWait(keys);
            }
            catch (Exception e)
            {
                Trace.WriteLine(LogLevel.Error, "Exception sending keystrokes: {0}", e.Message);
            }
        }

        // ---------------------------------------------------------------------
        // Send System Keys

        const byte VK_LBUTTON    = 0x01;
        const byte VK_RBUTTON    = 0x02;
        const byte VK_MBUTTON    = 0x04;
        const byte VK_SHIFT      = 0x10;
        const byte VK_CONTROL    = 0x11;
        const byte VK_MENU       = 0x12;
        const byte VK_LWIN       = 0x5B;
        const byte VK_WHEEL_DOWN = 0xCE;
        const byte VK_WHEEL_UP   = 0xCF;

        private void SendSystemKeys()
        {
            foreach (Keystroke keystroke in TheKeystrokes)
                SendSystemKeys(keystroke);
        }

        static private void SendSystemKeys(Keystroke keystroke)
        {
            byte vk = keystroke.VirtualKeyCode;
            if (vk == 0)
                // This should only happen for SendSystemKeys API calls
                throw new ActionException(null, "Invalid key for SendSystemKeys: '{0}'", keystroke.Char);
            //Trace.WriteLine(LogLevel.Low, "    Send system keys: '{0}'", keystroke);

            if (Char.IsUpper(keystroke.Char))
                keystroke.Shift = true;

            if (keystroke.Shift)     Win.SendKeyEvent(VK_SHIFT,   "Shift", true);
            if (keystroke.Control)   Win.SendKeyEvent(VK_CONTROL, "Ctrl",  true);
            if (keystroke.Alternate) Win.SendKeyEvent(VK_MENU,    "Alt",   true);
            if (keystroke.Windows)   Win.SendKeyEvent(VK_LWIN,    "Win",   true);

            while (keystroke.Count-- > 0)
            {
                if (vk == VK_LBUTTON || vk == VK_RBUTTON || vk == VK_MBUTTON)
                {
                    if (keystroke.HasModifierKey())
                        Thread.Sleep(100); // Otherwise {Ctrl+LeftButton} is unreliable
                    if (keystroke.Down)
                        Win.SendButtonEvent(vk, true);
                    if (keystroke.Up)
                        Win.SendButtonEvent(vk, false);
                    Thread.Sleep(50); // Seems to be necessary when running from Program Files
                }
                else if (vk == VK_WHEEL_DOWN || vk == VK_WHEEL_UP)
                    Win.MoveMouseWheel(vk == VK_WHEEL_DOWN);
                else
                {
                    if (keystroke.Down)
                        Win.SendKeyEvent(vk, keystroke.VocolaName, true);
                    if (keystroke.Alternate && vk == 0x09) // {Alt+Tab}
                        Thread.Sleep(100); // Otherwise unreliable
                    if (keystroke.Up)
                        Win.SendKeyEvent(vk, keystroke.KeyText, false);
                }
            }

            if (keystroke.Windows)   Win.SendKeyEvent(VK_LWIN,    "Shift", false);
            if (keystroke.Alternate) Win.SendKeyEvent(VK_MENU,    "Ctrl",  false);
            if (keystroke.Control)   Win.SendKeyEvent(VK_CONTROL, "Alt",   false);
            if (keystroke.Shift)     Win.SendKeyEvent(VK_SHIFT,   "Win",   false);
        }

    }


    // ---------------------------------------------------------------------

    public class Keystroke
    {
        // Either VirtualKeyCode or Char is set

        public string KeyText = null;
        public string VocolaName = null;
        public byte   VirtualKeyCode = 0;
        public char   Char = (char)0;

        public bool   Shift = false;
        public bool   Control = false;  
        public bool   Alternate = false;
        public bool   Windows = false;
        public uint   Count = 1;

        public bool   Down = true;
        public bool   Up = true;

        public Keystroke(char c)
        {
            KeyText = c.ToString();
            UpdateForCharacter(c);
        }

        public Keystroke(string keyText)
        {
            KeyText = "{" + keyText + "}";
        }

        public void UpdateForCharacter(char c)
        {
            byte virtualKey;
            bool shift, control, alternate;
            if (Win.GetVirtualKey(c, out virtualKey, out shift, out control, out alternate))
            {
                VirtualKeyCode = virtualKey;
                Shift = Shift || shift;
                Control = Control || control;
                Alternate = Alternate || alternate;
                VocolaName = c.ToString().ToLower();
            }
            else
                // It's a character that can't be sent using virtual keystrokes
                Char = c;
        }

        public override string ToString()
        {
            if (KeyText != null)
                return KeyText;
            else
                return Char.ToString();
        }

        public bool HasModifierKey()
        {
            return Shift | Control | Alternate | Windows;
        }

        public bool AddVirtualKeyCode(string keyName)
        {
            string keyNameUpperCase = keyName.ToUpper();
            if (VirtualKeyCodes.ContainsKey(keyNameUpperCase))
            {
                this.VirtualKeyCode = VirtualKeyCodes[keyNameUpperCase];
                this.VocolaName = keyName;
                return true;
            }
            return false;
        }

        // ---------------------------------------------------------------------
        // Key data

        static private Dictionary<string, byte> VirtualKeyCodes;

        static private void AddKey(string vocolaName, byte virtualKeyCode)
        {
            VirtualKeyCodes[vocolaName.ToUpper()] = virtualKeyCode;
        }

        static public void Initialize()
        {
            VirtualKeyCodes = new Dictionary<string, byte>();
            //      Vocola Name          Windows virtual key code
            AddKey("LeftButton"        , 0x01);  // VK_LBUTTON
            AddKey("RightButton"       , 0x02);  // VK_RBUTTON
            AddKey("Break"             , 0x03);  // VK_CANCEL
            AddKey("MiddleButton"      , 0x04);  // VK_MBUTTON
            AddKey("XButton1"          , 0x05);  // VK_XBUTTON1
            AddKey("XButton2"          , 0x06);  // VK_XBUTTON2
            AddKey("Backspace"         , 0x08);  // VK_BACK
            AddKey("Tab"               , 0x09);  // VK_TAB
            AddKey("Clear"             , 0x0C);  // VK_CLEAR
            AddKey("Enter"             , 0x0D);  // VK_RETURN
            AddKey("Pause"             , 0x13);  // VK_PAUSE
            AddKey("CapsLock"          , 0x14);  // VK_CAPITAL
            AddKey("Kana"              , 0x15);  // VK_KANA
            AddKey("Junja"             , 0x17);  // VK_JUNJA
            AddKey("Final"             , 0x18);  // VK_FINAL
            AddKey("Kanji"             , 0x19);  // VK_KANJI
            AddKey("Escape"            , 0x1B);  // VK_ESCAPE
            AddKey("Esc"               , 0x1B);
            AddKey("Convert"           , 0x1C);  // VK_CONVERT
            AddKey("NonConvert"        , 0x1D);  // VK_NONCONVERT
            AddKey("Accept"            , 0x1E);  // VK_ACCEPT
            AddKey("ModeChange"        , 0x1F);  // VK_MODECHANGE
            AddKey("Space"             , 0x20);  // VK_SPACE
            AddKey("PageUp"            , 0x21);  // VK_PRIOR
            AddKey("PgUp"              , 0x21);
            AddKey("ExtPgUp"           , 0x21);
            AddKey("PageDown"          , 0x22);  // VK_NEXT
            AddKey("ExtPgDn"           , 0x22);
            AddKey("PgDn"              , 0x22);
            AddKey("End"               , 0x23);  // VK_END
            AddKey("ExtEnd"            , 0x23);
            AddKey("Home"              , 0x24);  // VK_HOME
            AddKey("ExtHome"           , 0x24);
            AddKey("Left"              , 0x25);  // VK_LEFT
            AddKey("ExtLeft"           , 0x25);
            AddKey("Up"                , 0x26);  // VK_UP
            AddKey("ExtUp"             , 0x26);
            AddKey("Right"             , 0x27);  // VK_RIGHT
            AddKey("ExtRight"          , 0x27);
            AddKey("Down"              , 0x28);  // VK_DOWN
            AddKey("ExtDown"           , 0x28);
            AddKey("Select"            , 0x29);  // VK_SELECT
            AddKey("Print"             , 0x2A);  // VK_PRINT
            AddKey("Execute"           , 0x2B);  // VK_EXECUTE
            AddKey("PrintScreen"       , 0x2C);  // VK_SNAPSHOT
            AddKey("PrtSc"             , 0x2C);
            AddKey("Insert"            , 0x2D);  // VK_INSERT
            AddKey("Ins"               , 0x2D);
            AddKey("ExtIns"            , 0x2D);
            AddKey("Delete"            , 0x2E);  // VK_DELETE
            AddKey("Del"               , 0x2E);
            AddKey("ExtDel"            , 0x2E);
            AddKey("Help"              , 0x2F);  // VK_HELP
            AddKey("Win"               , 0x5B);  // VK_LWIN
            AddKey("LeftWin"           , 0x5B);
            AddKey("RightWin"          , 0x5C);  // VK_RWIN
            AddKey("ContextMenu"       , 0x5D);  // VK_APPS
            AddKey("Sleep"             , 0x5F);  // VK_SLEEP
            AddKey("NumKey0"           , 0x60);  // VK_NUMPAD0
            AddKey("NumKey1"           , 0x61);  // VK_NUMPAD1
            AddKey("NumKey2"           , 0x62);  // VK_NUMPAD2
            AddKey("NumKey3"           , 0x63);  // VK_NUMPAD3
            AddKey("NumKey4"           , 0x64);  // VK_NUMPAD4
            AddKey("NumKey5"           , 0x65);  // VK_NUMPAD5
            AddKey("NumKey6"           , 0x66);  // VK_NUMPAD6
            AddKey("NumKey7"           , 0x67);  // VK_NUMPAD7
            AddKey("NumKey8"           , 0x68);  // VK_NUMPAD8
            AddKey("NumKey9"           , 0x69);  // VK_NUMPAD9
            AddKey("NumKey*"           , 0x6A);  // VK_MULTIPLY
            AddKey("Mult"              , 0x6A);
            AddKey("Asterisk"          , 0x6A);
            AddKey("NumKey+"           , 0x6B);  // VK_ADD
            AddKey("Plus"              , 0x6B);
            AddKey("NumKeyEnter"       , 0x6C);  // VK_SEPARATOR
            AddKey("KeyPadEnter"       , 0x6C);
            AddKey("NumKey-"           , 0x6D);  // VK_SUBTRACT
            AddKey("Minus"             , 0x6D);
            AddKey("NumKey."           , 0x6E);  // VK_DECIMAL
            AddKey("NumKey/"           , 0x6F);  // VK_DIVIDE
            AddKey("Slash"             , 0x6F);
            AddKey("F1"                , 0x70);  // VK_F1
            AddKey("F2"                , 0x71);  // VK_F2
            AddKey("F3"                , 0x72);  // VK_F3
            AddKey("F4"                , 0x73);  // VK_F4
            AddKey("F5"                , 0x74);  // VK_F5
            AddKey("F6"                , 0x75);  // VK_F6
            AddKey("F7"                , 0x76);  // VK_F7
            AddKey("F8"                , 0x77);  // VK_F8
            AddKey("F9"                , 0x78);  // VK_F9
            AddKey("F10"               , 0x79);  // VK_F10
            AddKey("F11"               , 0x7A);  // VK_F11
            AddKey("F12"               , 0x7B);  // VK_F12
            AddKey("F13"               , 0x7C);  // VK_F13
            AddKey("F14"               , 0x7D);  // VK_F14
            AddKey("F15"               , 0x7E);  // VK_F15
            AddKey("F16"               , 0x7F);  // VK_F16
            AddKey("F17"               , 0x80);  // VK_F17
            AddKey("F18"               , 0x81);  // VK_F18
            AddKey("F19"               , 0x82);  // VK_F19
            AddKey("F20"               , 0x83);  // VK_F20
            AddKey("F21"               , 0x84);  // VK_F21
            AddKey("F22"               , 0x85);  // VK_F22
            AddKey("F23"               , 0x86);  // VK_F23
            AddKey("F24"               , 0x87);  // VK_F24
            AddKey("NumLock"           , 0x90);  // VK_NUMLOCK
            AddKey("ScrollLock"        , 0x91);  // VK_SCROLL
            AddKey("Jisho"             , 0x92);  // VK_OEM_FJ_JISHO
            AddKey("Mashu"             , 0x93);  // VK_OEM_FJ_MASSHOU
            AddKey("Touroku"           , 0x94);  // VK_OEM_FJ_TOUROKU
            AddKey("Loya"              , 0x95);  // VK_OEM_FJ_LOYA
            AddKey("Roya"              , 0x96);  // VK_OEM_FJ_ROYA
            AddKey("Shift"             , 0xA0);  // VK_LSHIFT
            AddKey("LeftShift"         , 0xA0);  // VK_LSHIFT
            AddKey("RightShift"        , 0xA1);  // VK_RSHIFT
            AddKey("Ctrl"              , 0xA2);  // VK_LCONTROL
            AddKey("Control"           , 0xA2);  // VK_LCONTROL
            AddKey("LeftCtrl"          , 0xA2);  // VK_LCONTROL
            AddKey("RightCtrl"         , 0xA3);  // VK_RCONTROL
            AddKey("Alt"               , 0xA4);  // VK_LMENU
            AddKey("Alternate"         , 0xA4);  // VK_LMENU
            AddKey("LeftAlt"           , 0xA4);  // VK_LMENU
            AddKey("RightAlt"          , 0xA5);  // VK_RMENU
            AddKey("BrowserBack"       , 0xA6);  // VK_BROWSER_BACK
            AddKey("BrowserForward"    , 0xA7);  // VK_BROWSER_FORWARD
            AddKey("BrowserRefresh"    , 0xA8);  // VK_BROWSER_REFRESH
            AddKey("BrowserStop"       , 0xA9);  // VK_BROWSER_STOP
            AddKey("BrowserSearch"     , 0xAA);  // VK_BROWSER_SEARCH
            AddKey("BrowserFavorites"  , 0xAB);  // VK_BROWSER_FAVORITES
            AddKey("BrowserHome"       , 0xAC);  // VK_BROWSER_HOME
            AddKey("VolumeMute"        , 0xAD);  // VK_VOLUME_MUTE
            AddKey("VolumeDown"        , 0xAE);  // VK_VOLUME_DOWN
            AddKey("VolumeUp"          , 0xAF);  // VK_VOLUME_UP
            AddKey("MediaNextTrack"    , 0xB0);  // VK_MEDIA_NEXT_TRACK
            AddKey("MediaPreviousTrack", 0xB1);  // VK_MEDIA_PREV_TRACK
            AddKey("MediaStop"         , 0xB2);  // VK_MEDIA_STOP
            AddKey("MediaPlayPause"    , 0xB3);  // VK_MEDIA_PLAY_PAUSE
            AddKey("Mail"              , 0xB4);  // VK_LAUNCH_MAIL
            AddKey("Media"             , 0xB5);  // VK_LAUNCH_MEDIA_SELECT
            AddKey("App1"              , 0xB6);  // VK_LAUNCH_APP1
            AddKey("App2"              , 0xB7);  // VK_LAUNCH_APP2
            AddKey("AbntC1"            , 0xC1);  // VK_ABNT_C1
            AddKey("AbntC2"            , 0xC2);  // VK_ABNT_C2
            AddKey("WheelDown"         , 0xCE);  // VK_WHEEL_DOWN  -- RM added, not windows standard
            AddKey("WheelUp"           , 0xCF);  // VK_WHEEL_UP    -- RM added, not windows standard
            AddKey("Ax"                , 0xE1);  // VK_OEM_AX
            AddKey("IcoHlp"            , 0xE3);  // VK_ICO_HELP
            AddKey("Ico00"             , 0xE4);  // VK_ICO_00
            AddKey("Process"           , 0xE5);  // VK_PROCESSKEY
            AddKey("IcoClr"            , 0xE6);  // VK_ICO_CLEAR
            AddKey("Packet"            , 0xE7);  // VK_PACKET
            AddKey("Reset"             , 0xE9);  // VK_OEM_RESET
            AddKey("Jump"              , 0xEA);  // VK_OEM_JUMP
            AddKey("OemPa1"            , 0xEB);  // VK_OEM_PA1
            AddKey("OemPa2"            , 0xEC);  // VK_OEM_PA2
            AddKey("OemPa3"            , 0xED);  // VK_OEM_PA3
            AddKey("WsCtrl"            , 0xEE);  // VK_OEM_WSCTRL
            AddKey("CuSel"             , 0xEF);  // VK_OEM_CUSEL
            AddKey("OemAttn"           , 0xF0);  // VK_OEM_ATTN
            AddKey("Finish"            , 0xF1);  // VK_OEM_FINISH
            AddKey("Copy"              , 0xF2);  // VK_OEM_COPY
            AddKey("Auto"              , 0xF3);  // VK_OEM_AUTO
            AddKey("Enlw"              , 0xF4);  // VK_OEM_ENLW
            AddKey("BackTab"           , 0xF5);  // VK_OEM_BACKTAB
            AddKey("Attn"              , 0xF6);  // VK_ATTN
            AddKey("CrSel"             , 0xF7);  // VK_CRSEL
            AddKey("ExSel"             , 0xF8);  // VK_EXSEL
            AddKey("ErEof"             , 0xF9);  // VK_EREOF
            AddKey("Play"              , 0xFA);  // VK_PLAY
            AddKey("Zoom"              , 0xFB);  // VK_ZOOM
            AddKey("Pa1"               , 0xFD);  // VK_PA1
            AddKey("OemClr"            , 0xFE);  // VK_OEM_CLEAR
        }
        
    }
}
