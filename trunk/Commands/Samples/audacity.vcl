# Sample commands for audacity

touchAndReturn(x,y) := SavePoint() Touch($x,$y) MoveToSavedPoint();

play() := touchAndReturn(322, 70);
stop() := touchAndReturn(476, 70);

Play That = play();
Stop That = stop();
Play Here = {LeftButton} stop() Wait(500) play();

Kill Back = {Shift+Home}{Del};
Kill Back Here = {LeftButton} stop() {Shift+Home} Wait(500) {Del};

Export That = {Alt+f}ee{Enter};
Export Back = {Shift+Home}{Alt+f}e Wait(100) e{Enter};

(Fit In Window | Show All) = {Ctrl+f};