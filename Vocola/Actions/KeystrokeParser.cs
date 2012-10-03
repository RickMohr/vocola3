using System;
using System.Collections.Generic;

namespace Vocola
{

    public class KeystrokeParser
    {

        static public List<Keystroke> Parse(string keys)
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
                            // No valid key name found -- add a "{" character
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

        static private Keystroke ParseKeystroke(string keyText)
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
                    keyText = keyText.Substring(0, 1);
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
            bool valid = false;
            if (keyName.Length == 1)
            {
                // It's a single character
                keystroke.Char = keyName[0];
                valid = true;
            }

            keyName = keyName.ToUpper();
            if (Keystroke.IsVocolaKey(keyName))
            {
                // It's a Vocola key name
                Keystroke vocolaKeystroke = Keystroke.Get(keyName);
                keystroke.VocolaName = vocolaKeystroke.VocolaName;
                keystroke.VirtualKeyCode = vocolaKeystroke.VirtualKeyCode;
                keystroke.KeyNameForSendKeys = vocolaKeystroke.KeyNameForSendKeys;
                if (keyName == "SPACE")
                    // It's a space (probably something like {Ctrl+Space})
                    keystroke.Char = ' ';
                valid = true;
            }
            else if (keyName.StartsWith("U+"))
            {
                // It's a unicode hex character, e.g. {U+2014} for Em Dash
                keystroke.Char = (char)UInt16.Parse(keyName.Substring(2),
                                                    System.Globalization.NumberStyles.AllowHexSpecifier);
                valid = true;
            }

            if (valid)
                return keystroke;
            else
                return null;
        }

    }

}

