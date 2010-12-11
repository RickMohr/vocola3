# Sample commands for Firefox

$include browser.vch;

Show ((Bookmarks|Favorites)=b | History=h) = {Ctrl+$1};
View Source = {Ctrl+u};
(Go Somewhere|Page List) = Touch(79,76);

Page Down = {PgDn}{Up_5};
Page Up   = {PgUp}{Down_5};

Find <_anything> = {Ctrl+f} $1;

Touch New = {RightButton} Wait(300) t;

### Tabs
<n> := 1..9;
Tab <n>       = Touch(-20, 145) Wait(100) {Down_$1}{Enter};
Tab <n> Right = Touch(-20, 145) Wait(100) {Up_$1}{Enter};
Duplicate (Tab|That) = {Alt+d}{Ctrl+c}{Ctrl+t}{Ctrl+v}{Enter}{Ctrl+Shift+Tab};

### Bookmarks
Bookmark That = {Ctrl+d};
Touch Rename = {RightButton}{Up}{Enter};

### Firebug
[Close] [Open] Firebug = {Alt+t} Wait(100) f{Enter};
(Inspect Element | Touch Inspect | Inspect Here) = {RightButton}{Up}{Enter};
