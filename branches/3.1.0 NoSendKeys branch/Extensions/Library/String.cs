using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Vocola;

namespace Library
{

    /// <summary>Functions for manipulating text strings.</summary>
    public class String : VocolaExtension
    {

        /// <summary>Returns a specified number of characters from the start of a string.</summary>
        /// <param name="s">String to operate on.</param>
        /// <param name="count">Number of characters to return.</param>
        /// <returns>The first <paramref name="count"/> characters of <paramref name="s"/>.</returns>
        /// <remarks>If <paramref name="count"/> is greater than the length of <paramref name="s"/>, <paramref name="s"/>
        /// is returned.</remarks>
        [VocolaFunction]
        static public string Left(string s, int count)
        {
            if (count < 0)
                throw new VocolaExtensionException("'count' argument may not be negative");
            else if (count > s.Length)
                return s;
            else
                return s.Substring(0, count);
        }

        /// <summary>Returns a specified number of characters from the end of a string.</summary>
        /// <param name="s">String to operate on.</param>
        /// <param name="count">Number of characters to return.</param>
        /// <returns>The last <paramref name="count"/> characters of <paramref name="s"/>.</returns>
        /// <remarks>If <paramref name="count"/> is greater than the length of <paramref name="s"/>, <paramref name="s"/>
        /// is returned.</remarks>
        [VocolaFunction]
        static public string Right(string s, int count)
        {
            if (count < 0)
                throw new VocolaExtensionException("'count' argument may not be negative");
            else if (count > s.Length)
                return s;
            else
                return s.Substring(s.Length - count);
        }

        /// <summary>In a given string, replaces all occurrences of a specified string with another specified string.</summary>
        /// <param name="s">String to operate on.</param>
        /// <param name="oldValue">String to be replaced. Case sensitive.</param>
        /// <param name="newValue">String to replace all occurrences of <paramref name="oldValue"/>.</param>
        /// <returns><paramref name="string"/>, with instances of <paramref name="oldValue"/> replaced by <paramref
        /// name="newValue"/>.</returns>
        [VocolaFunction]
        static public string Replace(string s, string oldValue, string newValue)
        {
            if (oldValue.Length == 0)
                throw new VocolaExtensionException("'oldValue' argument may not be empty");
            return s.Replace(oldValue, newValue);
        }

        /// <summary>Splits a given string using a specified separator, then returns a specified element of the
        /// resulting substrings.</summary>
        /// <param name="s">String to operate on.</param>
        /// <param name="separator">A string to delimit substrings in <paramref name="s"/>. Case sensitive.</param>
        /// <param name="index">Index of desired substring in list of split substrings. 0 denotes the first substring, 1
        /// denotes the second substring, etc. -1 denotes the last substring, -2 denotes the second to last substring, etc.
        /// </param>
        /// <returns>The <paramref name="index"/>th substring after splitting <paramref name="s"/> using <paramref
        /// name="separator"/>.</returns>
        /// <remarks>
        /// <paramref name="separator"/> is not included in the returned string.
        /// <para>If <paramref name="separator"/> is an empty string, white space characters are assumed to be the
        /// delimiters.</para>
        /// <para>If two delimiters are adjacent or a delimiter is found at the beginning or end of <paramref
        /// name="s"/>, the substring list contains an empty element.</para> 
        /// </remarks>
        [VocolaFunction]
        static public string Split(string s, string separator, int index)
        {
            string[] parts = s.Split(new string[] {separator}, StringSplitOptions.None);
            if (index >= parts.Length || -index > parts.Length)
                throw new VocolaExtensionException("Split string has {0} elements; 'index' argument {1} out of range",
                                                   parts.Length, index);
            else if (index >= 0)
                return parts[index];
            else
                return parts[parts.Length + index];
        }

        // ---------------------------------------------------------------------
        // Capitalize	 

