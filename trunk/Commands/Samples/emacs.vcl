# Sample commands for Emacs

do(command) := {Alt+x} $command {Enter};
elisp(e) := {Alt+:} $e {Enter};
switchWindow() := SendSystemKeys({Alt+Tab});

# ---------------------------------------------------------------------------
# Buffer and File Manipulation

$include folders.vch;
$include   files.vch;

Buffer <file>        = {Ctrl+x}{Ctrl+f} $1 {Enter};
Buffer <file> Revert = {Ctrl+x}{Ctrl+f} $1 {Enter}
                       do(revert-buffer) yes{Enter};
Switch Buffer = {Alt+o};
[Switch] Buffer 1..20 = {Ctrl+x}{Ctrl+b}{Down_$1} " ";
No Buffer 1..20 = {Alt+o}{Ctrl+x}{Ctrl+b}{Down_$1} " ";
Kill Buffer        = {Ctrl+x}k{Enter};
Two Buffers        = {Ctrl+x}2;
Two Buffers Across = {Ctrl+x}3;
One Buffer         = {Ctrl+x}1;
(Other Buffer | Left Side | Right Side) = do(other-window);
One Other Buffer   = {Ctrl+x}0;
Buffer List        = {Ctrl+x}{Ctrl+b};
Revert Buffer = do(revert-buffer) yes{Enter};
Revert Buffer Hard = {Ctrl+x}{Ctrl+v}{Enter};
Print Buffer = {Ctrl+c}p;
Print Landscape = do(print-landscape);
Print Portrait  = do(print-portrait);
Print Two Up    = do(print-2-up);
Buffer Shell = do(shell);
Buffer (scratch|compilation) = {Ctrl+x}b *$1* {Enter};
Buffer Temp <n> = {Ctrl+x}{Ctrl+f}C:\Users\Rick\Rick\Temp\temp$1{Enter};
[Toggle] Read Only = {Ctrl+x}{Ctrl+q};
[Toggle] Fill Mode = do(auto-fill-mode);
[Toggle] (Overwrite|Overstrike) Mode = do(overwrite-mode);
Window (Maximize=x | Minimize=n | Restore=r) = Touch(20,20) $1;

Open File  = {Ctrl+x}{Ctrl+f};
Save (File={Ctrl+s} | All=s | As={Ctrl+w}) = {Ctrl+x} $1;
Open File <folder> = {Ctrl+x}{Ctrl+f}{Ctrl+a}{Ctrl+k} $1 /;
Save As   <folder> = {Ctrl+x}{Ctrl+w}{Ctrl+a}{Ctrl+k} $1 /;
Wrong File = {Ctrl+x}{Ctrl+v};
Open That = {Ctrl+Space}{Ctrl+e} do(search-backward-regexp) \w{Enter}{Right}{Alt+w}{Ctrl+x}{Ctrl+x}
            {Ctrl+x}{Ctrl+f}{Ctrl+y}{Enter};
Recover File = do(recover-file) {Enter}yes{Enter};
Make Directory = do(make-directory) {Enter};

copyFileName() := elisp("(kill-new (flip-slashes buffer-file-name))");
Copy File Name = copyFileName();
Copy Leaf Name = elisp("(kill-new (file-name-nondirectory buffer-file-name))");

Relay .NET = copyFileName() 
             SwitchTo(devenv) {Ctrl+o}
             WaitForWindow("Open File") {Ctrl+v}{Enter}
             SwitchTo(emacs) elisp("(copy-line-number)")
             SwitchTo(devenv) {Ctrl+g}{Ctrl+v}{Enter}
             WaitForWindow(Studio) {Down_20}{Up_20}{End};

Relay Finder = copyFileName() SwitchTo(explorer) MoveTo(0,0)
               {Alt+d}{Ctrl+v};
Reload [Dot] Emacs = do(load-library) ~/.emacs{Enter};
(Exit Emacs | Close Window) = {Ctrl+x}{Ctrl+c};

