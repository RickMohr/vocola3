# Sample commands for iTunes

onLoad() := Dictation.DisableForWindow(iTunes, iTunes);

getInfo() := {Alt+f}g;  # {Ctrl+i} doesn't work

New Search = Touch(-46,42) {Shift+Home};
Clear Results = Touch(-36,40);
Filter <_anything> = Touch(-46,42) {Shift+Home} $1 {Enter};

Get Info = getInfo();
Copy Artist = getInfo() {Alt+r}{Ctrl+c}{Esc};

setSortArtistToCompilation() := getInfo() Wait(1000) {Alt+a}Compilations{Enter};
This Compilation = setSortArtistToCompilation();
Compilation 1..99 = Repeat($1, setSortArtistToCompilation() {Down});