        /// <summary>Capitalizes each word in the specified text.</summary>
        /// <param name="text">Text to capitalize.</param>
        /// <returns>A version of <paramref name="text"/> with each word capitalized.</returns>
        /// <example><code title="Capitalize dictated text">
        /// Cap That = Dictation.Replace(String.Capitalize(Dictation.Get()));</code>
        /// This command uses three Vocola library functions to capitalize the just-dictated Vocola phrase:
        /// <see cref="Dictation.Get">Dictation.Get</see> retrieves the just-dictated phrase,
        /// <see cref="Capitalize"/> capitalizes it, and
        /// <see cref="Dictation.Replace">Dictation.Replace</see> replaces it.
        /// </example>
        [VocolaFunction]
        static public string Capitalize(string text)
        {
            return Regex.Replace(text, @"\w[\w\']*",
                delegate(Match m) 
                {
                    string s = m.ToString();
                    if (char.IsLower(s[0])) 
                        return char.ToUpper(s[0]) + s.Substring(1);
                    else
                        return s;
                });
        }

        // ---------------------------------------------------------------------
        // JoinWords	 

        /// <summary>Replaces whitespace characters in the input text with specified character(s).</summary>
        /// <param name="text">Text to be joined.</param>
        /// <param name="joiner">Character(s) to insert between words of <paramref name="Text"/>.</param>
        /// <returns>A version of <paramref name="text"/> with each space replaced by <paramref
        /// name="joiner"/>.</returns>
        /// <remarks>Leading and trailing whitespace is not affected.</remarks>
        /// <example><code title="Hyphenate dictated text">
        /// Compound That = Dictation.Replace(String.JoinWords(Dictation.Get(), "-"));</code>
        /// This command uses three Vocola library functions to hyphenate the just-dictated Vocola phrase:
        /// <see cref="Dictation.Get">Dictation.Get</see> retrieves the just-dictated phrase,
        /// <see cref="JoinWords"/> replaces spaces with hyphens, and
        /// <see cref="Dictation.Replace">Dictation.Replace</see> replaces the just-dictated phrase,
        /// </example>
        /// <example><code title="Remove spaces in dictated text">
        /// Compound That = Dictation.Replace(String.JoinWords(Dictation.Get(), ""));</code>
        /// Similarly, this command removes spaces from the just-dictated Vocola phrase;
        /// <see cref="JoinWords"/> replaces the spaces with nothing.
        /// </example>
        [VocolaFunction]
        static public string JoinWords(string text, string joiner)
        {
            string leadingWhitespace = "";
            Match match = (new Regex(@"^(\s+)")).Match(text);
            if (match.Success)
                leadingWhitespace = match.Groups[1].Value;

            string trailingWhitespace = "";
            match = (new Regex(@"(\s+)$")).Match(text);
            if (match.Success)
                trailingWhitespace = match.Groups[1].Value;

            text = Regex.Replace(text.Trim(), @"\s+", joiner);

            return System.String.Format("{0}{1}{2}", leadingWhitespace, text, trailingWhitespace);
        }

        // ---------------------------------------------------------------------
        // Length	 

        /// <summary>Returns the number of characters in the input text.</summary>
        /// <param name="text">Text whose length is desired.</param>
        /// <returns>The number of characters in <paramref name="text"/>.</returns>
        /// <example><code title="Move to beginning of dictated phrase">
        /// Phrase Start = {Left_ String.Length(Dictation.Get()) };</code>
        /// This command retrieves the length of the just-dictated phrase and uses the result to
        /// send that many "left arrow" keystrokes. For example, if "now is the time" was just dictated,
        /// <c>{Left_15}</c> will be sent.
        /// </example>
        [VocolaFunction]
        static public int Length(string text)
        {
            return text.Length;
        }

        // ---------------------------------------------------------------------
        // ToTitleCase

