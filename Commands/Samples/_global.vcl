# Sample global commands

$include termAlternates.vch;
onLoad() := addTermAlternates();

<n> := 0..100;

# ---------------------------------------------------------------------------
# Switch applications and windows

<app> := ( Mailer  = thunderbird
         | Browser = firefox
         | Finder  = explorer
         | .NET    = devenv
         | Help    = dexplore
         );
pointerHere() := MoveToEdge(left) MoveBy(15,0);

# Switch by executable name
Use   <app>     = SwitchTo($1) pointerHere();
Close <app>     = SwitchTo($1) {Alt+F4};
Use   <app> <n> = TaskBar.SwitchToApplication($1, $2) pointerHere();
Close <app> <n> = TaskBar.SwitchToApplication($1, $2) {Alt+F4};
Copy to <app> = {Ctrl+a}{Ctrl+c} SwitchTo($1) pointerHere();

# Switch by window title
Use   <_windowTitle> = HearCommand("Switch To $1");
Close <_windowTitle> = HearCommand("Switch To $1") WaitForWindow($1) {Alt+F4};

# Switch by taskbar number
Use   <n>     = TaskBar.SwitchToButtonNumber($1) pointerHere();
Close <n>     = TaskBar.SwitchToButtonNumber($1) {Alt+F4};
Use   <n> End = TaskBar.SwitchToButtonNumber(-$1) pointerHere();
Close <n> End = TaskBar.SwitchToButtonNumber(-$1) {Alt+F4};

Switch Window = SendSystemKeys({Alt+Tab}) pointerHere();
Copy and Switch  = {Ctrl+a}{Ctrl+c}{Alt+Tab};
Close Here = {RightButton} Wait(500) c;
Close Window = {Alt+F4};
Show Desktop = {Win+d};
Task Manager = SendSystemKeys({Ctrl+Shift+Esc});

Start Finder = {Win+e} pointerHere();
Launch <n> = TaskBar.LaunchButtonNumber($1);

Switch View     = {Ctrl+Tab};
Switch View <n> = {Ctrl+Tab_$1};
Previous View     = {Ctrl+Shift+Tab};
Previous View <n> = {Ctrl+Shift+Tab_$1};

# ---------------------------------------------------------------------------
# Move and resize windows

<upDown>    :=   Up='-' |  Down='+';
<leftRight> := Left='-' | Right='+';
<edge>      := Top | Bottom | Left | Right;

[Move] Window <n> <upDown>    = MoveToEdge(Top) MoveBy(0,20) DragByScreenPercent(0, $2$1);
[Move] Window <n> <leftRight> = MoveToEdge(Top) MoveBy(0,20) DragByScreenPercent($2$1, 0);

[Size] Window <edge> <n> <upDown>    = MoveToEdge($1) DragByScreenPercent(0, $3$2);
[Size] Window <edge> <n> <leftRight> = MoveToEdge($1) DragByScreenPercent($3$2, 0);

Slam (<edge> | Top Left | Bottom Left | Top Right | Bottom Right)
    = Window.MoveToScreenEdge($1);

Window (Maximize=x | Minimize=n | Restore=r) = SendSystemKeys({Alt+Space}) $1;

Other Screen = Window.MoveToNextScreen();

# ---------------------------------------------------------------------------
# Press keys

$include CommandFile.GetBuiltinsPathname(keys.vch);

# <k> excludes arrow keys so sequences like "Press Alpha - 3 Left" will work
# and excludes letter keys so commands like "Press F3" will work
<k> := <actionKeyNotArrow> | <characterKeyNotLetter>;
<2to99> := 2..99;

# "Press" command (for command sequences)
Press                 <k> = {$1};
Press             <k> <k> = {$1}{$2};
Press         <k> <k> <k> = {$1}{$2}{$3};
Press     <k> <k> <k> <k> = {$1}{$2}{$3}{$4};
Press <k> <k> <k> <k> <k> = {$1}{$2}{$3}{$4}{$5};

