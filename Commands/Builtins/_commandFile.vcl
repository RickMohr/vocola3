# Open command files

$using Library.CommandFile;

<open> := Open | Edit;  # "Edit" for Vocola 2 users

<open> Commands                  = OpenCurrent();
<open> Global [Commands]         = OpenGlobal();
<open> Sample [Commands]         = OpenCurrentSamples();
<open> Sample Global [Commands]  = OpenGlobalSamples();
<open> Machine [Commands]        = OpenCurrentForMachine();
<open> Global Machine [Commands] = OpenGlobalForMachine();

<builtin> := ( User Interface = _ui.vcl         
             | Command File   = _commandFile.vcl
             | Dictation      = _dictation.vcl  
             | Correction     = Vocola.vcl      
             | Keystroke      = _keys.vcl       
             );

<header>  := ( Keys = keys.vch       
             );

Open <builtin> Commands = Open(GetBuiltinsPathname($1));
Open <header>  Header   = Open(GetBuiltinsPathname($1));

# Open most recently loaded command file with errors and move cursor to first error
Show Error = Open( GetErrorFilename() )
             {Ctrl+Home}
             {Down_  Eval( GetErrorLineNumber()   - 1) }
             {Right_ Eval( GetErrorColumnNumber() - 1) };
