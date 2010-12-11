# Voice commands for Vocola

# Choose alternatives in the Vocola correction panel
$if Correction;
  <n> := 0..20;
  Select <n> = {Tab}{Home}{Down_$1}{Shift+Tab}{End};
  Choose <n> = {Tab}{Home}{Down_$1}{Enter};
  <n> OK     = {Tab}{Home}{Down_$1}{Enter};
$end