Press               <modifierKey> <key> = {$1+$2};
Press <modifierKey> <modifierKey> <key> = {$1+$2+$3};

# Press a key multiple times
                            <k> Times <2to99> = {$1_$2};
              <modifierKey> <k> Times <2to99> = {$1+$2_$3};
<modifierKey> <modifierKey> <k> Times <2to99> = {$1+$2+$3_$4};

# Point at a specific place to insert keys
Touch                 <k> = {LeftButton}{$1};
Touch             <k> <k> = {LeftButton}{$1}{$2};
Touch         <k> <k> <k> = {LeftButton}{$1}{$2}{$3};
Touch     <k> <k> <k> <k> = {LeftButton}{$1}{$2}{$3}{$4};
Touch <k> <k> <k> <k> <k> = {LeftButton}{$1}{$2}{$3}{$4}{$5};

### Miscellaneous key presses

Space Bar = " ";
Tab Key = {Tab};
Tab Back     = {Shift+Tab};
Tab Back <n> = {Shift+Tab_$1};

Page (Up | Down)     = {Page$1};
Page (Up | Down) <n> = {Page$1_$2};

(Expand={Alt+Down} | Collapse={Alt+Up}) That = $1;  # Open/Close drop-down list
Context Menu = {ContextMenu};
Volume Mute = {VolumeMute};

# ---------------------------------------------------------------------------
# Pointer Handling

Touch = {LeftButton};
Touch (Double=2 | Triple=3) = {LeftButton_$1};
Touch (Middle|Right) = {$1Button};
<modifierKey> Touch = {$1+LeftButton};
Touch (Hold|Release) = {LeftButton_$1};

# Place pointer using percentage of screen width/height
<n> By <n> Go    = MoveToScreenPercent($2, $1);
<n> By <n> Touch = MoveToScreenPercent($2, $1) {LeftButton};

# Get current pointer coordinates
copyAndShowPoint(point) := Clipboard.SetText($point) ShowMessage($point);
Get Pointer       = copyAndShowPoint(Pointer.GetOffset());
Get Inner Pointer = copyAndShowPoint(Pointer.GetOffset(WindowInner));

Scroll (Down|Up)     = {Wheel$1};
Scroll           <n> = {WheelDown_$1};
Scroll (Down|Up) <n> = {Wheel$1_$2};

Mouse Away = MoveTo(0,0);

# ---------------------------------------------------------------------------
# Insert things

Quotes Around           <_anything> = '"$1"';
Parens Around           <_anything> = '($1)';
Brackets Around         <_anything> = '[$1]';
Angle Brackets [Around] <_anything> = '<$1>';
Braces Around           <_anything> = '{$1}';

# Insert a link, e.g. in an email
$include urls.vch;
Link <url> = http://$1;

# ---------------------------------------------------------------------------
# Modify active dictation

Cap       <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.Capitalize($1));
Up Case   <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToUpper($1));
Down Case <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToLower($1));
Hyphenate <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.JoinWords($1, "-"));
Scratch   <_vocolaDictation> = Dictation.ReplaceInActiveText($1, "");
Camel     <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToCamelCaseWord($1));
Title     <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.ToTitleCaseWord($1));
Compound  <_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.JoinWords($1, ""));
Replace <_vocolaDictation> With <_anything> = Dictation.ReplaceInActiveText($1, $2);

# ---------------------------------------------------------------------------
# Text Editing

<direction>  := Left | Right | Up | Down;
<left_right> := Left | Right;
<start_end>  := Start={Home} | End={End};

### Characters
<n> <direction>       = {$2_$1};
Kill (Char | 1)       = {Del};
Kill <n>              = {Del_$1};
Kill Back [1]         = {Backspace};
[Kill] Back <n>       = {Backspace_$1};
(Cap | Up Case) <n>   = {Shift+Right_$2} HearCommand("All Caps That");

