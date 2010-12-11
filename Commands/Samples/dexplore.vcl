# Sample commands for Microsoft Document Explorer

Show (Index=i | Contents=c | Favorites=p | Search=s) = {Alt+$1};
Show Topic = {Alt+w}2;

Go (Back=Left | Forward=Right)      = {Alt+$1};
Go (Back=Left | Forward=Right) 1..9 = Repeat($2, {Alt+$1} Wait(300));
