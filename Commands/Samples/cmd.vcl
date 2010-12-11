# Sample commands for cmd (Command Prompt)

$include folders.vch;

Folder <folder> = 'cd "$1"{Enter}';

windowMenu(keys) :=  Touch(20,20) $keys;

Copy That = {Enter};
Mark That    = windowMenu(ek);
Paste That   = windowMenu(ep);
Close Window = windowMenu(c);

Go Up = "cd ..{Enter}";
Go Up 1..20 = "cd " Repeat($1, ..\) {Enter};