# ---------------------------------------------------------------------------
# Text Navigation and Editing

### Edit thing

<edit> := ( Select    =
          | Copy      = {Alt+w}
          | Duplicate = {Alt+w}{Ctrl+y}
          | Kill      = {Ctrl+w}
          );

<thing> := ( That       =
           | Line       = {Ctrl+a}{Ctrl+Space}{Ctrl+n}   
           | Item       = do(mark-outline-item)
           | Graph      = {Alt+h}                        
           | (Flow|All) = {Ctrl+x}h                      
           );

<edit> <thing> = $2 $1;

### Edit to start or end of thing

<thing_start> := ( Word          = {Alt+b}
                 | Line          = {Ctrl+a}
                 | Item          = do(outline-item-start)
                 | Graph         = {Esc}{
                 | (Buffer|Flow) = "{Alt+<}"
                 );
<thing_end>   := ( Word          = {Alt+f}
                 | Line          = {Ctrl+e}
                 | Item          = do(outline-item-end)
                 | Graph         = {Esc}}
                 | (Buffer|Flow) = "{Alt+>}"
                 );

<thing_start> Start = $1;
<thing_end>   End   = $1;
<edit>      <thing_end>   Here = {Ctrl+Space} $2 $1;
<edit> Back <thing_start> Here = {Ctrl+Space} $2 $1;

### Characters

<edit> <n>    = {Ctrl+Space}{Right_$2} $1;
Cap 1         = {Ctrl+Space}{Right}    do(upcase-region);
Up Case   <n> = {Ctrl+Space}{Right_$1} do(upcase-region);
Down Case <n> = {Ctrl+Space}{Right_$1} do(downcase-region);

### Words

<left_right> := Left={Alt+b} | Right={Alt+f};
[One] Word  <left_right> = $1;
<n>   Words <left_right> = {Esc}$1 $2;
Forwards    <left_right> = {Esc}4  $1;  # "Four Words"

Word <n> = {Ctrl+a}{Esc} $1 {Alt+f}{Alt+b};

<editword> := ( Kill        = {Alt+d} 
              | [Kill] Back = {Alt+Backspace}
              | Copy        = {Alt+d}{Ctrl+x}u
              | Duplicate   = {Alt+d}{Ctrl+x}u{Ctrl+y}
              | Cap         = {Alt+c}
              | Up Case     = {Alt+u}
              | Down Case   = {Alt+l}
              );
<editword> Word = $1;
<editword> This Word = {Alt+b} $1;
<editword> <n> Words = {Esc} $2 $1;
<editword> Forwards  = {Esc} 4 $1;  # "Four Words"
Replace Word      = {Ctrl+Space}{Alt+f}   {Alt+w}{LeftButton}{Ctrl+y}{Alt+d}   {Alt+b};
Replace <n> Words = {Ctrl+Space}{Alt+f_$1}{Alt+w}{LeftButton}{Ctrl+y}{Alt+d_$1}{Alt+b_$1};
Get Word      = {Ctrl+Space}{LeftButton}{Alt+d}          {Ctrl+x}u{Ctrl+x_2}{Ctrl+y};
Get <n> Words = {Ctrl+Space}{LeftButton}{Alt+d_$1}{Esc}$1{Ctrl+x}u{Ctrl+x_2}{Ctrl+y};
Put Word      = {Ctrl+Space}{Alt+f}   {Alt+w}{LeftButton}{Ctrl+y};
Put <n> Words = {Ctrl+Space}{Alt+f_$1}{Alt+w}{LeftButton}{Ctrl+y};

(Camel={Alt+l} | Title={Alt+c}) <n> = {Alt+b_$2} $1 Repeat(Eval($2-1), {Ctrl+d}{Alt+c});
Complete Name = {Alt+/};

### Lines

