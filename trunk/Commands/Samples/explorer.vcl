# Sample commands for Windows Explorer

### View

Buffer (Start={Home} | End={End}) = $1;
Refresh [View] = {Alt+v}r;
(Show|View) (Details=d | List=l | Thumbnails=h) = {Alt+v} $2;
Search = {Ctrl+e};
Address    = {Alt+d};
Left Side  = {Alt+d}{Tab_2} MoveTo(0,0);
Right Side = {Alt+d}{Shift+Tab}{Left} MoveTo(0,0);
Go (Back=Left | Forward=Right) = {Alt+$1};
Go (Back=Left | Forward=Right) 1..10 = {Alt+$1_$2};
(Copy={Ctrl+c} | Paste={Ctrl+v}) Address = {Alt+d} $1;

### Folders

$include folders.vch;

Folder <folder> = {Alt+d}$1{Enter} MoveTo(0,0);

New Folder = {Alt+f}w{Enter};
Expand That   = {Alt+NumKey+};
Collapse That = {Alt+NumKey-};
Share That = {Alt+f}r Wait(1000) {Tab_5}{Right_2}{Alt+s}{Enter};

Remember Folder = {Alt+d}{Ctrl+c}{Tab_2}
                  CommandFile.Open(folders.vch) WaitForWindow(Emacs)
                  '{Ctrl+s});{Ctrl+a}{Ctrl+o}= "{Ctrl+y}"{Ctrl+a}{Tab}| ';

Map Network Drive = {Alt+t}n;

### Files

Copy Filename = {F2}{Shift+End}{Ctrl+c}{Alt+d}{End}\{Ctrl+v}
                {Shift+Home}{Ctrl+c}{Ctrl+z}{Shift+Tab_2};
Copy Folder Name = {Alt+d}{Ctrl+c}{Shift+Tab_2};
Copy Leaf Name = {F2}{Shift+End}{Ctrl+c}{Esc};

(Relay | Send to) Emacs = {Alt+f}ne;

Duplicate That = {Ctrl+c}{Left}{Ctrl+v}c;
Rename (That="" | Start={Home} | End={End}) = {F2} $1;

(Show|Edit) Properties = {Alt+f}r;
[Toggle] Read Only = {Alt+f}r Wait(2000) {Alt+r}{Enter};
Open With = {ContextMenu}h{Right};

$if "Windows Photo Gallery";
  $include WindowsPhotoGallery.vcl;
$end