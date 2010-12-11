# Voice commands for Microsoft Excel 2000 & 2007

### Files

Open File = {Ctrl+o};
Save File = {Ctrl+s};
Save As = SendSystemKeys({Alt+f}) a;
Close That = SendSystemKeys({Alt+f}) c;
Recent Files = SendSystemKeys({Alt+f}) ;
File 1..9 = SendSystemKeys({Alt+f}) $1;

### Rows and columns

<n> := 1..40;
<row_column> := (Row|Rows|Line|Lines)=r | (Column|Columns)=c;

Kill <row_column> = {Alt+e}d $1 {Enter};
Kill <n> <row_column> = {Shift+Up}{Shift+Down_$1}{Alt+e}d $2 {Enter}{Left};

Insert <row_column> = {Alt+i} $1;
Insert <n> <row_column> = Repeat($1, {Alt+i} $2);

Duplicate (That|Line) = {Ctrl+c}{Alt+i}e{Esc};

Fit <row_column> = {Ctrl+a}{Alt+o} $1 a{Right}{Left};

### Cells

Touch Copy  = {LeftButton}{Ctrl+c};
Touch Cut   = {LeftButton}{Ctrl+x};
Touch Paste = {LeftButton}{Ctrl+v};
Touch Insert = {RightButton}e;

Sort Lines = {Alt+d}s;
Sort All   = {Ctrl+a}{Alt+d}s;

# Cell contents
Cell Touch = {LeftButton_2};
Cell Start = {LeftButton_2}{Home};
Cell End   = {LeftButton_2}{End};
Cell Word  = {LeftButton_2} Wait(100) {LeftButton_2};
Cell Copy  = {LeftButton}{F2}{Shift+Home}{Ctrl+c}{Esc};
Cell Paste = {LeftButton_2}{Ctrl+v};
Cell Paste Partner = {LeftButton_2} " & " {Ctrl+v};
Cell Move Partner = {LeftButton_2} Wait(100) {LeftButton_2} Wait(100) {Ctrl+x}{Backspace_3}{Tab_2}{Ctrl+v};
Cell Back 1..9 = {LeftButton_2}{Backspace_$1}{Enter}{Up};

### Formatting
Bold That = {Ctrl+b};
Italics That = {Ctrl+i};
Bold Italics That = {Ctrl+b}{Ctrl+i};

### General

Kill That = {Alt+e}d;
Conditional Formatting = {Alt+o}d;
Paste (Format | Formatting) = {Alt+e}s{Alt+t}{Enter};