New Line = {Enter};
Newline Indent = {Ctrl+j};
Another One = {End}{Ctrl+j};
Line Here = {Ctrl+o};
(Paragraph|Graph) Here = {Ctrl+o}{Ctrl+o};
Open Line = {Enter}{Ctrl+o};
Isolate Line = {Ctrl+Space}{Ctrl+a}{Down}{Ctrl+o}{Up}{Enter}{Ctrl+x_2};
Join Line = {Ctrl+u} do(delete-indentation);
Join [Line] <n> = Repeat($1, {Ctrl+t});

moveLine(dir) := {Ctrl+a}{Ctrl+Space}{Ctrl+n}{Ctrl+w}{$dir}{Ctrl+y}{Up};
(Line Up | Lineup)     = moveLine(Up);
Line Down              = moveLine(Down);
(Line Up | Lineup) <n> = moveLine(Up_$2);
Line Down          <n> = moveLine(Down_$1);

<edit>      Here = {Ctrl+Space}{Ctrl+e} $1;
<edit> Back Here = {Ctrl+Space}{Ctrl+a} $1;
       Back Here = {Ctrl+Space}{Ctrl+a}{Ctrl+w};

<edit>      <n> Lines = {Ctrl+a}{Ctrl+Space}{Down_$2} $1;
<edit> Back <n> Lines = {Ctrl+a}{Ctrl+Space}{Up_$2}   $1;
       Back <n> Lines = {Ctrl+a}{Ctrl+Space}{Up_$1}   {Ctrl+w};
       Back     Line  = {Ctrl+a}{Ctrl+Space}{Up}      {Ctrl+w};

Replace Here = {Ctrl+k}{Ctrl+x}u{LeftButton}{Ctrl+y}{Ctrl+k}{LeftButton};
Get Here = {Ctrl+Space}{LeftButton}{Ctrl+k}{Ctrl+x}u{Ctrl+x_2}{Ctrl+y};
Put Here = {Ctrl+k}{Ctrl+x}u{LeftButton}{Ctrl+y}{LeftButton};

### Paragraphs

Graph (Start={ | End=}) <n> = {Esc} $2 {Esc} $1;
Fill Graph = {Alt+q};
Fill <n> Graphs = Repeat($1, {Alt+q}{Esc}} );
Fill Here = {LeftButton}{Alt+q};
Fill That = do(fill-region);
Join Graph = do(unfill-paragraph);
Join <n> Graphs = Repeat($1, do(unfill-paragraph) {Esc}} );
Join Graph Here = {LeftButton} do(unfill-paragraph);