### Words
[1] Word  <left_right> = {Ctrl+$1};
Forwards  <left_right> = {Ctrl+$1_4};  # "Four Words"
<n> Words <left_right> = {Ctrl+$2_$1};
Copy Word              = {Right_2}{Ctrl+Left}{Shift+Ctrl+Right}   {Ctrl+c};
Copy <n> Words         = {Right_2}{Ctrl+Left}{Shift+Ctrl+Right_$1}{Ctrl+c};
Kill Word              = {Right_2}{Ctrl+Left}{Shift+Ctrl+Right}   {Del};
Kill Forwards          = {Right_2}{Ctrl+Left}{Shift+Ctrl+Right_4} {Del};  # "Four Words"
Kill <n> Words         = {Right_2}{Ctrl+Left}{Shift+Ctrl+Right_$1}{Del};
[Kill] Back Word       = {Left}{Ctrl+Right}{Shift+Ctrl+Left}   {Del};
[Kill] Back Forwards   = {Left}{Ctrl+Right}{Shift+Ctrl+Left_4} {Del};  # "Four Words"
[Kill] Back <n> Words  = {Left}{Ctrl+Right}{Shift+Ctrl+Left_$1}{Del};
Put Word               = {Ctrl+Right}{Shift+Ctrl+Left}{Ctrl+c}{LeftButton}{Ctrl+v};
Replace Word           = {Ctrl+Right}{Shift+Ctrl+Left}{Ctrl+c}{LeftButton}{Shift+Ctrl+Right}{Del}{Ctrl+v};
Camel <n> = {Ctrl+Shift+Left_$1}{Ctrl+c} String.ToCamelCaseWord(Clipboard.GetText());
Title <n> = {Ctrl+Shift+Left_$1}{Ctrl+c} String.ToTitleCaseWord(Clipboard.GetText());

### Lines
Line <start_end>      = $1;
New Line              = {Enter};
Line Here             = {Enter}{Left};
Copy Line             = {home}{Shift+Down}{Shift+Home}{Ctrl+c};
Copy <n> Lines        = {home}{Shift+Down_$1}{Shift+Home}{Ctrl+c};
Copy Here             = {Shift+End}{Ctrl+c}{Left};
Kill Line             = {home}{Shift+Down}{Shift+Home}{Del};
[Kill] Back Line      = {home}{Shift+Up}  {Shift+Home}{Del};
Kill <n> Lines        = {home}{Shift+Down_$1}{Shift+Home}{Del};
[Kill] Back <n> Lines = {home}{Shift+Up_$1}  {Shift+Home}{Del};
Kill Here             = {Shift+End}{Del};
[Kill] Back Here      = {Shift+Home}{Del};
Duplicate Line        = {Home}{Shift+Down}{Shift+Home}{Ctrl+c}{Home}{Ctrl+v};
Duplicate <n> Lines   = {Home}{Shift+Down_$1}{Shift+Home}{Ctrl+c}{Home}{Ctrl+v};
Duplicate Here        = {Shift+End}{Ctrl+c}{Right}{Ctrl+v};
Isolate Line          = {End}{Enter}{Up}{Enter};
Join Line             = {End}{Del} " ";
Join [Line] <n>       = Repeat($1, {End}{Del} " ");
Another One           = {End}{Enter};
                    
### Paragraphs        
Graph Start          = {Ctrl+Up}{Right}{Home};
Graph End            = {Ctrl+Down}{Left_2}{End};
Graph Here           = {Enter}{Enter}{Left}{Left};
Open (Graph|Line)    = {Enter}{Enter}{Left};
Copy Graph           = {Ctrl+Down}{Shift+Ctrl+Up}{Ctrl+c};
Kill Graph           = {Ctrl+Down}{Shift+Ctrl+Up}{Del};
Duplicate Graph      = {Ctrl+Down}{Shift+Ctrl+Up}{Ctrl+c}{Home}{Ctrl+v};
                    
