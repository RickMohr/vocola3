use File::stat; # for mtime

$in_stats  = stat("Vocola.grammar");
$out_stats = stat("VocolaParser.cs");
$in_date  = $in_stats->mtime;
$out_date = $out_stats ? $out_stats->mtime : 0;
if ($in_date > $out_date) {
    print "Generating parser...\n";
    system 'java -jar "c:/Program Files/grammatica-1.4/lib/grammatica-1.4.jar" Vocola.grammar --csoutput . --csnamespace Vocola'
        and die "Error generating parser";
} else {
    print "Parser is up-to-date.\n";
}