<edit>      <n> Graphs = {Esc} $2 {Esc}} {Ctrl+Space}{Esc} $2 {Esc}{ $1;
<edit> Back <n> Graphs = {Esc} $2 {Esc}{ {Ctrl+Space}{Esc} $2 {Esc}} $1;

### Copy/Paste

Paste That = {Ctrl+y};
(Paste Here | Paster) = {LeftButton}{Ctrl+y};
Yank Again = {Alt+y};

Set Mark = {Ctrl+Space};
[Set] Mark Here = {LeftButton}{Ctrl+Space};
Switch Mark = {Ctrl+x}{Ctrl+x};

Copy To    <n> = do(copy-to-register) $1;
Paste From <n> = do(insert-register) $1 {Ctrl+x}{Ctrl+x};

(Kill=k | Paste=y | Clear=c | Open=o) Box = {Ctrl+x}r $1;
Copy Box = {Alt+J};

Replace All = "{Alt+<}{Ctrl+y}{Ctrl+Space}{Alt+>}{Ctrl+w}";
Copy and Switch        = {Ctrl+x}h{Alt+w} Wait(0) switchWindow();
Copy and Switch Buffer = {Ctrl+x}h{Alt+w} do(get-next-buffer);

### Indenting

indent(chars) := {Esc}$chars{Ctrl+x}{Ctrl+i};
Indent   <n> = indent($1);
Unindent <n> = indent(-$1);
Indent   <n> By <n> = {Ctrl+a}{Down_$1}{Ctrl+Space}{Up_$1} indent($2);
Unindent <n> By <n> = {Ctrl+a}{Down_$1}{Ctrl+Space}{Up_$1} indent(-$2);
Indent Region = do(indent-region);
Indent <n> Lines = {Ctrl+a}{Ctrl+Space}{Down_$1} do(indent-region);
Prefix Region = do(inleft);

# ---------------------------------------------------------------------------
# Navigation

### Scrolling
Center Cursor = {Ctrl+l};
Half (Up=backward | Down=forward) = do(scroll-window-$1-half);
Scroll (Up=down|Down=up) <n> = {Esc} $2 do(scroll-$1);

### Line Numbers
<digit> := 0..9;
Line Number = {Alt+Shift+g};
Line [Number] <digit> <digit>                 = {Alt+Shift+g}$1$2    {Enter};
Line [Number] <digit> <digit> <digit>         = {Alt+Shift+g}$1$2$3  {Enter};
Line [Number] <digit> <digit> <digit> <digit> = {Alt+Shift+g}$1$2$3$4{Enter};
What Line = do(what-line);

### Search/Replace
Find Down = {Ctrl+s};
Find Up   = {Ctrl+r};
Find Again [Down] = {Ctrl+s}{Ctrl+s};
Find Again  Up    = {Ctrl+r}{Ctrl+r};
Find Word [Down] = {Ctrl+s}{Ctrl+w}{Ctrl+s};
Find Word  Up    = {Ctrl+r}{Ctrl+w}{Ctrl+r};
Find That [Down] = {Ctrl+s} do(isearch-yank-kill);
Find That  Up    = {Ctrl+r} do(isearch-yank-kill);
Find <n> Words [Down] = {Ctrl+s}{Ctrl+w_$1}{Ctrl+s};
Find <n> Words  Up    = {Ctrl+r}{Ctrl+w_$1}{Ctrl+r};
Start Find <_anything> = "{Alt+<}{Ctrl+s}" $1;
Find Down <_anything> = {Ctrl+s} $1;
Find Up   <_anything> = {Ctrl+r} $1;
Replace String = do(replace-string);
Replace That = "{Alt+<}" do(replace-string) {Ctrl+y}{Enter}{Ctrl+y};
Remove That  = "{Alt+w}{Alt+<}" do(replace-string) {Ctrl+y}{Enter}{Enter};
Query Replace = {Esc}%;
Query [Replace] Regular [Expression] = do(query-replace-regexp);
Replace Regular [Expression] = do(replace-regexp);

### Bookmarks
Bookmark List = do(edit-bookmarks);
Set Bookmark <n> = {Ctrl+x}rm $1 {Enter};
Bookmark     <n> = {Ctrl+x}rb $1 {Enter};

### Search files
<filetypes> := ( Java      = *.java
               | See       = "*.cpp *.h"
               | A Ess Pea = "*.aspx *.ascx *.asax *.asp"
               | See Sharp = *.cs
               | Vocola    = "*.vcl *.vch"
               | Ex Ess El = "*.xsl"
               );
Find In Files = do(grep);
Find In <filetypes> Files = do(grep) {Ctrl+Space} " -i " $1 {Ctrl+x}{Ctrl+x};
Find Word In <filetypes> Files = {Ctrl+Space}{Alt+f}{Alt+w}
                                 do(grep) "{Ctrl+y} -i " $1 {Enter};
Find That In <filetypes> Files = do(grep) '"{Ctrl+y}" -i $1{Enter}';
Next Result = {Ctrl+x}`;

# ---------------------------------------------------------------------------
# Miscellaneous

Undo That = {Ctrl+x}u;
Undo <n> = Repeat($1, {Ctrl+x}u);
Abort That = {Ctrl+g};
Say (yes|no) = $1 {Enter};
Kill Space = {Esc}\;
One Space = {Alt+Space};
Kill (Blanks | Blank Lines) = do(delete-blank-lines);
(Sort|Keep|Flush) Lines = do($1-lines);
sort-backwards = {Esc}-1 do(sort-lines);
Hide <n> = {Esc} $1 "{Ctrl+x}$";
Macro (Start="(" | Stop=")" | Do=e) = {Ctrl+x} $1;
Macro (Do|Execute|Run) 1..5 = {Ctrl+u_$2}{Ctrl+x}e;

replaceOnLine(old,new) := {Ctrl+e}{Ctrl+Space}{Ctrl+a} do(narrow-to-region)
                          do(replace-string) $old{Enter}$new{Enter} do(widen) {Ctrl+l};
Backslash Line = replaceOnLine(/,\);
Slash Line     = replaceOnLine(\,/);

Kill Control Mikes = "{Alt+<}" do(replace-string)
                     "{Ctrl+q}{Ctrl+m}{Enter}{Enter}{Alt+<}";
(Kill|Convert|Fix) Tabs = {Ctrl+x}h do(untabify);

Mail That = {Ctrl+x}h{Alt+w} SwitchTo(thunderbird)
            {Ctrl+m} WaitForWindow(Compose) Wait(100)
            {Tab_2}{Ctrl+v}{Ctrl+Home}{Shift+Tab_2};

### Help
Help (Function=f | Variable=v | Key=k) = {Ctrl+h} $1;
Apropos <_anything> = do(apropos) $1;
(Apropos|Help Appropriate) = do(apropos);

# ---------------------------------------------------------------------------
# Programming in various languages

$if .cs | .cpp | .h | .grammar;
  Rebuild = {Ctrl+x}{Ctrl+s} SwitchTo(devenv) {Ctrl+Shift+F5};
$end

$if .vcl | .vch;
  Insert Wait = " Wait(100)";
  Use That = {Ctrl+x}{Ctrl+s} switchWindow();
  Comment Block 1..9   = {Ctrl+a}{Enter}{Up} "#|" {Down_$1}           {Ctrl+a}{Left}{Enter} "|#";
  Uncomment Block = {Ctrl+r} "#|" {Del_3}{Ctrl+s} "|#" {Left_3}{Del_3};
$end

### Comments

$if .vcl | .vch | .pl | .pm | .py;
  New Comment       = {Ctrl+e}        {Ctrl+j} "# ";
  New Comment Above = {Ctrl+a}{Ctrl+b}{Ctrl+j} "# ";
  Comment   <n> [Lines] = Repeat($1, "{Ctrl+a}#{Down}") {Up_$1};
  Uncomment <n> [Lines] = Repeat($1, {Ctrl+a}{Ctrl+d}{Down}) {Up_$1};

$elseif .css;
  Comment <n> [Lines] = Repeat($1, {Ctrl+a}/*{Ctrl+e}*/{Down}) {Up_$1};
  Uncomment <n> [Lines] =
      Repeat($1, {Ctrl+a}{Ctrl+d_2}{Ctrl+e}{Backspace_2}{Down}) {Up_$1};

$elseif .cs | .java | .cpp | .h | .js;
  New Comment       = {Ctrl+e}        {Ctrl+j} "// ";
  New Comment Above = {Ctrl+a}{Ctrl+b}{Ctrl+j} "// ";
        Doc Comment Here  =                 {Ctrl+j} "/// ";
  [New] Doc Comment       = {Ctrl+e}        {Ctrl+j} "/// ";
  [New] Doc Comment Above = {Ctrl+a}{Ctrl+b}{Ctrl+j} "/// ";
  Comment   <n> [Lines] = Repeat($1, {Ctrl+a}{Tab}//{Down}) {Up_$1}{Tab};
  Uncomment <n> [Lines] = Repeat($1, {Ctrl+a}{Tab}{Ctrl+d_2}{Down})
                          {Up_$1}{Tab};
  Comment Block 1..9   = {Ctrl+a}{Enter}{Up}/*{Left_2}{Esc}\ {Down_$1}           {Down}{Ctrl+a}{Enter}{Up}*/{Left_2}{Esc}\;
  Insert For =
      "{Ctrl+a}{Down}{Enter}{Up}for (int i = 0; i < n; i++){Enter} { {Enter_2} } {Enter}";
$end

### Perl

$if .pl | .pm | .py | .java | .cs | .cpp | .h | .js | .asp | .as;
  Statement (Start= | Down={Ctrl+n} | Up={Ctrl+p}) = $1 {Ctrl+a}{Ctrl+i};
  New Statement       = {Ctrl+e}        {Ctrl+j};
  New Statement Above = {Ctrl+a}{Ctrl+b}{Ctrl+j};
  New Block = {Ctrl+e}{Ctrl+j} { {Ctrl+j_2} } {Up}{Tab};
  New Block Demote = {Ctrl+e}{Ctrl+j} { {Left}{Backspace_4}{Right}{Ctrl+j_2} } {Up}{Tab};
  New Block 1..9 = {Ctrl+e}{Ctrl+j} { {Ctrl+Space}{Down_$1}{Ctrl+e}
                   do(indent-region){Ctrl+j} };
$end

$if .pl | .pm;
  Insert Hash Reference = "->{}{Ctrl+b}";
  Insert Subroutine = "{Enter}sub {Enter} { {Enter 2} } {Enter}{Ctrl+b 6}";
  Use That = {Ctrl+x}{Ctrl+s} do(shell) {Alt+p}{Enter};
$end

### Open related file

visitRelatedFile(ext) := elisp("(kill-new buffer-file-name)")
                         {Ctrl+x}{Ctrl+f}{Ctrl+y} $ext {Enter}
                         elisp("(rotate-yank-pointer 1)");

$if .cpp;
  (Open|Show) Header [File] = visitRelatedFile({Backspace_3}h);
$elseif .h;
  (Open|Show) See [File] = visitRelatedFile({Backspace}cpp);
$elseif .aspx | .ascx | .asmx;
  (Open|Show) (Code|See Sharp) = visitRelatedFile(.cs);
$end

### C#

$if .cs;
  (Open| Show) ASP [File] = visitRelatedFile({Backspace_3} " ");

  Insert (public|protected|private) Method =
      "{Enter}$1 void Do(type arg){Enter} { {Enter_2} } {Enter}{Up_2}{Tab}";
  Insert For Each = {Ctrl+a}{Down}{Enter}{Up}
                    "foreach (string s in list)"
                    {Enter} { {Enter_2} } {Enter};
  Insert (summary|returns|remarks|graph=para) = {End}{Ctrl+j} "/// <$1></$1>" {Alt+b}{Left_2};
  Insert Example = {End}{Ctrl+j}
                   '/// <example><code title="">{Enter}{Ctrl+y}{Up}{End}</code>{Right}'
                   do(inleft) '{Home}/// {Ctrl+k}{Enter}' do(indent-region)
                   '{Up}{End}{Enter}/// </example>'
                   {Ctrl+x}{Ctrl+x}{Up}{End}{Left_2};
  Insert Code Block = {End}{Ctrl+j}
                   '/// <code title="">{Enter}/// {Ctrl+y}{Backspace}</code>{Enter}/// '
                   {Ctrl+x}{Ctrl+x}{Up}{End}{Left_2};
  Insert (exclude) = {End}{Ctrl+j} "/// <$1/>";
  Insert (Param|Parameter) = {End}{Ctrl+j} '/// <param name=""></param>' {Alt+b}{Left_4};
  Insert Exception = {End}{Ctrl+j} '/// <exception cref="Pageflex.Scripting.Exceptions.Exception"></exception>' {Alt+b_2};
  Insert See Also = {End}{Ctrl+j} '/// <seealso cref=""/>'{Left_3};
  Insert See Overload = {End}{Ctrl+j} '/// <seealso cref="{Ctrl+y}">{Ctrl+y}</seealso>';

  Insert (Code=c) = " <$1></$1> " {Alt+b}{Left_2};
  Insert (Cross Reference=see) = ' <$1 cref=""/> ' {Left_4};
  Insert (Parameter Reference=paramref) = ' <$1 name=""/> ' {Left_4};
  Insert (True=true | False=false | Null Reference=null) = " <$1/> ";
  Cross Reference Word = {Alt+c}{Alt+b} '<see cref="' {Alt+f} '"/>';

  wrapLine() := {Ctrl+e}{Ctrl+Space}{Ctrl+a} do(narrow-to-region)
                do(replace-string) '"'{Enter}"'"{Enter} do(widen) {Ctrl+l}
                {Ctrl+a} 'root.AppendLine("' {Ctrl+e} '");' {Tab}{Right};
  Wrap Line = wrapLine();
  Wrap Line <n> = Repeat($1, wrapLine());
$end

# ---------------------------------------------------------------------------
# Other Context-Sensitive Commands

### Shell Commands
$if *shell*;
  Folder <folder> = "cd " $1 {Ctrl+a}{Alt+:}
                    '(replace-string "\\" "/"){Enter}{Enter}';
  Go Up = "cd ..{Enter}";
  Go Up <n> = "cd " Repeat($1, ../) {Enter};
  Command (Up=p | Down=n) = {Alt+$1};
  Command (Up=p | Down=n) Go = {Alt+$1}{Enter};
  Command (Up=p | Down=n) 1..30 = {Alt+$1_$2};
  Command (Up=p | Down=n) 1..30 Go = {Alt+$1_$2}{Enter};
$end

### Editing HTML files
$if .html | .htm | .aspx | .ascx | .asp | .css | .js;
  [Show] Preview Switch = {Ctrl+x}{Ctrl+s} SwitchTo(firefox) Wait(500) {F5} Wait(100) switchWindow();
  [Show] Preview [Wide] = {Ctrl+x}{Ctrl+s} SwitchTo(firefox) Wait(500) {F5};
  [Show] Preview I E  = {Ctrl+x}{Ctrl+s} SwitchTo(iexplore) Wait(500) {F5};
$end

$if .html | .htm | .aspx | .ascx | .asp | .xsl;
  Preview File = {Ctrl+x}{Ctrl+s} elisp("(kill-new buffer-file-name)")
                 SwitchTo(firefox) {Alt+d}file:///{Ctrl+v}{Enter}
                 Wait(100) switchWindow();
  New Statement       = {Ctrl+e}        {Ctrl+j};
  New Statement Above = {Ctrl+a}{Ctrl+b}{Ctrl+j};
 
  <htmlElement> := ( (Paragraph|Graph)=p | div | form
                   | table | Row=tr | Cell=td | Header=th
                   | Heading 1=h1 | Heading 2=h2 | Heading 3=h3 | Heading 4=h4
                   | List=ul | Ordered List=ol | Item=li
                   );
  <inlineElement> := span | Bold=b | Italic=i | Anchor=a | Break=br | No Break=nobr ;
  (Open= | Close=/) (<htmlElement> | <inlineElement>) = "<$1$2>{Tab}";
  Close Tag = {Ctrl+c}/;
  Insert <inlineElement> = "<$1></$1>" {Alt+b}{Left_2}{Tab};
  Insert <htmlElement> = {Ctrl+e}{Ctrl+j} "<$1></$1>" {Alt+b}{Left_2}{Tab};
  <htmlElement> Here   =                  "<$1></$1>" {Alt+b}{Left_2}{Tab};
  Insert Image = '<img src="">' {Left_2};
  Insert Anchor = '<a href=""></a>' {Left_6};
  Insert Target = '<a name="{Ctrl+y}"></a>' {Left_6};
  Insert Code = '<span class="code"></span>' {Left_7};
  Insert Pre = {Ctrl+e}{Ctrl+j} '<pre class="code"></pre>' {Left_6};
  Insert Comment = '<!--  -->' {Left_4};
  Open  Comment = '<!-- ';
  Close Comment = ' -->';
  Nonbreaking Space = "&nbsp;";
  Insert Em Dash = "&mdash;";
$end
