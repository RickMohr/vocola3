/*
 * VocolaAnalyzer.cs
 * 
 * THIS FILE HAS BEEN GENERATED AUTOMATICALLY. DO NOT EDIT!
 * 
 * Permission is granted to copy this document verbatim in any
 * medium, provided that this copyright notice is left intact.
 * 
 * Copyright (c) 2008 Rick Mohr. All rights reserved.
 */

using PerCederberg.Grammatica.Parser;

namespace Vocola {

    /**
     * <remarks>A class providing callback methods for the
     * parser.</remarks>
     */
    internal abstract class VocolaAnalyzer : Analyzer {

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public override void Enter(Node node) {
            switch (node.GetId()) {
            case (int) VocolaConstants.NAME:
                EnterName((Token) node);
                break;
            case (int) VocolaConstants.VARIABLE:
                EnterVariable((Token) node);
                break;
            case (int) VocolaConstants.NAMEPAREN:
                EnterNameparen((Token) node);
                break;
            case (int) VocolaConstants.DOTTEDNAMEPAREN:
                EnterDottednameparen((Token) node);
                break;
            case (int) VocolaConstants.REFERENCE:
                EnterReference((Token) node);
                break;
            case (int) VocolaConstants.RANGE:
                EnterRange((Token) node);
                break;
            case (int) VocolaConstants.CHARS:
                EnterChars((Token) node);
                break;
            case (int) VocolaConstants.QUOTED_CHARS:
                EnterQuotedChars((Token) node);
                break;
            case (int) VocolaConstants.EQUALS:
                EnterEquals((Token) node);
                break;
            case (int) VocolaConstants.ASSIGN:
                EnterAssign((Token) node);
                break;
            case (int) VocolaConstants.STOP:
                EnterStop((Token) node);
                break;
            case (int) VocolaConstants.INCLUDE_T:
                EnterIncludeT((Token) node);
                break;
            case (int) VocolaConstants.USING_T:
                EnterUsingT((Token) node);
                break;
            case (int) VocolaConstants.IF_T:
                EnterIfT((Token) node);
                break;
            case (int) VocolaConstants.ELSEIF_T:
                EnterElseifT((Token) node);
                break;
            case (int) VocolaConstants.ELSE_T:
                EnterElseT((Token) node);
                break;
            case (int) VocolaConstants.ENDIF_T:
                EnterEndifT((Token) node);
                break;
            case (int) VocolaConstants.OR:
                EnterOr((Token) node);
                break;
            case (int) VocolaConstants.BRACKET_L:
                EnterBracketL((Token) node);
                break;
            case (int) VocolaConstants.BRACKET_R:
                EnterBracketR((Token) node);
                break;
            case (int) VocolaConstants.PAREN_L:
                EnterParenL((Token) node);
                break;
            case (int) VocolaConstants.PAREN_R:
                EnterParenR((Token) node);
                break;
            case (int) VocolaConstants.ANGLE_L:
                EnterAngleL((Token) node);
                break;
            case (int) VocolaConstants.ANGLE_R:
                EnterAngleR((Token) node);
                break;
            case (int) VocolaConstants.COMMA:
                EnterComma((Token) node);
                break;
            case (int) VocolaConstants.FILE:
                EnterFile((Production) node);
                break;
            case (int) VocolaConstants.USING:
                EnterUsing((Production) node);
                break;
            case (int) VocolaConstants.STATEMENTS:
                EnterStatements((Production) node);
                break;
            case (int) VocolaConstants.INCLUDE:
                EnterInclude((Production) node);
                break;
            case (int) VocolaConstants.CONTEXT:
                EnterContext((Production) node);
                break;
            case (int) VocolaConstants.IFBODY:
                EnterIfbody((Production) node);
                break;
            case (int) VocolaConstants.DEFINITION:
                EnterDefinition((Production) node);
                break;
            case (int) VocolaConstants.FUNCTION:
                EnterFunction((Production) node);
                break;
            case (int) VocolaConstants.PROTOTYPE:
                EnterPrototype((Production) node);
                break;
            case (int) VocolaConstants.COMMAND:
                EnterCommand((Production) node);
                break;
            case (int) VocolaConstants.TERMS:
                EnterTerms((Production) node);
                break;
            case (int) VocolaConstants.TERM:
                EnterTerm((Production) node);
                break;
            case (int) VocolaConstants.OPTIONAL_WORDS:
                EnterOptionalWords((Production) node);
                break;
            case (int) VocolaConstants.MENU:
                EnterMenu((Production) node);
                break;
            case (int) VocolaConstants.MENU_BODY:
                EnterMenuBody((Production) node);
                break;
            case (int) VocolaConstants.ALTERNATIVE:
                EnterAlternative((Production) node);
                break;
            case (int) VocolaConstants.WORDS:
                EnterWords((Production) node);
                break;
            case (int) VocolaConstants.WORD:
                EnterWord((Production) node);
                break;
            case (int) VocolaConstants.ACTIONS:
                EnterActions((Production) node);
                break;
            case (int) VocolaConstants.ACTION:
                EnterAction((Production) node);
                break;
            case (int) VocolaConstants.CALL:
                EnterCall((Production) node);
                break;
            }
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public override Node Exit(Node node) {
            switch (node.GetId()) {
            case (int) VocolaConstants.NAME:
                return ExitName((Token) node);
            case (int) VocolaConstants.VARIABLE:
                return ExitVariable((Token) node);
            case (int) VocolaConstants.NAMEPAREN:
                return ExitNameparen((Token) node);
            case (int) VocolaConstants.DOTTEDNAMEPAREN:
                return ExitDottednameparen((Token) node);
            case (int) VocolaConstants.REFERENCE:
                return ExitReference((Token) node);
            case (int) VocolaConstants.RANGE:
                return ExitRange((Token) node);
            case (int) VocolaConstants.CHARS:
                return ExitChars((Token) node);
            case (int) VocolaConstants.QUOTED_CHARS:
                return ExitQuotedChars((Token) node);
            case (int) VocolaConstants.EQUALS:
                return ExitEquals((Token) node);
            case (int) VocolaConstants.ASSIGN:
                return ExitAssign((Token) node);
            case (int) VocolaConstants.STOP:
                return ExitStop((Token) node);
            case (int) VocolaConstants.INCLUDE_T:
                return ExitIncludeT((Token) node);
            case (int) VocolaConstants.USING_T:
                return ExitUsingT((Token) node);
            case (int) VocolaConstants.IF_T:
                return ExitIfT((Token) node);
            case (int) VocolaConstants.ELSEIF_T:
                return ExitElseifT((Token) node);
            case (int) VocolaConstants.ELSE_T:
                return ExitElseT((Token) node);
            case (int) VocolaConstants.ENDIF_T:
                return ExitEndifT((Token) node);
            case (int) VocolaConstants.OR:
                return ExitOr((Token) node);
            case (int) VocolaConstants.BRACKET_L:
                return ExitBracketL((Token) node);
            case (int) VocolaConstants.BRACKET_R:
                return ExitBracketR((Token) node);
            case (int) VocolaConstants.PAREN_L:
                return ExitParenL((Token) node);
            case (int) VocolaConstants.PAREN_R:
                return ExitParenR((Token) node);
            case (int) VocolaConstants.ANGLE_L:
                return ExitAngleL((Token) node);
            case (int) VocolaConstants.ANGLE_R:
                return ExitAngleR((Token) node);
            case (int) VocolaConstants.COMMA:
                return ExitComma((Token) node);
            case (int) VocolaConstants.FILE:
                return ExitFile((Production) node);
            case (int) VocolaConstants.USING:
                return ExitUsing((Production) node);
            case (int) VocolaConstants.STATEMENTS:
                return ExitStatements((Production) node);
            case (int) VocolaConstants.INCLUDE:
                return ExitInclude((Production) node);
            case (int) VocolaConstants.CONTEXT:
                return ExitContext((Production) node);
            case (int) VocolaConstants.IFBODY:
                return ExitIfbody((Production) node);
            case (int) VocolaConstants.DEFINITION:
                return ExitDefinition((Production) node);
            case (int) VocolaConstants.FUNCTION:
                return ExitFunction((Production) node);
            case (int) VocolaConstants.PROTOTYPE:
                return ExitPrototype((Production) node);
            case (int) VocolaConstants.COMMAND:
                return ExitCommand((Production) node);
            case (int) VocolaConstants.TERMS:
                return ExitTerms((Production) node);
            case (int) VocolaConstants.TERM:
                return ExitTerm((Production) node);
            case (int) VocolaConstants.OPTIONAL_WORDS:
                return ExitOptionalWords((Production) node);
            case (int) VocolaConstants.MENU:
                return ExitMenu((Production) node);
            case (int) VocolaConstants.MENU_BODY:
                return ExitMenuBody((Production) node);
            case (int) VocolaConstants.ALTERNATIVE:
                return ExitAlternative((Production) node);
            case (int) VocolaConstants.WORDS:
                return ExitWords((Production) node);
            case (int) VocolaConstants.WORD:
                return ExitWord((Production) node);
            case (int) VocolaConstants.ACTIONS:
                return ExitActions((Production) node);
            case (int) VocolaConstants.ACTION:
                return ExitAction((Production) node);
            case (int) VocolaConstants.CALL:
                return ExitCall((Production) node);
            }
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public override void Child(Production node, Node child) {
            switch (node.GetId()) {
            case (int) VocolaConstants.FILE:
                ChildFile(node, child);
                break;
            case (int) VocolaConstants.USING:
                ChildUsing(node, child);
                break;
            case (int) VocolaConstants.STATEMENTS:
                ChildStatements(node, child);
                break;
            case (int) VocolaConstants.INCLUDE:
                ChildInclude(node, child);
                break;
            case (int) VocolaConstants.CONTEXT:
                ChildContext(node, child);
                break;
            case (int) VocolaConstants.IFBODY:
                ChildIfbody(node, child);
                break;
            case (int) VocolaConstants.DEFINITION:
                ChildDefinition(node, child);
                break;
            case (int) VocolaConstants.FUNCTION:
                ChildFunction(node, child);
                break;
            case (int) VocolaConstants.PROTOTYPE:
                ChildPrototype(node, child);
                break;
            case (int) VocolaConstants.COMMAND:
                ChildCommand(node, child);
                break;
            case (int) VocolaConstants.TERMS:
                ChildTerms(node, child);
                break;
            case (int) VocolaConstants.TERM:
                ChildTerm(node, child);
                break;
            case (int) VocolaConstants.OPTIONAL_WORDS:
                ChildOptionalWords(node, child);
                break;
            case (int) VocolaConstants.MENU:
                ChildMenu(node, child);
                break;
            case (int) VocolaConstants.MENU_BODY:
                ChildMenuBody(node, child);
                break;
            case (int) VocolaConstants.ALTERNATIVE:
                ChildAlternative(node, child);
                break;
            case (int) VocolaConstants.WORDS:
                ChildWords(node, child);
                break;
            case (int) VocolaConstants.WORD:
                ChildWord(node, child);
                break;
            case (int) VocolaConstants.ACTIONS:
                ChildActions(node, child);
                break;
            case (int) VocolaConstants.ACTION:
                ChildAction(node, child);
                break;
            case (int) VocolaConstants.CALL:
                ChildCall(node, child);
                break;
            }
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterName(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitName(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterVariable(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitVariable(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterNameparen(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitNameparen(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDottednameparen(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDottednameparen(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterReference(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitReference(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterRange(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitRange(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterChars(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitChars(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterQuotedChars(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitQuotedChars(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterEquals(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitEquals(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAssign(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAssign(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterStop(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitStop(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIncludeT(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIncludeT(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterUsingT(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitUsingT(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIfT(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIfT(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterElseifT(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitElseifT(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterElseT(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitElseT(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterEndifT(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitEndifT(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterOr(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitOr(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBracketL(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBracketL(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterBracketR(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitBracketR(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterParenL(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitParenL(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterParenR(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitParenR(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAngleL(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAngleL(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAngleR(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAngleR(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterComma(Token node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitComma(Token node) {
            return node;
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterFile(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitFile(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildFile(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterUsing(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitUsing(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildUsing(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterStatements(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitStatements(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildStatements(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterInclude(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitInclude(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildInclude(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterContext(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitContext(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildContext(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterIfbody(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitIfbody(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildIfbody(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterDefinition(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitDefinition(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildDefinition(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterFunction(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitFunction(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildFunction(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterPrototype(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitPrototype(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildPrototype(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterCommand(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitCommand(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildCommand(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTerms(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTerms(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildTerms(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterTerm(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitTerm(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildTerm(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterOptionalWords(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitOptionalWords(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildOptionalWords(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMenu(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMenu(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildMenu(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterMenuBody(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitMenuBody(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildMenuBody(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAlternative(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAlternative(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildAlternative(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterWords(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitWords(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildWords(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterWord(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitWord(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildWord(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterActions(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitActions(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildActions(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterAction(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitAction(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildAction(Production node, Node child) {
            node.AddChild(child);
        }

        /**
         * <summary>Called when entering a parse tree node.</summary>
         * 
         * <param name='node'>the node being entered</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void EnterCall(Production node) {
        }

        /**
         * <summary>Called when exiting a parse tree node.</summary>
         * 
         * <param name='node'>the node being exited</param>
         * 
         * <returns>the node to add to the parse tree, or
         *          null if no parse tree should be created</returns>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual Node ExitCall(Production node) {
            return node;
        }

        /**
         * <summary>Called when adding a child to a parse tree
         * node.</summary>
         * 
         * <param name='node'>the parent node</param>
         * <param name='child'>the child node, or null</param>
         * 
         * <exception cref='ParseException'>if the node analysis
         * discovered errors</exception>
         */
        public virtual void ChildCall(Production node, Node child) {
            node.AddChild(child);
        }
    }
}
