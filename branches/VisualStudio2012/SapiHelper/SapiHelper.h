#pragma once

#include <sphelper.h>
#include <Vcclr.h>  // PtrToStringChars
//#include "alternates_h.h"  // ISpDisplayAlternates

#define MAX_TOKENS 200
#define MAX_ACOUSTIC_ALTERNATES 100
#define MAX_DISPLAY_ALTERNATES 10

namespace Vocola
{

    public ref class SapiHelper
    {
    public:

        static void CommitText(SpeechLib::ISpeechRecoResult^ iSpeechResult,
                               ULONG startElement, ULONG nElements,
                               System::String^ text,
                               System::Boolean addWordsToLexicon)
        {
            // Get an ISpRecoResult2 from our ISpeechRecoResult
            HRESULT hr;
            System::IntPtr pUnk = System::Runtime::InteropServices::Marshal::GetIUnknownForObject(iSpeechResult);
            IUnknown *cpUnk = (IUnknown *)pUnk.ToPointer();
            CComQIPtr<ISpRecoResult2> result2 = cpUnk;

            pin_ptr<const wchar_t> cText = PtrToStringChars(text);

            hr = result2->CommitText(startElement, nElements, cText,
                                     addWordsToLexicon ? SPCF_ADD_TO_USER_LEXICON : SPCF_DEFINITE_CORRECTION);
            CheckHResult(hr);
        }

        // MS didn't expose in SpeechLib the ability to get a "display"
        // version of alternates for the correction panel (e.g. to
        // convert "two dollars" to "$2") so we get to do things the
        // hard way here. The key functionality is in the undocumented
        // interface ISpDisplayAlternates, available from
        // ISpRecoContext. (See alternates.h)

        static int GetAlternateStrings(SpeechLib::ISpeechRecoResult^ iSpeechResult,
                                       ULONG maxAlternates,
                                       System::Boolean capitalizeAlternates,
                                       System::Collections::Generic::List<System::String^>^ alternateStrings)
        {
            // Get an ISpRecoResult from our ISpeechRecoResult
            HRESULT hr;
            System::IntPtr pUnk = System::Runtime::InteropServices::Marshal::GetIUnknownForObject(iSpeechResult);
            IUnknown *cpUnk = (IUnknown *)pUnk.ToPointer();
            CComQIPtr<ISpRecoResult> result = cpUnk;

            // Get current RecoContext
            CComPtr<ISpRecoContext> context;
            hr = result->GetRecoContext(&context);
            CheckHResult(hr);

            // Get an ISpDisplayAlternates from our RecoContext
            CComQIPtr<ISpDisplayAlternates> alternatesGetter = context;
  
            // Add "case flip" display alternate for recognized phrase
            CComQIPtr<ISpPhrase> resultPhrase = result;
            int nCaseFlipDisplayAlternatesOfRecognizedPhrase =
                AddDisplayAlternates(alternatesGetter, alternateStrings, resultPhrase, FALSE, !capitalizeAlternates);
  
            // Add all display alternates for recognized phrase
            //System::Diagnostics::Debug::WriteLine(gcnew System::String(L"------ Display alternates of recognized phrase:"));
            int nDisplayAlternatesOfRecognizedPhrase = nCaseFlipDisplayAlternatesOfRecognizedPhrase +
                AddDisplayAlternates(alternatesGetter, alternateStrings, resultPhrase, TRUE, capitalizeAlternates);

            // Get acoustic alternates
            ISpPhraseAlt* alternates[MAX_ACOUSTIC_ALTERNATES];
            ULONG maxAcousticAlternates = maxAlternates - nDisplayAlternatesOfRecognizedPhrase + 1;
            if (maxAcousticAlternates > MAX_ACOUSTIC_ALTERNATES)
                maxAcousticAlternates = MAX_ACOUSTIC_ALTERNATES;
            ULONG nAlternates;
            hr = result->GetAlternates(0, SPPR_ALL_ELEMENTS, maxAcousticAlternates, alternates, &nAlternates);
            CheckHResult(hr);

            // Add one display alternate for each acoustic alternate.
            for (ULONG i = 1; i < nAlternates; i++)
            {
                //System::Diagnostics::Debug::WriteLine(gcnew System::String(L"------ Display alternates of acoustic alternate:"));
                CComPtr<ISpPhraseAlt> alternate;
                alternate.p = alternates[i];
                CComQIPtr<ISpPhrase> alternatePhrase = alternate;
                AddDisplayAlternates(alternatesGetter, alternateStrings, alternatePhrase, FALSE, capitalizeAlternates);
            }

            // Return number of display alternates for recognized phrase
            System::Runtime::InteropServices::Marshal::Release(pUnk);
            return nDisplayAlternatesOfRecognizedPhrase;
        }
 
    private:

        static System::String^ OneSpace  = gcnew System::String(L" ");
        static System::String^ TwoSpaces = gcnew System::String(L"  ");

        static int AddDisplayAlternates(CComQIPtr<ISpDisplayAlternates> alternatesGetter,
                                        System::Collections::Generic::List<System::String^>^ alternateStrings,
                                        CComQIPtr<ISpPhrase> cpPhrase,
                                        bool addAll,
                                        System::Boolean capitalizeAlternates)
        {
            // Get words from SPPHRASE
            HRESULT hr;
            SPPHRASE* pPhrase;
            hr = cpPhrase->GetPhrase(&pPhrase);
            CheckHResult(hr);
            const SPPHRASEELEMENT* words = pPhrase->pElements;
            ULONG iFirstWord = pPhrase->Rule.ulFirstElement;
            ULONG nWords     = pPhrase->Rule.ulCountOfElements;

            // Make SPDISPLAYTOKENs for phrase words, allowing for two extra "delimiter" tokens
            int nTokens = nWords + 2;
            if (nTokens > MAX_TOKENS)
                nTokens = MAX_TOKENS;
            SPDISPLAYTOKEN tokens[MAX_TOKENS];
            // Set "." or "a" as context token to control alternates capitalization
            if (capitalizeAlternates)
                SetPeriodToken(&tokens[0]);
            else
                SetAToken(&tokens[0]);
            // Add word tokens
            for (ULONG i = 0; i < nWords; i++)
                SetTokenText(&tokens[i+1], words[i]);
            // Add end token
            SetAToken(&tokens[nWords+1]);

            // Make an SPDISPLAYPHRASE for those SPDISPLAYTOKENs
            SPDISPLAYPHRASE phrase;
            phrase.ulNumTokens = nTokens;
            phrase.pTokens = &tokens[0];

            // Get the display alternates
            SPDISPLAYPHRASE* alternates[MAX_DISPLAY_ALTERNATES];
            ULONG nRequested = (addAll ? MAX_DISPLAY_ALTERNATES : 1);
            ULONG nAlternates;
            hr = alternatesGetter->GetDisplayAlternates(&phrase, nRequested, alternates, &nAlternates);
            int nAdded = 0;
            if (SUCCEEDED(hr))  // mysterious failure on 64-bit systems with e.g. "caps hello"
            {
                for (ULONG i = 0; i < nAlternates; i++)
                {
                    // Build a string for this display alternate
                    System::Text::StringBuilder^ sb = gcnew System::Text::StringBuilder();
                    SPDISPLAYTOKEN* tokens = alternates[i]->pTokens;
                    System::String^ word = nullptr;
                    for (int j = 1; j < nTokens - 1; j++)
                    {
                        // Append the word
                        SPDISPLAYTOKEN token = tokens[j];
                        System::String^ word = gcnew System::String(token.pszDisplay);
                        sb->Append(word);

                        // Append any trailing spaces
                        if (j == nTokens - 2 || tokens[j+1].bDisplayAttributes & SPAF_CONSUME_LEADING_SPACES)
                            continue;
                        else if (token.bDisplayAttributes & SPAF_ONE_TRAILING_SPACE)
                            sb->Append(OneSpace);
                        else if (token.bDisplayAttributes & SPAF_TWO_TRAILING_SPACES)
                            sb->Append(TwoSpaces);
                    }
                    // Add to collection, avoiding duplicates
                    System::String^ text = sb->ToString()->TrimEnd();
                    //System::Diagnostics::Debug::WriteLine(text);
                    bool matchesRecognizedPhrase  = (                               text->Equals(alternateStrings[0]));
                    bool matchesDisplayAlternate1 = (alternateStrings->Count > 1 && text->Equals(alternateStrings[1]));
                    if (!matchesRecognizedPhrase && !matchesDisplayAlternate1)
                    {
                        alternateStrings->Add(text);
                        nAdded++;
                    }
                }
            }
            ::CoTaskMemFree(pPhrase);
            return nAdded;
        }

        static void SetTokenText(SPDISPLAYTOKEN* pToken, SPPHRASEELEMENT word)
        {
            pToken->pszLexical = word.pszLexicalForm;
            pToken->pszDisplay = word.pszDisplayText;
            pToken->bDisplayAttributes = word.bDisplayAttributes;
        }

        static void SetPeriodToken(SPDISPLAYTOKEN* pToken)
        {
            pToken->pszLexical = L"period";
            pToken->pszDisplay = L".";
            pToken->bDisplayAttributes = SPAF_ONE_TRAILING_SPACE;
        }

        static void SetAToken(SPDISPLAYTOKEN* pToken)
        {
            pToken->pszLexical = L"a";
            pToken->pszDisplay = L"a";
            pToken->bDisplayAttributes = SPAF_ONE_TRAILING_SPACE;
        }

        static void SetNullToken(SPDISPLAYTOKEN* pToken)
        {
            pToken->pszLexical = L"";
            pToken->pszDisplay = L"";
            pToken->bDisplayAttributes = 0;
        }

        static void CheckHResult(HRESULT hr)
        {
            if (FAILED(hr))
                throw gcnew System::Exception(System::String::Format(L"COM exception 0x{0:X8}", hr));
        }

    };

}
