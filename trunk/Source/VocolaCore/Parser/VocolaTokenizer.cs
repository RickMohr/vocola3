/*
 * VocolaTokenizer.cs
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
     * <remarks>A character stream tokenizer.</remarks>
     */
    internal class VocolaTokenizer : Tokenizer {

        /**
         * <summary>Creates a new tokenizer for the specified input
         * stream.</summary>
         * 
         * <param name='input'>the input stream to read</param>
         * 
         * <exception cref='ParserCreationException'>if the tokenizer
         * couldn't be initialized correctly</exception>
         */
        public VocolaTokenizer(TextReader input)
            : base(input) {

            CreatePatterns();
        }

        /**
         * <summary>Initializes the tokenizer by creating all the token
         * patterns.</summary>
         * 
         * <exception cref='ParserCreationException'>if the tokenizer
         * couldn't be initialized correctly</exception>
         */
        private void CreatePatterns() {
            TokenPattern  pattern;

            pattern = new TokenPattern((int) VocolaConstants.WHITESPACE,
                                       "WHITESPACE",
                                       TokenPattern.PatternType.REGEXP,
                                       "[ \\t\\n\\r]+");
            pattern.SetIgnore();
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.NAME,
                                       "NAME",
                                       TokenPattern.PatternType.REGEXP,
                                       "[a-zA-Z_]\\w*");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.VARIABLE,
                                       "VARIABLE",
                                       TokenPattern.PatternType.REGEXP,
                                       "<\\w+>");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.NAMEPAREN,
                                       "NAMEPAREN",
                                       TokenPattern.PatternType.REGEXP,
                                       "[a-zA-Z_]\\w*\\(");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.DOTTEDNAMEPAREN,
                                       "DOTTEDNAMEPAREN",
                                       TokenPattern.PatternType.REGEXP,
                                       "([a-zA-Z_]\\w*\\.)*[a-zA-Z_]\\w*\\(");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.REFERENCE,
                                       "REFERENCE",
                                       TokenPattern.PatternType.REGEXP,
                                       "\\$([a-zA-Z_]\\w*|\\d+)");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.RANGE,
                                       "RANGE",
                                       TokenPattern.PatternType.REGEXP,
                                       "\\d+\\.\\.\\d+");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.CHARS,
                                       "CHARS",
                                       TokenPattern.PatternType.REGEXP,
                                       "[^\"'#=;|\\[\\]()<>, \\t\\n\\r]+");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.QUOTED_CHARS,
                                       "QUOTED_CHARS",
                                       TokenPattern.PatternType.REGEXP,
                                       "\"([^\"\\n\\r]|\"\")*\"|'([^'\\n\\r]|'')*'");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.COMMENT_MULTI,
                                       "COMMENT_MULTI",
                                       TokenPattern.PatternType.REGEXP,
                                       "#\\|(.|[\\n\\r])*?\\|#");
            pattern.SetIgnore();
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.COMMENT,
                                       "COMMENT",
                                       TokenPattern.PatternType.REGEXP,
                                       "#.*");
            pattern.SetIgnore();
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.EQUALS,
                                       "EQUALS",
                                       TokenPattern.PatternType.STRING,
                                       "=");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.ASSIGN,
                                       "ASSIGN",
                                       TokenPattern.PatternType.STRING,
                                       ":=");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.STOP,
                                       "STOP",
                                       TokenPattern.PatternType.STRING,
                                       ";");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.INCLUDE_T,
                                       "INCLUDE_T",
                                       TokenPattern.PatternType.STRING,
                                       "$include");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.USING_T,
                                       "USING_T",
                                       TokenPattern.PatternType.STRING,
                                       "$using");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.IF_T,
                                       "IF_T",
                                       TokenPattern.PatternType.STRING,
                                       "$if");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.ELSEIF_T,
                                       "ELSEIF_T",
                                       TokenPattern.PatternType.STRING,
                                       "$elseif");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.ELSE_T,
                                       "ELSE_T",
                                       TokenPattern.PatternType.STRING,
                                       "$else");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.ENDIF_T,
                                       "ENDIF_T",
                                       TokenPattern.PatternType.STRING,
                                       "$end");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.OR,
                                       "OR",
                                       TokenPattern.PatternType.STRING,
                                       "|");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.BRACKET_L,
                                       "BRACKET_L",
                                       TokenPattern.PatternType.STRING,
                                       "[");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.BRACKET_R,
                                       "BRACKET_R",
                                       TokenPattern.PatternType.STRING,
                                       "]");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.PAREN_L,
                                       "PAREN_L",
                                       TokenPattern.PatternType.STRING,
                                       "(");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.PAREN_R,
                                       "PAREN_R",
                                       TokenPattern.PatternType.STRING,
                                       ")");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.ANGLE_L,
                                       "ANGLE_L",
                                       TokenPattern.PatternType.STRING,
                                       "<");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.ANGLE_R,
                                       "ANGLE_R",
                                       TokenPattern.PatternType.STRING,
                                       ">");
            AddPattern(pattern);

            pattern = new TokenPattern((int) VocolaConstants.COMMA,
                                       "COMMA",
                                       TokenPattern.PatternType.STRING,
                                       ",");
            AddPattern(pattern);
        }
    }
}
