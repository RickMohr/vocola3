# Sample commands for thunderbird

onLoad() := Dictation.DisableForWindow(thunderbird, Thunderbird);

<n> := 1..99;
<addressBook> := ( Home = Personal Address Book
                 );
$include email.vch;

$if Compose;

  toCcBcc(n) := {Shift+Tab}{Down}{Home}{Down_$n}{Up}{Enter}{Tab};
  Address <emailAddress> = $1 {Enter};
  See See <emailAddress> = toCcBcc(2) $1{Enter};
  (To=1 | See See=2 | BCC=3) That = toCcBcc($1);
  Send That = {Ctrl+Enter} Wait(500) {Alt+Tab};
  Send [That] Later = {Ctrl+Shift+Enter} Wait(500) {Alt+Tab};
  I Meant Reply All = {Ctrl+a}{Ctrl+c}{Alt+F4} Wait(100) {Right}{Enter} Wait(100) {Ctrl+Shift+r};
  Kill Message = {Alt+F4} WaitForWindow(Save) D;
  BCC <n> = Repeat($1, {Tab_2}{Home}{Down_2}{Enter} );

  Kill Address = {Ctrl+a}{Del_2} Wait(0) {Home};

  Address Book <addressBook> = Touch(200,190) Wait(100) $1 Wait(100) {Enter};
  Address All = Touch(15,-90) Wait(100)
                {Home}{Shift+End}{ContextMenu}{Down_3}{Enter};

  ### Editing by Words
  [Kill] Back Word       = {Shift+Ctrl+Left}    {Del};
  [Kill] Back Forwards   = {Shift+Ctrl+Left_4}  {Del};   # "For Words"
  [Kill] Back <n> Words  = {Shift+Ctrl+Left_$1} {Del};
$end

$if Thunderbird;

  Get Messages     = {Ctrl+t};
  New Message      = {Ctrl+m};
  New Message <emailAddress> = {Ctrl+m} WaitForWindow(compose) $1 Wait(300) {Enter} Wait(300) {Enter};
  New Message HTML = MoveTo(110,83) {Shift+LeftButton};
  Reply to Message = {Ctrl+r};
  Reply to All     = {Ctrl+Shift+r};
  Forward Message  = {Ctrl+l};
  Forward As New   = {Ctrl+e};
  Print Message = {Ctrl+p}{Tab_6}1{Enter};
  Message Source = {Ctrl+u};
  Open Message = {ContextMenu}w;
  (Junk=j | No Junk=J) That = $1;
  Kill Junk = {Alt+t}l;
  Address Book = {Ctrl+2};

  ### Folders

  Folder (Inbox=1 | Drafts=3 | Sent=4)
    = {Ctrl+k}{Shift+F6}  # focus on folder pane
      {Home}{Down_$1} Wait(200) {F6};

  ### Navigating Messages

  summary() := {Ctrl+k}{F6_3}; # folders() {Tab_3};
  Final Message = summary() {Alt+v}se{End};
  First Unread  = summary() {Alt+v}se{End}mn;
  Next Unread = {End}n;
  Kill and Read = {Del} Wait(100) {End}n;
  Kill (1|Line) = {Del};
  Kill 1..10 [Lines] = Repeat($1, {Del} Wait(100));
  Mark All Read = {Ctrl+Shift+c};  
  Summary = summary();
  Summary <n> (Up|Down) = summary() {$2_$1};

  (Flag|Unflag) That = {Alt+m}ks{Down};
  First Flagged = {Home}{Alt+g}nf;
  (Next Flagged | Flag Down) = {Alt+g}nf;
  (Last Flagged | Flag Up) = {Alt+g}pf;
  Label None = {Alt+m}l0;
  
  (Show|Hide) Message [Pane] = {Alt+v}wm;
  Sort by (Date=e | Sender=n | Recipient=c | Subject=b | Thread=t | Size=z)
      = {Alt+v}s $1;
  Sort by Flag = summary() {Alt+v}ss{Alt+g}ps{Up_10}{Down_10};
  Center Cursor = summary() {Up_9}{Down_18}{Up_9};
  (Unthread That | Fix Summary) = {Alt+v}sh;
  Untag That = {Alt+m}g0;

  (Bigger=+ | Smaller=-) [Text] 1..4= {Ctrl+$1_$2};

  Filter <_anything> = {Ctrl+k} $1 Wait(1000) summary() {Alt+v}se{End};
  New Search = {Ctrl+k};
  Clear Results = {Ctrl+k}{Del}{F6};
  Search Messages = {Ctrl+Shift+F} Wait(100) {Tab_7}b{Tab_2};

  ### Attachments

  (Open=o Wait(1000) {Enter} | Save=a) Attachment = {Alt+f}a{Right} $1;
  (Open=o Wait(1000) {Enter} | Save=a) Attachment 1..5 = {Alt+f}a $2 $1;
  (Save|Detach) All [Attachments] = {Alt+f}as;
  Kill Attachments = {Alt+f}ae " " Wait(1000) {End};
$end
