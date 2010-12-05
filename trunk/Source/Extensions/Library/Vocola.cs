using System;
using Vocola;

namespace Library
{

    /// <summary>Functions for interacting with the Vocola application.</summary>
    public class Vocola : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // AddTermAlternate

        /// <summary>Specifies alternate words to be recognized when speaking commands containing a specified
        /// word.</summary>
        /// <param name="term">Word for which alternates are desired. May not contain a space character.</param>
        /// <param name="alternates">One or more alternate words to be recognized when speaking commands containing
        /// <paramref name="term"/>.</param>
        /// <remarks>Sometimes commands are misrecognized as dictation, especially when using command sequences.
        /// For example, the command "Line End" may be misrecognized as the dictation "line and". Use this
        /// function to register alternate words (such as "And") for problematic command words (such as "End").
        /// Vocola will recognize any command containing <paramref name="term"/> even if one of the <paramref
        /// name="alternates"/> is spoken instead. So if the recognizer hears "Line And" Vocola will still invoke the
        /// command "Line End". It's as if the "Line End" command were written as <c>Line (End|And)</c>,
        /// except it doesn't obscure the meaning of the command and it works for all commands containing the word
        /// "End".</remarks>
        /// <example><code title="Add alternate terms">
        /// onLoad() := Vocola.AddTermAlternate(End, And)
        ///             Vocola.AddTermAlternate(Find, Fine)
        ///             Vocola.AddTermAlternate(Paste, Pace, Base, Based);
        /// </code>
        /// These calls add "And" as an alternate for "End", add "Fine" as an alternate for "Find", and add three
        /// alternates for "Paste".
        /// <para> Here <see cref="AddTermAlternate"/> is called not by a voice command but by defining an
        /// <c>onLoad</c> function. Putting this code in <c>_global.vcl</c> will
        /// add the alternate terms without further action on your part.</para>
        /// </example>
        [VocolaFunction]
        static public void AddTermAlternate(string term, params string[] alternates)
        {
            foreach (string alternate in alternates)
                VocolaApi.AddTermAlternate(term, alternate);
        }

        // ---------------------------------------------------------------------
        // ExitVocola

        /// <summary>Closes the Vocola application.</summary>
        /// <example><code title="Exit Vocola">
        /// Exit Vocola = UI.ExitVocola();</code>
        /// Saying "Exit Vocola" closes the Vocola application.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void ExitVocola()
        {
            VocolaApi.ExitVocola();
        }

        // ---------------------------------------------------------------------
        // ShowFunctionLibraryDocumentation
        // ShowLogWindow
        // ShowOptionsDialog
        // ShowVocolaMenu

        /// <summary>Opens the Vocola function library documentation.</summary>
        /// <remarks>The Vocola function library documentation, opened by this function, may also be opened by right-clicking
        /// the Vocola icon in the Windows system tray.</remarks>
        /// <example><code title="Open function library documentation">
        /// [Vocola] Library Doc = UI.ShowFunctionLibraryDocumentation();</code>
        /// Saying "Library Doc" opens the Vocola function library documentation.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void ShowFunctionLibraryDocumentation()
        {
            VocolaApi.ShowFunctionLibraryDocumentation();
        }

        /// <summary>Activates the Vocola Log window.</summary>
        /// <remarks>The Vocola Log window, activated by this function, displays error messages, command and
        /// dictation activity, and diagnostics. It is activated automatically by Vocola errors, and may also be activated
        /// by right-clicking the Vocola icon in the Windows system tray to raise the Vocola menu.</remarks>
        /// <example><code title="Activate Vocola Log window">
        /// Vocola Log = UI.ShowLogWindow();</code>
        /// Saying "Vocola Log" activates the Vocola Log window.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void ShowLogWindow()
        {
            VocolaApi.ShowLogWindow();
        }

        /// <summary>Activates the Vocola "Options" dialog box.</summary>
        /// <remarks>The Vocola "Options" dialog box, activated by this function, allows setting
        /// Vocola preferences and parameters. It may also be activated
        /// by right-clicking the Vocola icon in the Windows system tray to raise the Vocola menu.</remarks>
        /// <example><code title="Activate Vocola Options dialog">
        /// Vocola Options = UI.ShowOptionsDialog();</code>
        /// Saying "Vocola Options" activates the Vocola Options dialog.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void ShowOptionsDialog()
        {
            VocolaApi.ShowOptionsDialog();
        }

        /// <summary>Activates the Vocola menu.</summary>
        /// <remarks>The Vocola menu, activated by this function, may also be activated by right-clicking
        /// the Vocola icon in the Windows system tray.</remarks>
        /// <example><code title="Activate Vocola menu">
        /// Vocola Menu = UI.ShowVocolaMenu();</code>
        /// Saying "Vocola Menu" activates the Vocola menu.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void ShowVocolaMenu()
        {
            VocolaApi.ShowVocolaMenu();
        }

    }

}
