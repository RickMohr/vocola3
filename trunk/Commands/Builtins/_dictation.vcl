# Commands related to Vocola dictation

$using Library;

Text On  = Dictation.Enable();
Text Off = Dictation.Disable();

Dictation Shortcuts = Dictation.ShowDictationShortcutsDialog();

Phrase Start = {Left_ String.Length( Dictation.Get() ) };

### Modify the phrase that was just spoken

# We want commands like "Cap That" to work for both Vocola dictation and WSR dictation.
# If there's no Vocola dictation to operate on, try an appropriate WSR command.

Fix That       = If( Dictation.CanGet(),
                     Dictation.Correct(),
                     HearCommand("Correct That") );

Cap That       = If( Dictation.CanGet(),
                     Dictation.Replace( String.Capitalize( Dictation.Get() )),
                     HearCommand("Capitalize That") );

All Caps That  = If( Dictation.CanGet(),
                     Dictation.Replace( String.ToUpper( Dictation.Get() )),
                     HearCommand("Upper Case That") );

No Caps That   = If( Dictation.CanGet(),
                     Dictation.Replace( String.ToLower( Dictation.Get() )),
                     HearCommand("Lower Case That") );

Hyphenate That = If( Dictation.CanGet(),
                     Dictation.Replace( String.JoinWords( Dictation.Get(), "-")),
                     HearCommand("Add Hyphens To That") );

Scratch That   = If( Dictation.CanGet(),
                     Dictation.Replace( ""),
                     HearCommand("Select That") HearCommand("Delete That"));

# These commands have no analogous WSR command, but we can still make them work with WSR dictation

Camel [Case] That = If( Dictation.CanGet(),
                        Dictation.Replace( String.ToCamelCaseWord( Dictation.Get() )),
                        HearCommand("Select That") Wait(100) {Ctrl+c} String.ToCamelCaseWord( Clipboard.GetText() ));

Title That        = If( Dictation.CanGet(),
                        Dictation.Replace( String.ToTitleCaseWord( Dictation.Get() )),
                        HearCommand("Select That") Wait(100) {Ctrl+c} String.ToTitleCaseWord( Clipboard.GetText() ));

Title Case That   = If( Dictation.CanGet(),
                        Dictation.Replace( String.ToTitleCase( Dictation.Get() )),
                        HearCommand("Select That") Wait(100) {Ctrl+c} String.ToTitleCase( Clipboard.GetText() ));

Compound That     = If( Dictation.CanGet(),
                        Dictation.Replace( String.JoinWords( Dictation.Get(), "")),
                        HearCommand("Select That") Wait(100) {Ctrl+c} String.JoinWords( Clipboard.GetText(), ""));

Underscore That   = If( Dictation.CanGet(),
                        Dictation.Replace( String.JoinWords( Dictation.Get(), "_")),
                        HearCommand("Select That") Wait(100) {Ctrl+c} String.JoinWords( Clipboard.GetText(), "_"));

# These commands work only with Vocola dictation, not WSR dictation

Fix Cap           = Dictation.Replace( String.ToggleInitialCase( Dictation.Get() ));
Fix (Space|Pace)  = Dictation.Replace( String.ToggleInitialSpace( Dictation.Get() ));
Fix Both          = Dictation.Replace( String.ToggleInitialCase( String.ToggleInitialSpace( Dictation.Get() )));
Try Again         = Dictation.ReplaceWithAlternate();

# Modify active dictation

(Cap|Caps) <_vocolaDictation> = Dictation.ReplaceInActiveText($2, String.Capitalize($2));
All Caps   <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToUpper($1));
No Caps    <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToLower($1));
Hyphenate  <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.JoinWords($1, "-"));
Camel      <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToCamelCaseWord($1));
Title      <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToTitleCaseWord($1));
Compound   <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.JoinWords($1, ""));

Replace <_vocolaDictation> With <_anything> = Dictation.ReplaceInActiveText($1, $2);
