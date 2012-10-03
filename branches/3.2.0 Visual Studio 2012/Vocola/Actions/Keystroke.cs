using System;
using System.Collections.Generic;

namespace Vocola
{

    public class Keystroke
    {
        // Char or VirtualKeyCode always set, and both may be set
        // If VirtualKeyCode is set, KeyNameForSendKeys may also be set

        public string KeyText = null;
        public string VocolaName = null;
        public byte VirtualKeyCode = 0;
        public string KeyNameForSendKeys = null;
        public char Char = (Char)0;

        public bool Shift = false;
        public bool Control = false;
        public bool Alternate = false;
        public bool Windows = false;
        public uint Count = 1;

        public bool Down = true;
        public bool Up = true;

        public Keystroke(string keyText)
        {
            KeyText = keyText;
        }

        public Keystroke(char key)
        {
            this.Char = key;
        }

        public Keystroke(string vocolaName, byte virtualKeyCode, string keyNameForSendKeys)
        {
            VocolaName = vocolaName;
            VirtualKeyCode = virtualKeyCode;
            KeyNameForSendKeys = keyNameForSendKeys;
        }

        public override string ToString()
        {
            if (KeyText != null)
                return "{" + KeyText + "}";
            else
                return Char.ToString();
        }

        public bool HasModifierKey()
        {
            return Shift | Control | Alternate | Windows;
        }

        // ---------------------------------------------------------------------
        // Key data

        static private Dictionary<string, Keystroke> KeyData;

        static private void AddKey(string vocolaName, string keyNameForSendKeys, byte virtualKeyCode)
        {
            KeyData[vocolaName.ToUpper()] = new Keystroke(vocolaName, virtualKeyCode, keyNameForSendKeys);
        }

        static public bool IsVocolaKey(string vocolaName)
        {
            return KeyData.ContainsKey(vocolaName);
        }

        static public Keystroke Get(string vocolaName)
        {
            return KeyData[vocolaName];
        }

