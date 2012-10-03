using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Vocola
{

    public abstract class KeystrokeSender
    {
        private List<Keystroke> Keystrokes;

        public KeystrokeSender()
        {
            Keystroke.Initialize();
        }

        // ---------------------------------------------------------------------
        // Entry points

        public void SendSystemKeys(string keys)
        {
            Keystrokes = KeystrokeParser.Parse(keys);
            SendSystemKeys();
        }

        public void SendKeys(string keys)
        {
            Keystrokes = KeystrokeParser.Parse(keys);
            SendKeys();
        }

        public void SendText(string text)
        {
            Keystrokes = new List<Keystroke>();
            foreach (Char c in text)
                Keystrokes.Add(new Keystroke(c));
            SendKeys();
        }

        public void SendTextUsingSeparateThread(string text)
        {
            // Launch thread to send text, hopefully avoiding duplicated keystrokes bug
            Thread thread = new Thread(new ParameterizedThreadStart(SendText));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "Send Keys Thread";
            thread.Start(text);
        }

        private void SendText(object text)
        {
            SendText((string)text);
        }

        // ---------------------------------------------------------------------
        // Send Keys

        private void SendKeys()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Keystroke keystroke in Keystrokes)
            {
                if ((keystroke.Char == (Char)0 && keystroke.KeyNameForSendKeys == null)
                    || keystroke.Windows      // not supported by SendKeys
                    || !keystroke.Down || !keystroke.Up)  // holding or releasing a key
                {
                    // Can't use SendKeys for this keystroke.
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

        // Subclass responsibility

        protected virtual void AppendKeystrokeForSendKeys(Keystroke k, StringBuilder sb) { }
        protected virtual void ReallySendKeys(string keys) { }

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
            foreach (Keystroke keystroke in Keystrokes)
                SendSystemKeys(keystroke);
        }

        static private void SendSystemKeys(Keystroke keystroke)
        {
            byte vk = keystroke.VirtualKeyCode;
            if (vk == 0)
                // This should only happen for SendSystemKeys API calls
                throw new ActionException(null, "Invalid key for SendSystemKeys: '{0}'", keystroke.Char);
            Trace.WriteLine(LogLevel.Low, "    Send system keys: '{0}'", keystroke);

            if (Char.IsUpper(keystroke.Char))
                keystroke.Shift = true;

            if (keystroke.Shift)     Win.SendKeyEvent(VK_SHIFT,   true);
            if (keystroke.Control)   Win.SendKeyEvent(VK_CONTROL, true);
            if (keystroke.Alternate) Win.SendKeyEvent(VK_MENU,    true);
            if (keystroke.Windows)   Win.SendKeyEvent(VK_LWIN,    true);

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
                        Win.SendKeyEvent(vk, true);
                    if (keystroke.Alternate && vk == 0x09) // {Alt+Tab}
                        Thread.Sleep(100); // Otherwise unreliable
                    if (keystroke.Up)
                        Win.SendKeyEvent(vk, false);
                }
            }

            if (keystroke.Windows)   Win.SendKeyEvent(VK_LWIN,    false);
            if (keystroke.Alternate) Win.SendKeyEvent(VK_MENU,    false);
            if (keystroke.Control)   Win.SendKeyEvent(VK_CONTROL, false);
            if (keystroke.Shift)     Win.SendKeyEvent(VK_SHIFT,   false);
        }

    }

    // ------------------------------------------------------------------------
    // Subclass to send keystrokes using System.Windows.Forms.SendKeys

    public class KeystrokeSenderWinForms : KeystrokeSender
    {
        static private string CharactersNeedingEscape = "{}()[]+^%~\n";

        protected override void AppendKeystrokeForSendKeys(Keystroke k, StringBuilder sb)
        {
            if (k.Count == 0)
                return;
            if (k.Char == '+')
            {
                // Work around SendKeys glitch. To send '+' we must escape it, as '{+}', 
                // but SendKeys sends that as numeric keypad "+", which toggles the Dragon mic!
                // So we send "Shift =" instead. Will that fail on non-English systems?
                k.Char = '=';
                k.Shift = true;
            }
            if (k.Shift)
                sb.Append('+');
            if (k.Control)
                sb.Append('^');
            if (k.Alternate)
                sb.Append('%');
            bool useBraces = ((k.Count > 1 && k.Char != ' ')
                              || k.KeyNameForSendKeys != null
                              || CharactersNeedingEscape.IndexOf(k.Char) >= 0);
            if (useBraces)
                sb.Append('{');
            if (k.KeyNameForSendKeys != null)
                sb.Append(k.KeyNameForSendKeys);
            else if (k.Char == '\n')
                sb.Append("ENTER");
            else
                sb.Append(k.Char);
            if (k.Count > 1)
                if (k.Char == ' ')
                    // SendKeys doesn't seem to support multiple spaces ("{  4}" fails)
                    sb.Append("".PadLeft((int)k.Count - 1));
                else
                    sb.Append(String.Format(" {0}", k.Count));
            if (useBraces)
                sb.Append('}');
        }

        protected override void ReallySendKeys(string keys)
        {
            Trace.WriteLine(LogLevel.Low, "    Keystrokes: '{0}'", keys);
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

    }

    // ------------------------------------------------------------------------
    // Subclass to send keystrokes using NatSpeak SendDragonKeys

    public class KeystrokeSenderDragon : KeystrokeSender
    {

        protected override void AppendKeystrokeForSendKeys(Keystroke k, StringBuilder sb)
        {
            if (k.Count == 0)
                return;
            bool useBraces = ((k.Count > 1 && k.Char != ' ')
                              || k.KeyNameForSendKeys != null
                              || k.HasModifierKey());
            if (useBraces)
                sb.Append('{');
            if (k.Control)
                sb.Append("Ctrl+");
            if (k.Alternate)
                sb.Append("Alt+");
            if (k.Shift)
                sb.Append("Shift+");
            if (k.KeyNameForSendKeys != null)
                sb.Append(k.KeyNameForSendKeys);
            else if (k.Char == '\n')
                sb.Append("ENTER");
            else
                sb.Append(k.Char);
            if (k.Count > 1)
                if (k.Char == ' ')
                    // SendKeys doesn't seem to support multiple spaces ("{  4}" fails)
                    sb.Append("".PadLeft((int)k.Count - 1));
                else
                    sb.Append(String.Format(" {0}", k.Count));
            if (useBraces)
                sb.Append('}');
        }

        protected override void ReallySendKeys(string keys)
        {
            Trace.WriteLine(LogLevel.Low, "    Keystrokes: '{0}'", keys);
            bool success = NatLinkToVocolaServer.CurrentNatLinkCallbackHandler.SendKeys(keys);
            if (!success)
                Trace.WriteLine(LogLevel.Error, "Failed to send keystrokes via SendDragonKeys");
        }

    }

}

