# Insert keystrokes using Vocola syntax

$using Library;

$if .vcl | .vch;
  $include CommandFile.GetBuiltinsPathname(keys.vch);
  <1to99> := 1..99;

  Insert                             <key> = Main.InsertText({$1});
  Insert               <modifierKey> <key> = Main.InsertText({$1+$2});
  Insert <modifierKey> <modifierKey> <key> = Main.InsertText({$1+$2+$3});
  Insert                             <key> <1to99> = Main.InsertText({$1_$2});
  Insert               <modifierKey> <key> <1to99> = Main.InsertText({$1+$2_$3});
  Insert <modifierKey> <modifierKey> <key> <1to99> = Main.InsertText({$1+$2+$3_$4});
$end