        /// <summary>Converts the specified text to title case, where all but small words are capitalized.</summary>
        /// <param name="text">Text to convert to title case.</param>
        /// <returns>A title case version of <paramref name="text"/>.</returns>
        /// <remarks>In titles of books, songs, chapters, etc., "small" words are not capitalized.
        /// Opinions differ even among prominent style manuals on exactly which words are considered "small";
        /// this function recognizes "a", "an", "and", "as", "at", "but", "by", "for", "from", "in", "into", "nor",
        /// "of", "on", "or", "over", "per", "the", "to", "upon", "vs.", and "with" as small words.</remarks>
        /// <example><code title="Convert spoken phrase to title case">
        /// Title Case That = Dictation.Replace(String.ToTitleCase(Dictation.Get()));</code>
        /// This command uses three Vocola library functions to convert the just-dictated Vocola phrase to title case:
        /// <see cref="Dictation.Get">Dictation.Get</see> retrieves the just-dictated phrase,
        /// <see cref="ToTitleCase"/> converts it to title case, and
        /// <see cref="Dictation.Replace">Dictation.Replace</see> replaces it.
        /// For example, "the best of the best" would be converted to "The Best of the Best".
        /// </example>
        [VocolaFunction]
        static public string ToTitleCase(string text)
        {
            bool isFirstWord = true;
            return Regex.Replace(text, @"\w[\w\']*",
                delegate(Match m) 
                {
                    string s = m.ToString();
                    if (char.IsLower(s[0]) && (isFirstWord || !LowerCaseWords.ContainsKey(s)))
                        s = char.ToUpper(s[0]) + s.Substring(1);
                    isFirstWord = false;
                    return s;
                });
        }

        static private Dictionary<string, bool> lowerCaseWords = null;
        static private Dictionary<string, bool> LowerCaseWords
        {
            get
            {
                if (lowerCaseWords == null)
                {
                    string[] words = new string[] {
                        "a", "an", "and", "as", "at", "but", "by", "for", "from", "in", "into",
                        "nor", "of", "on", "or", "over", "per", "the", "to", "upon", "vs.", "with"
                        // http://www.analphilosopher.com/posts/1090606048.shtml
                    };
                    lowerCaseWords = new Dictionary<string, bool>();
                    foreach (string word in words)
                        lowerCaseWords[word] = true;
                }
                return lowerCaseWords;
            }
        }

        // ---------------------------------------------------------------------
        // ToCamelCaseWord	 
        // ToTitleCaseWord	 

        /// <summary>Converts the specified text to a single "camel case" word.</summary>
        /// <param name="text">Text to convert to a camel case word.</param>
        /// <returns>A single-word camel case version of <paramref name="text"/>.</returns>
        /// <remarks>Programmers use camel case for multi-word variable names. For example, "number of items"
        /// converted to camel case would be "numberOfItems". It's called "camel case" because the resulting
        /// names are shaped something like a camel.</remarks>
        /// <example><code title="Camel-case previous words">
        /// Camel 2..9 = {Ctrl+Shift+Left_$1}{Ctrl+c} String.ToCamelCaseWord(Clipboard.GetText());</code>
        /// This command converts the specified number of words before the insertion point to camel case.
        /// <c>{Ctrl+Shift+Left_$1}</c> selects 2 or more words, <c>{Ctrl+c}</c> copies them,
        /// <c>Clipboard.GetText()</c> retrieves them, and <c>String.ToCamelCaseWord()</c> converts them.
        /// </example>
        [VocolaFunction]
        static public string ToCamelCaseWord(string text)
        {
            return ToggleInitialCase(ToTitleCaseWord(text));
        }

        /// <summary>Converts the specified text to a single "title case" word.</summary>
        /// <param name="text">Text to convert to a title case word.</param>
        /// <returns>A single-word title case version of <paramref name="text"/>.</returns>
        /// <remarks>Programmers use title case for multi-word variable names. For example, "number of items"
        /// converted to title case would be "NumberOfItems".</remarks>
        /// <example><code title="Title-case previous words">
        /// Title 2..9 = {Ctrl+Shift+Left_$1}{Ctrl+c} String.ToTitleCaseWord(Clipboard.GetText());</code>
        /// This command converts the specified number of words before the insertion point to title case.
        /// <c>{Ctrl+Shift+Left_$1}</c> selects 2 or more words, <c>{Ctrl+c}</c> copies them,
        /// <c>Clipboard.GetText()</c> retrieves them, and <c>String.ToTitleCaseWord()</c> converts them.
        /// </example>
        [VocolaFunction]
        static public string ToTitleCaseWord(string text)
        {
            return JoinWords(Capitalize(text), "");
        }

        // ---------------------------------------------------------------------
        // ToLower	 
        // ToUpper	 