### Entire "Flow"   
(Flow|Buffer) Start  = {Ctrl+Home};
(Flow|Buffer) End    = {Ctrl+End};
Select All           = {Ctrl+a};
Copy All             = {Ctrl+a}{Ctrl+c};
(Cut|Kill) All       = {Ctrl+a}{Ctrl+x};
Kill Flow Here       = {Ctrl+Shift+End} {Ctrl+x};
[Kill] Back Flow Here= {Ctrl+Shift+Home}{Ctrl+x};
Replace All          = {Ctrl+a}{Del}{Ctrl+v};
                    
### Selection         
Undo That            = {Ctrl+z};
Undo <n>             = {Ctrl+z_$1};
Redo That            = {Ctrl+y};
Kill That            = {Del};
Cut That             = {Ctrl+x};
Copy That            = {Ctrl+c};
Paste That           = {Ctrl+v};
(Paste Here|Touch Paste) = {LeftButton}{Ctrl+v};
Paste Clean          = Clipboard.ConvertToPlainText() {Ctrl+v};
Duplicate That       = {Ctrl+c}{Left}{Ctrl+v};
Keep That            = {Ctrl+c}{Ctrl+a}{Del}{Ctrl+v};

### Global clipboard bins
Copy To    <n> = {Ctrl+c} Variable.Set($1, Clipboard.GetText());
Paste From <n> = Variable.Get($1);

# ---------------------------------------------------------------------------
# Accessing folders and files

$include folders.vch;
$include files.vch;

Open File <folder> = {Ctrl+o} $1 {Enter};

$if Open|New|Save|File|Attachment|Browse|Directory|Extract|Reference;
  Folder <folder> = {Ctrl+c}$1\{Enter};
  Go Up = ..{Enter};
  Go Up <n> = Repeat($1, ..\) {Enter};
$end

Buffer <file>   = SwitchTo(emacs) {Ctrl+x}{Ctrl+f} $1 {Enter};
Buffer Temp <n> = SwitchTo(emacs) {Ctrl+x}{Ctrl+f}C:\Users\Rick\Rick\Temp\temp$1{Enter};

# ---------------------------------------------------------------------------
# Control WSR

Speech Menu = HearCommand("Show Speech Options");
Edit Words  = HearCommand("Open Speech Dictionary");

# Toggle ms speech (can also use hotkey Ctrl+Win)
# Note that behavior is controlled by this registry entry:
#     Key:  HKEY_CURRENT_USER\Software\Microsoft\Speech\Preferences
#   Value:  "ModeForOff" (DWORD)
# Choices:  0 == OFF_BATTERY (sleeping when on AC power, off when on batteries) (default)
#           1 == OFF_OFF (truely OFF when "off" regardless of AC/battery power)
#           2 == OFF_SLEEPING (sleeping when "off" regardless of AC/battery power)

Speech Off = HearCommand("Stop Listening");

# "Click" is hard to say a lot, so substitute "Please"
Please <_itemInWindow> = HearCommand("Click $1");

# Capture single-word WSR commands and convert to dictation
<wsrWord> := ( delete | escape | start | home | end
             | scratch | undo | correct | copy | paste
             );
<wsrWord> = HearCommand("Insert $1");

# ---------------------------------------------------------------------------
# Specific programs

(Restart=r|Log Off=l) The Computer = SendSystemKeys({Ctrl+Esc}) {Right_3}$1;
Start Menu = SendSystemKeys({Ctrl+Esc});
Run Program = SendSystemKeys({Win+r});
Environment Variables = RunProgram(C:\Windows\System32\SYSDM.CPL)
                        WaitForWindow("System Properties", 15)
                        {Ctrl+Tab_2}{Alt+n}
                        ;

Manage Computer = RunProgram("compmgmt.msc \s");
[Add] Remove Programs = RunProgram(appwiz.cpl);

Web Search = SwitchTo(firefox) {Alt+g};
Web Search That = {Ctrl+c} SwitchTo(firefox) {Alt+g} '"{Ctrl+v}" ';
Web Search <_anything> = SwitchTo(firefox) {Alt+g} '"$1" ';

New Message = RunProgram(mailto:);

