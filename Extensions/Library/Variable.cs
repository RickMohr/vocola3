using System;
using System.Collections.Generic;
using Vocola;

namespace Library
{

    /// <summary>Functions for accessing Vocola variables.</summary>
    /// <remarks>Vocola allows extensions to store text values in named variables.
    /// This class allows Vocola commands to get and set variable values directly.
    /// <para>Note: these simple text variables should not be confused with
    /// named alternative sets in the Vocola language, also called "variables".</para>
    /// </remarks>
    public class Variable : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // Get

        /// <summary>Gets text from the specified Vocola variable.</summary>
        /// <param name="name">Name of the Vocola variable whose value should be retrieved. Case sensitive.</param>
        /// <returns>The text stored in the specified Vocola variable.</returns>
        /// <example><code title="Insert text from a numbered &quot;bin&quot;">
        /// Paste From 1..9 = Variable.Get($1);</code>
        /// Saying for example "Paste From 3" will retrieve the text stored in the Vocola variable "3"
        /// and send its characters to the current window.
        /// </example>
        [VocolaFunction]
        [CallEagerly(false)]
        static public string Get(string name)
        {
            return VocolaApi.GetVariable(name);
        }

        // ---------------------------------------------------------------------
        // Set

        /// <summary>Stores text in the specified Vocola variable.</summary>
        /// <param name="name">Name of the Vocola variable whose value should be set. Case sensitive.</param>
        /// <param name="value">Text value to store.</param>
        /// <remarks>When you set a variable, the value persists for the remainder of the Vocola session.</remarks>
        /// <example><code title="Store selected text in a numbered &quot;bin&quot;">
        /// Copy To 1..9 = {Ctrl+c} Variable.Set($1, Clipboard.GetText());</code>
        /// Copies the currently-selected text (by sending <c>{Ctrl+c}</c>), retrieves
        /// it from the Windows clipboard (using <see cref="Clipboard.GetText"/>), and stores it in a Vocola variable.
        /// Saying for example "Copy To 3" stores the currently-selected text in the Vocola variable "3".
        /// </example>
        [VocolaFunction]
        [CallEagerly(true)]
        [ClearDictationStack(false)]
        static public void Set(string name, string value)
        {
            VocolaApi.SetVariable(name, value);
        }

    }

}
