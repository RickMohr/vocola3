# Voice commands for Microsoft Visual Studio 2005 and 2008
#
# Note that many of these commands use the VisualStudio sample Vocola extension.
#   For documentation, see Extensions\Samples\VisualStudio\Doc in the Vocola installation folder
#   To install, copy Extensions\Samples\VisualStudio\VisualStudio.dll from the Vocola installation folder
#     to your Vocola extensions folder (C:\Users\<username>\Vocola3\Extensions by default).

### Solutions and Projects

Open Solution = {Ctrl+Shift+o};
Close Solution = {Alt+f}t;
Recent (Solution | Project) = {Alt+f}j;

Add Project = {Alt+f}de;
Project (Settings=p | Dependencies=s) = {Alt+p}$1;

Rebuild = {Ctrl+Shift+b};
Build Project = {Shift+F6};
Build Project Only = {Alt+b}jb;  # for C++ projects
Stop Build = {Ctrl+Break};

# Solution Explorer
solutionExplorer() := {Ctrl+Alt+l};
Solution Explorer = solutionExplorer();
Rename That = {F2};
Show (ASP={Shift+F7} | C Sharp={F7} | Code={F7}) = $1;
Show (Design|HTML) = {Ctrl+PgDn};

### Files 

Save File = {Ctrl+s};
Save All  = {Ctrl+S};
Save As = {Alt+f}a;
Close That = {Alt+f}c;

Find in Files = {Ctrl+F};
Replace In Files = {Ctrl+H};

copyFileName() := MoveTo(37,87) {RightButton}{Down_4} Wait(500) {Enter};
relayEmacs(cs) := {Ctrl+s} VisualStudio.SetCurrentFileVariables(CurrentFilename, CurrentLineNumber, CurrentColumnNumber)
                  SwitchTo(emacs) {Ctrl+x}{Ctrl+f} Variable.Get(CurrentFilename) $cs{Enter}
                  {Alt+G} Wait(100) Variable.Get(CurrentLineNumber) {Enter}
                  {Esc} Variable.Get(CurrentColumnNumber) {Alt+x}move-to-column{Enter}{Left};
Copy File Name = copyFileName();
Relay Emacs = relayEmacs("");
Relay Emacs Code = relayEmacs(".cs");
Full Screen = {Alt+Shift+Enter};

Switch Buffer = {Ctrl+Tab}{Enter};
Switch Buffer <n> = {Ctrl+Tab}{Down_$1}{Up}{Enter};
No Buffer <n> = {Ctrl+Tab}{Enter}{Ctrl+Tab}{Down_$1}{Up}{Enter};

# Bookmarks
[Set] [Remove] [Clear] Bookmark = {Ctrl+b}t;
Touch Bookmark = {LeftButton}{Ctrl+b}t;
(Last=p | Next=n)  Bookmark = {Ctrl+b} $1;
(Clear|Remove) [All] Bookmarks = {Ctrl+b}c{Enter};

### Debugging

[Set] [Remove] [Clear] Breakpoint = {F9};
Touch Breakpoint = {LeftButton}{F9};
(Clear|Remove) [All] Breakpoints = {Ctrl+Shift+F9}{Enter};
Continue = {F5};
Execute = {Ctrl+F5};
Restart = {Ctrl+Shift+F5};
Step Over = {F10};
Step Into = {F11};
Step Out = {Shift+F11};
Stop Debugging = {Shift+F5};
Show Exceptions = {Ctrl+Alt+e};
Exceptions (On|Off) = {Ctrl+Alt+e} WaitForWindow(Exceptions) Touch(-324,110) {Enter};

# ---------------------------------------------------------------------------
# Text Navigation and Editing

### edit.vch contains text-editing commands, which call functions to do their work.
### Here we define those functions for Visual Studio

$include edit.vch;

switchWindow() := {Alt+Tab};

gotoLine(lineNumber) := {Ctrl+g};
gotoVisibleLine(lineNumberSuffix)       := VisualStudio.GoToVisibleLine($lineNumberSuffix);
gotoVisibleLineExtend(lineNumberSuffix) := VisualStudio.GoToVisibleLine($lineNumberSuffix, true);

gotoLineStart()       := {Home}{Home};
gotoLineEnd()         := {End};
gotoLineStartExtend() := {Shift+Home};
gotoLineEndExtend()   := {Shift+End};

