# Voice commands for Microsoft Word 2007

Open File = {Ctrl+o};
Save File = {Ctrl+s};
Save As = SendSystemKeys({Alt+f})a;
Recent Files = SendSystemKeys({Alt+f});
[Recent] File 1..9 = SendSystemKeys({Alt+f}) $1;
Close That = SendSystemKeys({Alt+f})c;

View (Outline=u | (Normal|Draft)=e | Layout=p | Web Layout=l) = {Alt+w} $1;

# ---------------------------------------------------------------------------
# Editing

Kill Line      = {End}{Home}{Shift+End}{Ctrl+c}{Ctrl+x};
Copy Line      = {End}{Home}{Shift+End}{Ctrl+c}{Right};
Duplicate Line = {End}{Home}{Shift+End}{Ctrl+c}{Right}{Ctrl+v};

Kill Here      = {Shift+End}{Shift+Left}{Ctrl+x};
Copy Here      = {Shift+End}{Shift+Left}{Ctrl+c};
Kill Back Here = {Shift+Home}{Shift+Right}{Ctrl+x};

(Subscript=b | Superscript=p) That = {Alt+o}f{Alt+$1}{Enter};
En Dash = {Ctrl+NumKey-};
Em Dash = {Ctrl+Alt+NumKey-};

# ---------------------------------------------------------------------------
# Find/Replace

Find That = {Ctrl+c}{Ctrl+f}{Ctrl+v}{Enter}{Esc};
Replace Text = {Alt+e}e;

$if "Find and Replace";
  Replace = {Alt+r};
  Replace All = {Alt+a};
  Find [Next] = {Alt+f};
$end

# ---------------------------------------------------------------------------
# Common markup

Format Paragraph = {Alt+o}p;

Keep with Next = {Alt+h}pg{Alt+p}{Alt+x}{Enter};#{Alt+o}p;
Show Breaks = {Alt+o}p;
(Page=p | Column=c) Break = {Alt+i}b $1 {Enter};

# ---------------------------------------------------------------------------
# Styles

Style ( Body = "Body Text"
      | None = "Default Paragraph Font"
      | Normal | Code | Code Line | Strong
      ) = SendSystemKeys({Ctrl+S}) WaitForWindow("Apply Styles") $1 {Enter};

Style (List|Item) = {Alt+h}m{Home}{Down_4}{Enter};

Heading 0..9 = SendSystemKeys({Ctrl+S}) WaitForWindow("Apply Styles") "Heading $1" {Enter};

Edit Style = {Alt+o} Wait(100) s{Alt+m}{Alt+o};

# ---------------------------------------------------------------------------
# Tables

<tableThing> := (Table=t | Row=r | Column=c | Cell=e);
(Select=c | Kill=d) <tableThing> = {Alt+a} $1 $2;

Insert Table = {Alt+a}it{Alt+d}{Enter};
Insert ( Row   =b | Row    After=b | Row    Before=a
       | Column=r | Column After=r | Column Before=l ) = {Alt+a}i $1;

Split (Cells | That) = {Alt+a}p{Enter};
Merge (Cells | That) = {Alt+a}m;
Merge Row = {Alt+Home}{Shift+Alt+End}{Alt+a}m;

Align (Top=p | Center=c | Bottom=b) (Left=l | Center=c | Right=r) =
    {Alt+a}r{Alt+e}{Alt+ $1 }{Enter}{Alt+o}p{Alt+i}{Alt+g} $2 {Enter}{Enter};

Fit (Window=w | Contents=c | Fixed=n) = {Alt+j}lf $1;

Sort That = SendSystemKeys({Alt+a}) s Wait(100) {Enter};

(Show|Hide) Grid = {Alt+a}g;
convertToText() := {Alt+j}lv WaitForWindow(convert) {Enter};
Convert To Text = convertToText();
Convert To Text 1..20 = Repeat($1, convertToText() {Up});

selectTable() := {Alt+a}ct;
selectRow()   := {Alt+a}cr;
killPadding() := {Alt+a}r WaitForWindow(properties) 
                 {Alt+o} WaitForWindow(options) Wait(100) {Alt+s}{Enter}
                 {Tab_2}{Enter};

fixLyricsTable() := selectTable() killPadding() {Left} selectRow()  
                    {Ctrl+Shift+s}Chord{Enter};

Fix Table 1..20 = Repeat($1, fixLyricsTable() {Up});

# ---------------------------------------------------------------------------
# Outlines

New Item         = {Ctrl+Down}{Left_2}{End}{Enter};
New Item Promote = {Ctrl+Down}{Left_2}{End}{Enter}{Alt+Shift+Left};
New Item Demote  = {Ctrl+Down}{Left_2}{End}{Enter}{Alt+Shift+Right};

Promote That = {Alt+h}ao;
Demote  That = {Alt+h}ai;

Promote 1..9 = Repeat(3, {Alt+h}ao);
Demote  1..9 = Repeat(3, {Alt+h}ai);

Expand   That = {Alt+Shift+Plus};
Collapse That = {Alt+Shift+Minus};

Item (Up | Down)       = {Alt+Shift+$1};
Item (Up | Down) 1..20 = {Alt+Shift+$1_$2};

# ---------------------------------------------------------------------------
# Miscellaneous

$if "Accept or Reject Changes";
  Accept = {Alt+a};
  Reject = {Alt+r};
  Find = {Alt+f};
$end