        /// <summary>Converts the specified text to lower case.</summary>
        /// <param name="text">Text to convert to lower case.</param>
        /// <returns>A lower case version of <paramref name="text"/>.</returns>
        /// <example><code title="Convert dictated text to lower case">
        /// No Caps That = Dictation.Replace(String.ToLower(Dictation.Get()));</code>
        /// This command uses three Vocola library functions to convert the just-dictated Vocola phrase to lower case:
        /// <see cref="Dictation.Get">Dictation.Get</see> retrieves the just-dictated phrase,
        /// <see cref="ToLower"/> converts it to title case, and
        /// <see cref="Dictation.Replace">Dictation.Replace</see> replaces it.
        /// </example>
        [VocolaFunction]
        static public string ToLower(string text)
        {
            return text.ToLower();
        }

        /// <summary>Converts the specified text to upper case.</summary>
        /// <param name="text">Text to convert to upper case.</param>
        /// <returns>An upper case version of <paramref name="text"/>.</returns>
        /// <example><code title="Convert dictated text to upper case">
        /// All Caps That = Dictation.Replace(String.ToUpper(Dictation.Get()));</code>
        /// This command uses three Vocola library functions to convert the just-dictated Vocola phrase to upper case:
        /// <see cref="Dictation.Get">Dictation.Get</see> retrieves the just-dictated phrase,
        /// <see cref="ToUpper"/> converts it to upper case, and
        /// <see cref="Dictation.Replace">Dictation.Replace</see> replaces it.
        /// </example>
        [VocolaFunction]
        static public string ToUpper(string text)
        {
            return text.ToUpper();
        }

        // ---------------------------------------------------------------------
        // ToggleInitialCase	 

        /// <summary>Toggles the case of the initial letter of the specified text.</summary>
        /// <param name="text">Text to modify.</param>
        /// <returns><paramref name="text"/>, with the case of the initial letter toggled.</returns>
        /// <remarks>If <paramref name="text"/> begins with a lower-case letter this function converts it to upper case.
        /// If <paramref name="text"/> begins with an upper-case letter this function converts it to lower case.</remarks>
        /// <example><code title="Toggle initial case of dictated text">
        /// Fix Case = Dictation.Replace(String.ToggleInitialCase(Dictation.Get()));</code>
        /// Because Vocola has no knowledge of a document's text, the initial case of a dictated phrase
        /// is sometimes incorrect. This command toggles the case, using three Vocola library functions:
        /// <see cref="Dictation.Get">Dictation.Get</see> retrieves the just-dictated phrase,
        /// <see cref="ToggleInitialCase"/> converts the case of the initial letter, and
        /// <see cref="Dictation.Replace">Dictation.Replace</see> replaces the phrase.
        /// </example>
        [VocolaFunction]
        static public string ToggleInitialCase(string text)
        {
            return new Regex(@"\w").Replace(text,
                delegate(Match m) 
                {
                    string s = m.ToString();
                    if (Char.IsLower(s[0]))
                        return s.ToUpper();
                    else if (Char.IsUpper(s[0]))
                        return s.ToLower();
                    else
                        return s;
                }, 1 /*only replace 1st occurrence*/);
        }

        // ---------------------------------------------------------------------
        // ToggleInitialSpace

        /// <summary>Adds or removes a space at the beginning of the specified text.</summary>
        /// <param name="text">Text to modify.</param>
        /// <returns><paramref name="text"/>, with an initial space added or removed.</returns>
        /// <remarks>If <paramref name="text"/> begins with a space character this function removes it.
        /// If <paramref name="text"/> does not begin with a space character this function adds one.</remarks>
        /// <example><code title="Toggle initial space in dictated text">
        /// Fix Space = Dictation.Replace(String.ToggleInitialSpace(Dictation.Get()));</code>
        /// Because Vocola has no knowledge of a document's text, a dictated phrase may have an extra initial space
        /// or lack a desired initial space. This command toggles the initial space, using three Vocola library functions:
        /// <see cref="Dictation.Get">Dictation.Get</see> retrieves the just-dictated phrase,
        /// <see cref="ToggleInitialCase"/> adds or removes an initial space, and
        /// <see cref="Dictation.Replace">Dictation.Replace</see> replaces the phrase.
        /// </example>
        [VocolaFunction]
        static public string ToggleInitialSpace(string text)
        {
            if (text.StartsWith(" "))
                return text.Substring(1);
            else
                return " " + text;
        }
        
    }

}
