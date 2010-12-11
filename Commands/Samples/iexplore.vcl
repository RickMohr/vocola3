# Sample commands for Internet Explorer

$include browser.vch;

Open File = {Ctrl+o}{Alt+r};
View Source = {Alt+v}c;
Copy Source = {Alt+v}c Wait(1000) {Ctrl+a}{Ctrl+c}{Alt+f}x SwitchTo(emacs);
Set [As] Home Page = {Alt+t}o{Alt+c}{Enter};
Recent Page = {Alt+v}o;

(Show|Hide|Kill) (Favorites|Bookmarks) = {Ctrl+i};
Proxy Server = {Alt+t}o{Shift+Tab}{Right_4}{Alt+l}{Tab+2};
(Show|Hide) Status Bar = {Alt+v}b;
Internet Options = {Alt+t}o;
Small Text = {Alt+v}xs;

Touch New = {LeftButton}{Down_2}{Enter};

(Go Somewhere | Page List) = Touch(87,52);

### Tabs

<n> := 1..9;
Tab <n>       = Touch(125,176) Wait(100) {Down_$1}{Enter};
Tab <n> Right = Touch(125,176) Wait(100) {Up_$1}{Enter};

### Editing Favorites

Kill (That|1) = {Del} Wait(500) {Enter};
Rename That = {ContextMenu}m;