        static public void Initialize()
        {
            KeyData = new Dictionary<string, Keystroke>();
            //      Vocola Name        SendKeys name   Windows virtual key code
            AddKey("LeftButton"        , null        , 0x01);  // VK_LBUTTON
            AddKey("RightButton"       , null        , 0x02);  // VK_RBUTTON
            AddKey("Break"             , "Break"     , 0x03);  // VK_CANCEL
            AddKey("MiddleButton"      , null        , 0x04);  // VK_MBUTTON
            AddKey("XButton1"          , null        , 0x05);  // VK_XBUTTON1
            AddKey("XButton2"          , null        , 0x06);  // VK_XBUTTON2
            AddKey("Backspace"         , "BS"        , 0x08);  // VK_BACK
            AddKey("Tab"               , "Tab"       , 0x09);  // VK_TAB
            AddKey("Clear"             , null        , 0x0C);  // VK_CLEAR
            AddKey("Enter"             , "Enter"     , 0x0D);  // VK_RETURN
            AddKey("Pause"             , null        , 0x13);  // VK_PAUSE
            AddKey("CapsLock"          , "CapsLock"  , 0x14);  // VK_CAPITAL
            AddKey("Kana"              , null        , 0x15);  // VK_KANA
            AddKey("Junja"             , null        , 0x17);  // VK_JUNJA
            AddKey("Final"             , null        , 0x18);  // VK_FINAL
            AddKey("Kanji"             , null        , 0x19);  // VK_KANJI
            AddKey("Escape"            , "Esc"       , 0x1B);  // VK_ESCAPE
            AddKey("Esc"               , "Esc"       , 0x1B);
            AddKey("Convert"           , null        , 0x1C);  // VK_CONVERT
            AddKey("NonConvert"        , null        , 0x1D);  // VK_NONCONVERT
            AddKey("Accept"            , null        , 0x1E);  // VK_ACCEPT
            AddKey("ModeChange"        , null        , 0x1F);  // VK_MODECHANGE
            AddKey("Space"             , null        , 0x20);  // VK_SPACE
            AddKey("PageUp"            , "PgUp"      , 0x21);  // VK_PRIOR
            AddKey("PgUp"              , "PgUp"      , 0x21);
            AddKey("ExtPgUp"           , "PgUp"      , 0x21);
            AddKey("PageDown"          , "PgDn"      , 0x22);  // VK_NEXT
            AddKey("ExtPgDn"           , "PgDn"      , 0x22);
            AddKey("PgDn"              , "PgDn"      , 0x22);
            AddKey("End"               , "End"       , 0x23);  // VK_END
            AddKey("ExtEnd"            , "End"       , 0x23);
            AddKey("Home"              , "Home"      , 0x24);  // VK_HOME
            AddKey("ExtHome"           , "Home"      , 0x24);
            AddKey("Left"              , "Left"      , 0x25);  // VK_LEFT
            AddKey("ExtLeft"           , "Left"      , 0x25);
            AddKey("Up"                , "Up"        , 0x26);  // VK_UP
            AddKey("ExtUp"             , "Up"        , 0x26);
            AddKey("Right"             , "Right"     , 0x27);  // VK_RIGHT
            AddKey("ExtRight"          , "Right"     , 0x27);
            AddKey("Down"              , "Down"      , 0x28);  // VK_DOWN
            AddKey("ExtDown"           , "Down"      , 0x28);
            AddKey("Select"            , null        , 0x29);  // VK_SELECT
            AddKey("Print"             , null        , 0x2A);  // VK_PRINT
            AddKey("Execute"           , null        , 0x2B);  // VK_EXECUTE
            AddKey("PrintScreen"       , null        , 0x2C);  // VK_SNAPSHOT
            AddKey("PrtSc"             , null        , 0x2C);
            AddKey("Insert"            , "Ins"       , 0x2D);  // VK_INSERT
            AddKey("Ins"               , "Ins"       , 0x2D);
            AddKey("ExtIns"            , "Ins"       , 0x2D);
            AddKey("Delete"            , "Del"       , 0x2E);  // VK_DELETE
            AddKey("Del"               , "Del"       , 0x2E);
            AddKey("ExtDel"            , "Del"       , 0x2E);
            AddKey("Help"              , null        , 0x2F);  // VK_HELP
            AddKey("0"                 , null        , 0x30);  // VK_KEY_0
            AddKey("1"                 , null        , 0x31);  // VK_KEY_1
            AddKey("2"                 , null        , 0x32);  // VK_KEY_2
            AddKey("3"                 , null        , 0x33);  // VK_KEY_3
            AddKey("4"                 , null        , 0x34);  // VK_KEY_4
            AddKey("5"                 , null        , 0x35);  // VK_KEY_5
            AddKey("6"                 , null        , 0x36);  // VK_KEY_6
            AddKey("7"                 , null        , 0x37);  // VK_KEY_7
            AddKey("8"                 , null        , 0x38);  // VK_KEY_8
            AddKey("9"                 , null        , 0x39);  // VK_KEY_9
            AddKey("A"                 , null        , 0x41);  // VK_KEY_A
            AddKey("B"                 , null        , 0x42);  // VK_KEY_B
            AddKey("C"                 , null        , 0x43);  // VK_KEY_C
            AddKey("D"                 , null        , 0x44);  // VK_KEY_D
            AddKey("E"                 , null        , 0x45);  // VK_KEY_E
            AddKey("F"                 , null        , 0x46);  // VK_KEY_F
            AddKey("G"                 , null        , 0x47);  // VK_KEY_G
            AddKey("H"                 , null        , 0x48);  // VK_KEY_H
            AddKey("I"                 , null        , 0x49);  // VK_KEY_I
            AddKey("J"                 , null        , 0x4A);  // VK_KEY_J
            AddKey("K"                 , null        , 0x4B);  // VK_KEY_K
            AddKey("L"                 , null        , 0x4C);  // VK_KEY_L
            AddKey("M"                 , null        , 0x4D);  // VK_KEY_M
            AddKey("N"                 , null        , 0x4E);  // VK_KEY_N
            AddKey("O"                 , null        , 0x4F);  // VK_KEY_O
            AddKey("P"                 , null        , 0x50);  // VK_KEY_P
            AddKey("Q"                 , null        , 0x51);  // VK_KEY_Q
            AddKey("R"                 , null        , 0x52);  // VK_KEY_R
            AddKey("S"                 , null        , 0x53);  // VK_KEY_S
            AddKey("T"                 , null        , 0x54);  // VK_KEY_T
            AddKey("U"                 , null        , 0x55);  // VK_KEY_U
            AddKey("V"                 , null        , 0x56);  // VK_KEY_V
            AddKey("W"                 , null        , 0x57);  // VK_KEY_W
            AddKey("X"                 , null        , 0x58);  // VK_KEY_X
            AddKey("Y"                 , null        , 0x59);  // VK_KEY_Y
            AddKey("Z"                 , null        , 0x5A);  // VK_KEY_Z
            AddKey("Win"               , null        , 0x5B);  // VK_LWIN
            AddKey("LeftWin"           , null        , 0x5B);
            AddKey("RightWin"          , null        , 0x5C);  // VK_RWIN
            AddKey("ContextMenu"       , null        , 0x5D);  // VK_APPS
            AddKey("Sleep"             , null        , 0x5F);  // VK_SLEEP
            AddKey("NumKey0"           , null        , 0x60);  // VK_NUMPAD0
            AddKey("NumKey1"           , null        , 0x61);  // VK_NUMPAD1
            AddKey("NumKey2"           , null        , 0x62);  // VK_NUMPAD2
            AddKey("NumKey3"           , null        , 0x63);  // VK_NUMPAD3
            AddKey("NumKey4"           , null        , 0x64);  // VK_NUMPAD4
            AddKey("NumKey5"           , null        , 0x65);  // VK_NUMPAD5
            AddKey("NumKey6"           , null        , 0x66);  // VK_NUMPAD6
            AddKey("NumKey7"           , null        , 0x67);  // VK_NUMPAD7
            AddKey("NumKey8"           , null        , 0x68);  // VK_NUMPAD8
            AddKey("NumKey9"           , null        , 0x69);  // VK_NUMPAD9
            AddKey("NumKey*"           , "Multiply"  , 0x6A);  // VK_MULTIPLY
            AddKey("Mult"              , "Multiply"  , 0x6A);
            AddKey("Asterisk"          , "Multiply"  , 0x6A);
            AddKey("NumKey+"           , "Add"       , 0x6B);  // VK_ADD
            AddKey("Plus"              , "Add"       , 0x6B);
            AddKey("NumKeyEnter"       , null        , 0x6C);  // VK_SEPARATOR
            AddKey("KeyPadEnter"       , null        , 0x6C);
            AddKey("NumKey-"           , "Subtract"  , 0x6D);  // VK_SUBTRACT
            AddKey("Minus"             , "Subtract"  , 0x6D);
            AddKey("NumKey."           , null        , 0x6E);  // VK_DECIMAL
            AddKey("NumKey/"           , "Divide"    , 0x6F);  // VK_DIVIDE
            AddKey("Slash"             , "Divide"    , 0x6F);
            AddKey("F1"                , "F1"        , 0x70);  // VK_F1
            AddKey("F2"                , "F2"        , 0x71);  // VK_F2
            AddKey("F3"                , "F3"        , 0x72);  // VK_F3
            AddKey("F4"                , "F4"        , 0x73);  // VK_F4
            AddKey("F5"                , "F5"        , 0x74);  // VK_F5
            AddKey("F6"                , "F6"        , 0x75);  // VK_F6
            AddKey("F7"                , "F7"        , 0x76);  // VK_F7
            AddKey("F8"                , "F8"        , 0x77);  // VK_F8
            AddKey("F9"                , "F9"        , 0x78);  // VK_F9
            AddKey("F10"               , "F10"       , 0x79);  // VK_F10
            AddKey("F11"               , "F11"       , 0x7A);  // VK_F11
            AddKey("F12"               , "F12"       , 0x7B);  // VK_F12
            AddKey("F13"               , null        , 0x7C);  // VK_F13
            AddKey("F14"               , null        , 0x7D);  // VK_F14
            AddKey("F15"               , null        , 0x7E);  // VK_F15
            AddKey("F16"               , null        , 0x7F);  // VK_F16
            AddKey("F17"               , null        , 0x80);  // VK_F17
            AddKey("F18"               , null        , 0x81);  // VK_F18
            AddKey("F19"               , null        , 0x82);  // VK_F19
            AddKey("F20"               , null        , 0x83);  // VK_F20
            AddKey("F21"               , null        , 0x84);  // VK_F21
            AddKey("F22"               , null        , 0x85);  // VK_F22
            AddKey("F23"               , null        , 0x86);  // VK_F23
            AddKey("F24"               , null        , 0x87);  // VK_F24
            AddKey("NumLock"           , "NumLock"   , 0x90);  // VK_NUMLOCK
            AddKey("ScrollLock"        , "ScrollLock", 0x91);  // VK_SCROLL
            AddKey("Jisho"             , null        , 0x92);  // VK_OEM_FJ_JISHO
            AddKey("Mashu"             , null        , 0x93);  // VK_OEM_FJ_MASSHOU
            AddKey("Touroku"           , null        , 0x94);  // VK_OEM_FJ_TOUROKU
            AddKey("Loya"              , null        , 0x95);  // VK_OEM_FJ_LOYA
            AddKey("Roya"              , null        , 0x96);  // VK_OEM_FJ_ROYA
            AddKey("Shift"             , null        , 0xA0);  // VK_LSHIFT
            AddKey("LeftShift"         , null        , 0xA0);  // VK_LSHIFT
            AddKey("RightShift"        , null        , 0xA1);  // VK_RSHIFT
            AddKey("Ctrl"              , null        , 0xA2);  // VK_LCONTROL
            AddKey("Control"           , null        , 0xA2);  // VK_LCONTROL
            AddKey("LeftCtrl"          , null        , 0xA2);  // VK_LCONTROL
            AddKey("RightCtrl"         , null        , 0xA3);  // VK_RCONTROL
            AddKey("Alt"               , null        , 0xA4);  // VK_LMENU
            AddKey("Alternate"         , null        , 0xA4);  // VK_LMENU
            AddKey("LeftAlt"           , null        , 0xA4);  // VK_LMENU
            AddKey("RightAlt"          , null        , 0xA5);  // VK_RMENU
            AddKey("BrowserBack"       , null        , 0xA6);  // VK_BROWSER_BACK
            AddKey("BrowserForward"    , null        , 0xA7);  // VK_BROWSER_FORWARD
            AddKey("BrowserRefresh"    , null        , 0xA8);  // VK_BROWSER_REFRESH
            AddKey("BrowserStop"       , null        , 0xA9);  // VK_BROWSER_STOP
            AddKey("BrowserSearch"     , null        , 0xAA);  // VK_BROWSER_SEARCH
            AddKey("BrowserFavorites"  , null        , 0xAB);  // VK_BROWSER_FAVORITES
            AddKey("BrowserHome"       , null        , 0xAC);  // VK_BROWSER_HOME
            AddKey("VolumeMute"        , null        , 0xAD);  // VK_VOLUME_MUTE
            AddKey("VolumeDown"        , null        , 0xAE);  // VK_VOLUME_DOWN
            AddKey("VolumeUp"          , null        , 0xAF);  // VK_VOLUME_UP
            AddKey("MediaNextTrack"    , null        , 0xB0);  // VK_MEDIA_NEXT_TRACK
            AddKey("MediaPreviousTrack", null        , 0xB1);  // VK_MEDIA_PREV_TRACK
            AddKey("MediaStop"         , null        , 0xB2);  // VK_MEDIA_STOP
            AddKey("MediaPlayPause"    , null        , 0xB3);  // VK_MEDIA_PLAY_PAUSE
            AddKey("Mail"              , null        , 0xB4);  // VK_LAUNCH_MAIL
            AddKey("Media"             , null        , 0xB5);  // VK_LAUNCH_MEDIA_SELECT
            AddKey("App1"              , null        , 0xB6);  // VK_LAUNCH_APP1
            AddKey("App2"              , null        , 0xB7);  // VK_LAUNCH_APP2
            AddKey(";"                 , null        , 0xBA);  // VK_OEM_1
            AddKey("="                 , null        , 0xBB);  // VK_OEM_PLUS
            AddKey(","                 , null        , 0xBC);  // VK_OEM_COMMA
            AddKey("-"                 , null        , 0xBD);  // VK_OEM_MINUS
            AddKey("."                 , null        , 0xBE);  // VK_OEM_PERIOD
            AddKey("/"                 , null        , 0xBF);  // VK_OEM_2
            AddKey("`"                 , null        , 0xC0);  // VK_OEM_3
            AddKey("AbntC1"            , null        , 0xC1);  // VK_ABNT_C1
            AddKey("AbntC2"            , null        , 0xC2);  // VK_ABNT_C2
            AddKey("WheelDown"         , null        , 0xCE);  // VK_WHEEL_DOWN  -- RM added, not windows standard
            AddKey("WheelUp"           , null        , 0xCF);  // VK_WHEEL_UP    -- RM added, not windows standard
            AddKey("["                 , null        , 0xDB);  // VK_OEM_4
            AddKey("\\"                , null        , 0xDC);  // VK_OEM_5
            AddKey("]"                 , null        , 0xDD);  // VK_OEM_6
            AddKey("'"                 , null        , 0xDE);  // VK_OEM_7
            AddKey("!"                 , null        , 0xDF);  // VK_OEM_8
            AddKey("Ax"                , null        , 0xE1);  // VK_OEM_AX
            AddKey("<"                 , null        , 0xE2);  // VK_OEM_102
            AddKey("IcoHlp"            , null        , 0xE3);  // VK_ICO_HELP
            AddKey("Ico00"             , null        , 0xE4);  // VK_ICO_00
            AddKey("Process"           , null        , 0xE5);  // VK_PROCESSKEY
            AddKey("IcoClr"            , null        , 0xE6);  // VK_ICO_CLEAR
            AddKey("Packet"            , null        , 0xE7);  // VK_PACKET
            AddKey("Reset"             , null        , 0xE9);  // VK_OEM_RESET
            AddKey("Jump"              , null        , 0xEA);  // VK_OEM_JUMP
            AddKey("OemPa1"            , null        , 0xEB);  // VK_OEM_PA1
            AddKey("OemPa2"            , null        , 0xEC);  // VK_OEM_PA2
            AddKey("OemPa3"            , null        , 0xED);  // VK_OEM_PA3
            AddKey("WsCtrl"            , null        , 0xEE);  // VK_OEM_WSCTRL
            AddKey("CuSel"             , null        , 0xEF);  // VK_OEM_CUSEL
            AddKey("OemAttn"           , null        , 0xF0);  // VK_OEM_ATTN
            AddKey("Finish"            , null        , 0xF1);  // VK_OEM_FINISH
            AddKey("Copy"              , null        , 0xF2);  // VK_OEM_COPY
            AddKey("Auto"              , null        , 0xF3);  // VK_OEM_AUTO
            AddKey("Enlw"              , null        , 0xF4);  // VK_OEM_ENLW
            AddKey("BackTab"           , null        , 0xF5);  // VK_OEM_BACKTAB
            AddKey("Attn"              , null        , 0xF6);  // VK_ATTN
            AddKey("CrSel"             , null        , 0xF7);  // VK_CRSEL
            AddKey("ExSel"             , null        , 0xF8);  // VK_EXSEL
            AddKey("ErEof"             , null        , 0xF9);  // VK_EREOF
            AddKey("Play"              , null        , 0xFA);  // VK_PLAY
            AddKey("Zoom"              , null        , 0xFB);  // VK_ZOOM
            AddKey("Pa1"               , null        , 0xFD);  // VK_PA1
            AddKey("OemClr"            , null        , 0xFE);  // VK_OEM_CLEAR
        }

    }

}
