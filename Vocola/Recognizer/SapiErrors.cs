using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Vocola
{

    public class SapiErrors
    {
        static Dictionary<uint, string> Errors;

        public static string GetErrorMessage(uint hResult)
        {
            if (Errors == null)
                Initialize();
            return Errors[hResult];
        }

        public static string GetErrorMessage(string messageContainingHResult)
        {
            // Parse message "Exception from HRESULT: 0x80045003"
            string message = messageContainingHResult;
            int index = messageContainingHResult.IndexOf("0x");
            if (index != -1)
            {
                string code = messageContainingHResult.Substring(index + 2);
                uint hResult;
                if (UInt32.TryParse(code, NumberStyles.AllowHexSpecifier, null, out hResult))
                    message = GetErrorMessage(hResult);
            }
            return message;
        }

        static void Initialize()
        {
            Errors = new Dictionary<uint, string>();
            Errors[0x80045001] = "The object has not been properly initialized.";  // SPERR_UNINITIALIZED
            Errors[0x80045002] = "The object has already been initialized.";  // SPERR_ALREADY_INITIALIZED
            Errors[0x80045003] = "The caller has specified an unsupported format.";  // SPERR_UNSUPPORTED_FORMAT
            Errors[0x80045004] = "The caller has specified invalid flags for this operation.";  // SPERR_INVALID_FLAGS
            Errors[0x00045005] = "The operation has reached the end of stream.";  // SP_END_OF_STREAM
            Errors[0x80045006] = "The wave device is busy.";  // SPERR_DEVICE_BUSY
            Errors[0x80045007] = "The wave device is not supported.";  // SPERR_DEVICE_NOT_SUPPORTED
            Errors[0x80045008] = "The wave device is not enabled.";  // SPERR_DEVICE_NOT_ENABLED
            Errors[0x80045009] = "There is no wave driver installed.";  // SPERR_NO_DRIVER
            Errors[0x8004500a] = "The file must be Unicode.";  // SPERR_FILEMUSTBEUNICODE
            Errors[0x0004500b] = "Insufficient data";  // SP_INSUFFICIENTDATA
            Errors[0x0004500c] = "The phrase ID specified does not exist or is out of range.";  // SPERR_INVALID_PHRASE_ID
            Errors[0x8004500d] = "The caller provided a buffer too small to return a result.";  // SPERR_BUFFER_TOO_SMALL
            Errors[0x8004500e] = "Caller did not specify a format prior to opening a stream.";  // SPERR_FORMAT_NOT_SPECIFIED
            Errors[0x8004500f] = "The stream I/O was stopped by setting the audio object to the stopped state. This will be returned for both read and write streams.";  // SPERR_AUDIO_STOPPED
            Errors[0x00045010] = "This will be returned only on input (read) streams when the stream is paused. Reads on paused streams will not block, and this return code indicates that all of the data has been removed from the stream.";  // SP_AUDIO_PAUSED
            Errors[0x80045011] = "Invalid rule name passed to ActivateGrammar.";  // SPERR_RULE_NOT_FOUND
            Errors[0x80045012] = "An exception was raised during a call to the current TTS driver.";  // SPERR_TTS_ENGINE_EXCEPTION
            Errors[0x80045013] = "An exception was raised during a call to an application sentence filter.";  // SPERR_TTS_NLP_EXCEPTION
            Errors[0x80045014] = "In speech recognition, the current method cannot be performed while a grammar rule is active.";  // SPERR_ENGINE_BUSY
            Errors[0x00045015] = "The operation was successful, but only with automatic stream format conversion.";  // SP_AUDIO_CONVERSION_ENABLED
            Errors[0x00045016] = "There is currently no hypothesis recognition available.";  // SP_NO_HYPOTHESIS_AVAILABLE
            Errors[0x80045017] = "Cannot create a new object instance for the specified object category.";  // SPERR_CANT_CREATE
            Errors[0x00045018] = "The word, pronunciation, or POS pair being added is already in lexicon.";  // SP_ALREADY_IN_LEX
            Errors[0x80045019] = "The word does not exist in the lexicon.";  // SPERR_NOT_IN_LEX
            Errors[0x0004501a] = "The client is currently synced with the lexicon.";  // SP_LEX_NOTHING_TO_SYNC
            Errors[0x8004501b] = "The client is excessively out of sync with the lexicon. Mismatches may not sync incrementally.";  // SPERR_LEX_VERY_OUT_OF_SYNC
            Errors[0x8004501c] = "A rule reference in a grammar was made to a named rule that was never defined.";  // SPERR_UNDEFINED_FORWARD_RULE_REF
            Errors[0x8004501d] = "A non-dynamic grammar rule that has no body.";  // SPERR_EMPTY_RULE
            Errors[0x8004501e] = "The grammar compiler failed due to an internal state error.";  // SPERR_GRAMMAR_COMPILER_INTERNAL_ERROR
            Errors[0x8004501f] = "An attempt was made to modify a non-dynamic rule.";  // SPERR_RULE_NOT_DYNAMIC
            Errors[0x80045020] = "A rule name was duplicated.";  // SPERR_DUPLICATE_RULE_NAME
            Errors[0x80045021] = "A resource name was duplicated for a given rule.";  // SPERR_DUPLICATE_RESOURCE_NAME
            Errors[0x80045022] = "Too many grammars have been loaded.";  // SPERR_TOO_MANY_GRAMMARS
            Errors[0x80045023] = "Circular reference in import rules of grammars.";  // SPERR_CIRCULAR_REFERENCE
            Errors[0x80045024] = "A rule reference to an imported grammar that could not be resolved.";  // SPERR_INVALID_IMPORT
            Errors[0x80045025] = "The format of the WAV file is not supported.";  // SPERR_INVALID_WAV_FILE
            Errors[0x00045026] = "This success code indicates that an SR method called with the SPRIF_ASYNC flag is being processed. When it has finished processing, an SPFEI_ASYNC_COMPLETED event will be generated.";  // SP_REQUEST_PENDING
            Errors[0x80045027] = "A grammar rule was defined with a null path through the rule. That is, it is possible to satisfy the rule conditions with no words.";  // SPERR_ALL_WORDS_OPTIONAL
            Errors[0x80045028] = "It is not possible to change the current engine or input. This occurs in the following cases: 1) SelectEngine called while a recognition context exists, or 2) SetInput called in the shared instance case.";  // SPERR_INSTANCE_CHANGE_INVALID
            Errors[0x80045029] = "A rule exists with matching IDs (names) but different names (IDs).";  // SPERR_RULE_NAME_ID_CONFLICT
            Errors[0x8004502a] = "A grammar contains no top-level, dynamic, or exported rules. There is no possible way to activate or otherwise use any rule in this grammar.";  // SPERR_NO_RULES
            Errors[0x8004502b] = "Rule 'A' refers to a second rule 'B' which, in turn, refers to rule 'A'.";  // SPERR_CIRCULAR_RULE_REF
            Errors[0x0004502c] = "Parse path cannot be parsed given the currently active rules.";  // SP_NO_PARSE_FOUND
            Errors[0x8004502d] = "Parse path cannot be parsed given the currently active rules.";  // SPERR_NO_PARSE_FOUND
            Errors[0x8004502e] = "A marshaled remote call failed to respond.";  // SPERR_REMOTE_CALL_TIMED_OUT
            Errors[0x8004502f] = "This will only be returned on input (read) streams when the stream is paused because the SR driver has not retrieved data recently.";  // SPERR_AUDIO_BUFFER_OVERFLOW
            Errors[0x80045030] = "The result does not contain any audio, nor does the portion of the element chain of the result contain any audio.";  // SPERR_NO_AUDIO_DATA
            Errors[0x80045031] = "This alternate is no longer a valid alternate to the result it was obtained from. Returned from ISpPhraseAlt methods.";  // SPERR_DEAD_ALTERNATE
            Errors[0x80045032] = "The result does not contain any audio, nor does the portion of the element chain of the result contain any audio. Returned from ISpResult::GetAudio and ISpResult::SpeakAudio.";  // SPERR_HIGH_LOW_CONFIDENCE
            Errors[0x80045033] = "The XML format string for this RULEREF is invalid, e.g. not a GUID or REFCLSID.";  // SPERR_INVALID_FORMAT_STRING
            Errors[0x00045034] = "The operation is not supported for stream input.";  // SP_UNSUPPORTED_ON_STREAM_INPUT
            Errors[0x80045035] = "The operation is invalid for all but newly created application lexicons.";  // SPERR_APPLEX_READ_ONLY
            Errors[0x80045036] = "No terminating rule path.";  // SPERR_NO_TERMINATING_RULE_PATH
            Errors[0x80045037] = "The word exists but without pronunciation.";  // SP_WORD_EXISTS_WITHOUT_PRONUNCIATION
            Errors[0x80045038] = "An operation was attempted on a stream object that has been closed.";  // SPERR_STREAM_CLOSED
            Errors[0x80045039] = "When enumerating items, the requested index is greater than the count of items.";  // SPERR_NO_MORE_ITEMS
            Errors[0x8004503a] = "The requested data item (data key, value, etc.) was not found.";  // SPERR_NOT_FOUND
            Errors[0x8004503b] = "Audio state passed to SetState() is invalid.";  // SPERR_INVALID_AUDIO_STATE
            Errors[0x8004503c] = "A generic MMSYS error not caught by _MMRESULT_TO_HRESULT.";  // SPERR_GENERIC_MMSYS_ERROR
            Errors[0x8004503d] = "An exception was raised during a call to the marshaling code.";  // SPERR_MARSHALER_EXCEPTION
            Errors[0x8004503e] = "Attempt was made to manipulate a non-dynamic grammar.";  // SPERR_NOT_DYNAMIC_GRAMMAR
            Errors[0x8004503f] = "Cannot add ambiguous property.";  // SPERR_AMBIGUOUS_PROPERTY
            Errors[0x80045040] = "The key specified is invalid.";  // SPERR_INVALID_REGISTRY_KEY
            Errors[0x80045041] = "The token specified is invalid.";  // SPERR_INVALID_TOKEN_ID
            Errors[0x80045042] = "The xml parser failed due to bad syntax.";  // SPERR_XML_BAD_SYNTAX
            Errors[0x80045043] = "The xml parser failed to load a required resource (e.g., voice, phoneconverter, etc.).";  // SPERR_XML_RESOURCE_NOT_FOUND
            Errors[0x80045044] = "Attempted to remove registry data from a token that is already in use elsewhere.";  // SPERR_TOKEN_IN_USE
            Errors[0x80045045] = "Attempted to perform an action on an object token that has had associated registry key deleted.";  // SPERR_TOKEN_DELETED
            Errors[0x80045046] = "The selected voice was registered as multi-lingual. SAPI does not support multi-lingual registration.";  // SPERR_MULTI_LINGUAL_NOT_SUPPORTED
            Errors[0x80045047] = "Exported rules cannot refer directly or indirectly to a dynamic rule.";  // SPERR_EXPORT_DYNAMIC_RULE
            Errors[0x80045048] = "Error parsing the SAPI Text Grammar Format (XML grammar).";  // SPERR_STGF_ERROR
            Errors[0x80045049] = "Incorrect word format, probably due to incorrect pronunciation string.";  // SPERR_WORDFORMAT_ERROR
            Errors[0x8004504a] = "Methods associated with active audio stream cannot be called unless stream is active.";  // SPERR_STREAM_NOT_ACTIVE
            Errors[0x8004504b] = "Arguments or data supplied by the engine are in an invalid format or are inconsistent.";  // SPERR_ENGINE_RESPONSE_INVALID
            Errors[0x8004504c] = "An exception was raised during a call to the current SR engine.";  // SPERR_SR_ENGINE_EXCEPTION
            Errors[0x8004504d] = "Stream position information supplied from engine is inconsistent.";  // SPERR_STREAM_POS_INVALID
            Errors[0x0004504e] = "Operation could not be completed because the recognizer is inactive. It is inactive either because the recognition state is currently inactive or because no rules are active.";  // SP_RECOGNIZER_INACTIVE
            Errors[0x8004504f] = "When making a remote call to the server, the call was made on the wrong thread.";  // SPERR_REMOTE_CALL_ON_WRONG_THREAD
            Errors[0x80045050] = "The remote process terminated unexpectedly.";  // SPERR_REMOTE_PROCESS_TERMINATED
            Errors[0x80045051] = "The remote process is already running; it cannot be started a second time.";  // SPERR_REMOTE_PROCESS_ALREADY_RUNNING
            Errors[0x80045052] = "An attempt to load a CFG grammar with a LANGID different than other loaded grammars.";  // SPERR_LANGID_MISMATCH
            Errors[0x00045053] = "A grammar-ending parse has been found that does not use all available words.";  // SP_PARTIAL_PARSE_FOUND
            Errors[0x80045054] = "An attempt to deactivate or activate a non top-level rule.";  // SPERR_NOT_TOPLEVEL_RULE
            Errors[0x00045055] = "An attempt to parse when no rule was active.";  // SP_NO_RULE_ACTIVE
            Errors[0x80045056] = "An attempt to ask a container lexicon for all words at once.";  // SPERR_LEX_REQUIRES_COOKIE
            Errors[0x00045057] = "An attempt to activate a rule/dictation/etc without calling SetInput first in the InProc case.";  // SP_STREAM_UNINITIALIZED
            Errors[0x80045059] = "The requested language is not supported.";  // SPERR_UNSUPPORTED_LANG
            Errors[0x8004505a] = "The operation cannot be performed because the voice is currently paused.";  // SPERR_VOICE_PAUSED
            Errors[0x8004505b] = "This will only be returned on input (read) streams when the real time audio device stops returning data for a long period of time.";  // SPERR_AUDIO_BUFFER_UNDERFLOW
            Errors[0x8004505c] = "An audio device stopped returning data from the Read() method even though it was in the run state. This error is only returned in the END_SR_STREAM event.";  // SPERR_AUDIO_STOPPED_UNEXPECTEDLY
            Errors[0x8004505d] = "The SR engine is unable to add this word to a grammar. The application may need to supply an explicit pronunciation for this word.";  // SPERR_NO_WORD_PRONUNCIATION
            Errors[0x8004505e] = "An attempt to call ScaleAudio on a recognition result having previously called GetAlternates. Allowing the call to succeed would result in the previously created alternates located in incorrect audio stream positions.";  // SPERR_ALTERNATES_WOULD_BE_INCONSISTENT
            Errors[0x8004505f] = "The method called is not supported for the shared recognizer. For example, ISpRecognizer::GetInputStream().";  // SPERR_NOT_SUPPORTED_FOR_SHARED_RECOGNIZER
            Errors[0x80045060] = "A task could not complete because the SR engine had timed out.";  // SPERR_TIMEOUT
            Errors[0x80045061] = "An SR engine called synchronize while inside of a synchronize call.";  // SPERR_REENTER_SYNCHRONIZE
            Errors[0x80045062] = "The grammar contains a node no arcs.";  // SPERR_STATE_WITH_NO_ARCS
            Errors[0x80045063] = "Neither audio output nor input is supported for non-active console sessions.";  // SPERR_NOT_ACTIVE_SESSION
            Errors[0x80045064] = "The object is a stale reference and is invalid to use. For example, having an ISpeechGrammarRule object reference and then calling ISpeechRecoGrammar::Reset() will cause the rule object to be invalidated. Calling any methods after this will result in this error.";  // SPERR_ALREADY_DELETED
            Errors[0x00045065] = "This can be returned from Read or Write calls for audio streams when the stream is stopped.";  // SP_AUDIO_STOPPED
            Errors[0x80045066] = "The Recognition Parse Tree could not be generated. For example, a rule name begins with a digit but the XML parser does not allow an element name beginning with a digit.";  // SPERR_RECOXML_GENERATION_FAIL
            Errors[0x80045067] = "The SML could not be generated. For example, the transformation xslt template is not well formed.";  // SPERR_SML_GENERATION_FAIL
            Errors[0x80045068] = "The SML could not be generated. For example, the transformation xslt template is not well formed.";  // SPERR_NOT_PROMPT_VOICE
            Errors[0x80045069] = "There is already a root rule for this grammar. Defining another root rule will fail.";  // SPERR_ROOTRULE_ALREADY_DEFINED
            Errors[0x80045070] = "Support for embedded script not supported because browser security settings have disabled it.";  // SPERR_SCRIPT_DISALLOWED
            Errors[0x80045071] = "A time out occurred starting the sapi server.";  // SPERR_REMOTE_CALL_TIMED_OUT_START
            Errors[0x80045072] = "A timeout occurred obtaining the lock for starting or connecting to sapi server.";  // SPERR_REMOTE_CALL_TIMED_OUT_CONNECT
            Errors[0x80045073] = "When there is a cfg grammar loaded, changing the security manager is not permitted.";  // SPERR_SECMGR_CHANGE_NOT_ALLOWED
            Errors[0x00045074] = "Parse is valid but could be extendable (internal use only).";  // SP_COMPLETE_BUT_EXTENDABLE
            Errors[0x80045075] = "Tried and failed to delete an existing file.";  // SPERR_FAILED_TO_DELETE_FILE
            Errors[0x80045076] = "The user has chosen to disable speech from running on the machine, or the system is not set up to run speech (for example, initial setup and tutorial has not been run).";  // SPERR_SHARED_ENGINE_DISABLED
            Errors[0x80045077] = "No recognizer is installed.";  // SPERR_RECOGNIZER_NOT_FOUND
            Errors[0x80045078] = "No audio device is installed.";  // SPERR_AUDIO_NOT_FOUND
            Errors[0x80045079] = "No vowel in a word.";  // SPERR_NO_VOWEL
            Errors[0x8004507A] = "No vowel in a word.";  // SPERR_UNSUPPORTED_PHONEME
            Errors[0x0004507B] = "The grammar does not have any root or top-level active rules to activate.";  // SP_NO_RULES_TO_ACTIVATE
            Errors[0x0004507C] = "The engine does not need SAPI word entry handles for this grammar.";  // SP_NO_WORDENTRY_NOTIFICATION
            Errors[0x8004507D] = "The word passed to the GetPronunciations interface needs normalizing first.";  // SPERR_WORD_NEEDS_NORMALIZATION
            Errors[0x8004507E] = "The word passed to the normalize interface cannot be normalized.";  // SPERR_CANNOT_NORMALIZE
            Errors[0x80045080] = "This combination of function call and input is currently not supported."; // S_NOTSUPPORTED
        }

    }

}
