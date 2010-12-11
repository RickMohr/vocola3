# Sample commands for wordpad

Save File = {Ctrl+s};

Find That = {Ctrl+f}{Alt+f}{Esc};
Find Again = {F3};
Kill All = {Ctrl+a}{Ctrl+x};

Reply Here = "{Home}{Shift+End}{Del}{Enter 3}{Left 2}";

Plain Text = {Ctrl+a}{Alt+o}fArial{Tab}Regular{Tab}10{Enter};

Mail That = {Ctrl+a}{Ctrl+c} SwitchTo(thunderbird)
            {Ctrl+m} WaitForWindow(Compose) Wait(100)
            {Tab_2}{Ctrl+v}{Ctrl+Home}{Shift+Tab_2};
