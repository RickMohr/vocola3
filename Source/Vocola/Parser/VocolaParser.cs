/*
 * VocolaParser.cs
 * 
 * THIS FILE HAS BEEN GENERATED AUTOMATICALLY. DO NOT EDIT!
 * 
 * Permission is granted to copy this document verbatim in any
 * medium, provided that this copyright notice is left intact.
 * 
 * Copyright (c) 2008 Rick Mohr. All rights reserved.
 */

using System.IO;

using PerCederberg.Grammatica.Parser;

namespace Vocola {

    /**
     * <remarks>A token stream parser.</remarks>
     */
    internal class VocolaParser : RecursiveDescentParser {

        /**
         * <summary>An enumeration with the generated production node
         * identity constants.</summary>
         */
        private enum SynteticPatterns {
            SUBPRODUCTION_1 = 3001,
            SUBPRODUCTION_2 = 3002,
            SUBPRODUCTION_3 = 3003,
            SUBPRODUCTION_4 = 3004,
            SUBPRODUCTION_5 = 3005,
            SUBPRODUCTION_6 = 3006,
            SUBPRODUCTION_7 = 3007,
            SUBPRODUCTION_8 = 3008,
            SUBPRODUCTION_9 = 3009,
            SUBPRODUCTION_10 = 3010,
            SUBPRODUCTION_11 = 3011,
            SUBPRODUCTION_12 = 3012,
            SUBPRODUCTION_13 = 3013
        }

        /**
         * <summary>Creates a new parser.</summary>
         * 
         * <param name='input'>the input stream to read from</param>
         * 
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public VocolaParser(TextReader input)
            : base(new VocolaTokenizer(input)) {

            CreatePatterns();
        }

        /**
         * <summary>Creates a new parser.</summary>
         * 
         * <param name='input'>the input stream to read from</param>
         * 
         * <param name='analyzer'>the analyzer to parse with</param>
         * 
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public VocolaParser(TextReader input, Analyzer analyzer)
            : base(new VocolaTokenizer(input), analyzer) {

            CreatePatterns();
        }

        /**
         * <summary>Initializes the parser by creating all the production
         * patterns.</summary>
         * 
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        private void CreatePatterns() {
            ProductionPattern             pattern;
            ProductionPatternAlternative  alt;

            pattern = new ProductionPattern((int) VocolaConstants.FILE,
                                            "file");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.USING, 0, -1);
            alt.AddProduction((int) VocolaConstants.STATEMENTS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.USING,
                                            "using");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.USING_T, 1, 1);
            alt.AddProduction((int) VocolaConstants.WORDS, 1, 1);
            alt.AddToken((int) VocolaConstants.STOP, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.STATEMENTS,
                                            "statements");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_1, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.INCLUDE,
                                            "include");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.INCLUDE_T, 1, 1);
            alt.AddProduction((int) VocolaConstants.ACTIONS, 1, 1);
            alt.AddToken((int) VocolaConstants.STOP, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.CONTEXT,
                                            "context");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.IF_T, 1, 1);
            alt.AddProduction((int) VocolaConstants.IFBODY, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_2, 0, -1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_3, 0, 1);
            alt.AddToken((int) VocolaConstants.ENDIF_T, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.IFBODY,
                                            "ifbody");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.WORDS, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_4, 0, -1);
            alt.AddToken((int) VocolaConstants.STOP, 1, 1);
            alt.AddProduction((int) VocolaConstants.STATEMENTS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.DEFINITION,
                                            "definition");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.VARIABLE, 1, 1);
            alt.AddToken((int) VocolaConstants.ASSIGN, 1, 1);
            alt.AddProduction((int) VocolaConstants.MENU_BODY, 1, 1);
            alt.AddToken((int) VocolaConstants.STOP, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.FUNCTION,
                                            "function");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.PROTOTYPE, 1, 1);
            alt.AddToken((int) VocolaConstants.ASSIGN, 1, 1);
            alt.AddProduction((int) VocolaConstants.ACTIONS, 1, 1);
            alt.AddToken((int) VocolaConstants.STOP, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.PROTOTYPE,
                                            "prototype");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.NAMEPAREN, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_6, 0, 1);
            alt.AddToken((int) VocolaConstants.PAREN_R, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.COMMAND,
                                            "command");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.TERMS, 1, 1);
            alt.AddToken((int) VocolaConstants.EQUALS, 1, 1);
            alt.AddProduction((int) VocolaConstants.ACTIONS, 1, 1);
            alt.AddToken((int) VocolaConstants.STOP, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.TERMS,
                                            "terms");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_7, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.TERM,
                                            "term");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.WORDS, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.VARIABLE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.RANGE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.MENU, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.OPTIONAL_WORDS,
                                            "optionalWords");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.BRACKET_L, 1, 1);
            alt.AddProduction((int) VocolaConstants.WORDS, 1, 1);
            alt.AddToken((int) VocolaConstants.BRACKET_R, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.MENU,
                                            "menu");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.PAREN_L, 1, 1);
            alt.AddProduction((int) VocolaConstants.MENU_BODY, 1, 1);
            alt.AddToken((int) VocolaConstants.PAREN_R, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.MENU_BODY,
                                            "menuBody");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.ALTERNATIVE, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_8, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.ALTERNATIVE,
                                            "alternative");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.TERMS, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_9, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.WORDS,
                                            "words");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.WORD, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.WORD,
                                            "word");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_10, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.ACTIONS,
                                            "actions");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.ACTION, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.ACTION,
                                            "action");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.WORDS, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.REFERENCE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.CALL, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) VocolaConstants.CALL,
                                            "call");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_11, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_13, 0, 1);
            alt.AddToken((int) VocolaConstants.PAREN_R, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_1,
                                            "Subproduction1");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.INCLUDE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.CONTEXT, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.DEFINITION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.FUNCTION, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.COMMAND, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_2,
                                            "Subproduction2");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.ELSEIF_T, 1, 1);
            alt.AddProduction((int) VocolaConstants.IFBODY, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_3,
                                            "Subproduction3");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.ELSE_T, 1, 1);
            alt.AddProduction((int) VocolaConstants.STATEMENTS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_4,
                                            "Subproduction4");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.OR, 1, 1);
            alt.AddProduction((int) VocolaConstants.WORDS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_5,
                                            "Subproduction5");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.COMMA, 1, 1);
            alt.AddToken((int) VocolaConstants.NAME, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_6,
                                            "Subproduction6");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.NAME, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_5, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_7,
                                            "Subproduction7");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.TERM, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.OPTIONAL_WORDS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_8,
                                            "Subproduction8");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.OR, 1, 1);
            alt.AddProduction((int) VocolaConstants.ALTERNATIVE, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_9,
                                            "Subproduction9");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.EQUALS, 1, 1);
            alt.AddProduction((int) VocolaConstants.ACTIONS, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_10,
                                            "Subproduction10");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.QUOTED_CHARS, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.NAME, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.CHARS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_11,
                                            "Subproduction11");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.DOTTEDNAMEPAREN, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.NAMEPAREN, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_12,
                                            "Subproduction12");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) VocolaConstants.COMMA, 1, 1);
            alt.AddProduction((int) VocolaConstants.ACTIONS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_13,
                                            "Subproduction13");
            pattern.SetSyntetic(true);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) VocolaConstants.ACTIONS, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_12, 0, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);
        }
    }
}