gotoGrafStart(n)       := VisualStudio.MoveByParagraphs(-$n);
gotoGrafEnd(n)         := VisualStudio.MoveByParagraphs( $n);
gotoGrafStartExtend(n) := VisualStudio.MoveByParagraphs(-$n, true);
gotoGrafEndExtend(n)   := VisualStudio.MoveByParagraphs( $n, true);

gotoFlowStart()        := {Ctrl+Home};
gotoFlowEnd()          := {Ctrl+End};
gotoFlowStartExtend()  := {Shift+Ctrl+Home};
gotoFlowEndExtend()    := {Shift+Ctrl+End};

moveByWords(n) := VisualStudio.MoveByWords($n);
selectWords(n) := VisualStudio.SelectWords($n);

selectChars(n) := {Shift+Right_$n};
selectLines(n) := {End}{Home}{Home}{Shift+Down_$n};
selectGraf()   := VisualStudio.MoveByParagraphs(1) VisualStudio.MoveByParagraphs(-1, true);
selectAll()    := {Ctrl+a};

saveCaret()    := VisualStudio.SaveCaret();
restoreCaret() := VisualStudio.RestoreCaret();

copy()      := {Ctrl+c}{Right};
delete()    := {Del};
cut()       := {Ctrl+x};
paste()     := {Ctrl+v};
duplicate() := {Ctrl+c}{Right}{Ctrl+v};

mark()       := ""; # No-op
centerLine() := VisualStudio.RunCommand(Edit.ScrollLineCenter);
capitalize() := VisualStudio.RunCommand(Edit.Capitalize) {Right};
upcase()     := VisualStudio.RunCommand(Edit.MakeUppercase) {Right};
downcase()   := VisualStudio.RunCommand(Edit.MakeLowercase) {Right};

addLine() := {End}{Enter};
joinLine() := {End}{Del} VisualStudio.RunCommand(Edit.DeleteHorizontalWhiteSpace);

indent(levels)   := Repeat($levels, VisualStudio.RunCommand(Edit.IncreaseLineIndent));
unindent(levels) := Repeat($levels, VisualStudio.RunCommand(Edit.DecreaseLineIndent));
shrinkSpace()    := VisualStudio.RunCommand(Edit.DeleteHorizontalWhiteSpace);
deleteSpace()    := VisualStudio.RunCommand(Edit.DeleteHorizontalWhiteSpace);

find(text)        := {Ctrl+/}$text{F3};
findUp(text)      := {Ctrl+/}$text{Shift+F3};
findAgain()       := {F3};
findAgainUp()     := {Shift+F3};
findWord()        := {Ctrl+F3};
findWordUp()      := {Ctrl+F3}{Shift+F3_2};
findSelection()   := {Ctrl+c}{Ctrl+F3};
findSelectionUp() := {Ctrl+c}{Shift+F3_2};

### Miscellaneous

Show  (Definition|Define|Go) = {ContextMenu}g;
Touch (Definition|Define|Go) = {RightButton}g;
Touch References = {RightButton}a;
Complete [Name] = VisualStudio.RunCommand(Edit.CompleteWord);
Insert (For Each=foreach | try) = {Ctrl+k}x $1 {Enter};

Camel <n> = VisualStudio.SelectWords(-$1) {Ctrl+c} String.ToCamelCaseWord(Clipboard.GetText());
Title <n> = VisualStudio.SelectWords(-$1) {Ctrl+c} String.ToTitleCaseWord(Clipboard.GetText());

# Intellisense
<1to9> := 1..9;
Choose <1to9> = {Down_$1}{Enter};
<1to9> OK     = {Down_$1}{Enter};
Choose <1to9> Up = {Up_$1}{Enter};
<1to9> Up OK     = {Up_$1}{Enter};

### Editing Code

$include editCode.vch;

code(code) := Clipboard.SetText($code) paste();  # avoid Intellisense interference

indentCode()     := {Ctrl+e}f{Right};
indentCodeLine() := {Ctrl+e}f;
commentLines()   := {Ctrl+e}c{Right_2};
uncommentLines() := {Ctrl+e}u{Right_2};

newBlock()             := addLine() { addLine() } {Up} addLine();
newBlockOf(nLines)     := addLine() { {Down_$nLines}               addLine() };
newBlockAt(lineNumber) := addLine() { gotoVisibleLine($lineNumber) addLine() };

### HTML

$if htm | html;
  Promote That = {Ctrl+T};
  Demote That  = {Ctrl+t};
  Hyperlink = {Ctrl+l}{Enter}{F4}{Down};
$end
