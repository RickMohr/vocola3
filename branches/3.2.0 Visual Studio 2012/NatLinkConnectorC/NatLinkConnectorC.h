// NatLinkConnectorC.h

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Windows::Forms;

// Python can easily call a function exported from a DLL.
// C# can't export functions, so this thin C++ file does the job.
// These functions call C# methods (easier to read) to do the work.

static bool Initialized = false;
static wchar_t VocolaExecutionFolder[1000];
static int (*fpEmulateRecognize)(const wchar_t* words);
static int (*fpSendKeys)(const wchar_t* keys);

Assembly^ CurrentDomain_AssemblyResolve(Object^ sender, ResolveEventArgs^ args)
{
    try
    {
        String^ shortAssemblyName = args->Name->Substring(0, args->Name->IndexOf(','));
        String^ fileName = Path::Combine(gcnew String(VocolaExecutionFolder), shortAssemblyName + gcnew String(L".dll"));
        //String^ projectDir = gcnew System::String(L"C:\\Users\\Rick\\Rick\\Technical\\Vocola3\\OpenSource\\GoogleCode 3.1\\Source\\VocolaCore\\bin\\Debug");
        //String^ fileName = Path::Combine(projectDir, shortAssemblyName + gcnew System::String(L".dll"));
        if (File::Exists(fileName))
        {
            Assembly^ result = Assembly::LoadFrom(fileName);
            return result;
        }
        else
            return (Assembly::GetExecutingAssembly()->FullName == args->Name)
            ? Assembly::GetExecutingAssembly()
            : nullptr;
    }
    catch (Exception^ ex)
    {
        MessageBox::Show(ex->Message);
        return nullptr;
    }
}

bool ReallyInitializeConnection()
{
	return Vocola::NatLinkToVocolaClient::InitializeConnection();
}


extern "C" 
{

	// Initialization

	int __declspec(dllexport) __stdcall InitializeConnection(const wchar_t* vocolaExecutionFolder)
	{
		try
		{
			if (Initialized)
				return 1;
			wcscpy_s(VocolaExecutionFolder, vocolaExecutionFolder);

			// The executing AppDomain's "CodeBase" is the Python executable folder, so we have to specify how to find assemblies
			AppDomain::CurrentDomain->AssemblyResolve += gcnew ResolveEventHandler(CurrentDomain_AssemblyResolve);

			// If InitializeConnection() isn't called in a separate function the assembly load fails before the resolver is established
			Initialized = ReallyInitializeConnection();
			return (Initialized ? 1 : 0);
		}
		catch (Exception^)
		{
			//MessageBox::Show(ex->Message);
		}
	}

	void __declspec(dllexport) __stdcall SetCallbacks(
		int (*fpNatLinkEmulateRecognize)(const wchar_t* words),
		int (*fpNatLinkSendKeys)(const wchar_t* keys))
	{
		fpEmulateRecognize = fpNatLinkEmulateRecognize;
		fpSendKeys = fpNatLinkSendKeys;
	}

	// NatLink to Vocola

	int __declspec(dllexport) __stdcall HaveAnyGrammarFilesChanged()
	{
		return Vocola::NatLinkToVocolaClient::HaveAnyGrammarFilesChanged();
	}

	void __declspec(dllexport) __stdcall RunActions(const wchar_t* commandId, const wchar_t* variableWords)
	{
		Vocola::NatLinkToVocolaClient::RunActions(gcnew String(commandId), gcnew String(variableWords));
	}

	void __declspec(dllexport) __stdcall LogMessage(int level, const wchar_t* message)
	{
		Vocola::NatLinkToVocolaClient::LogMessage(level, gcnew String(message));
	}

	// Vocola to NatLink

	int __declspec(dllexport) NatLinkEmulateRecognize(const wchar_t* words)
	{
		return fpEmulateRecognize(words);
	}

	int __declspec(dllexport) NatLinkSendKeys(const wchar_t* keys)
	{
		return fpSendKeys(keys);
	}
}
