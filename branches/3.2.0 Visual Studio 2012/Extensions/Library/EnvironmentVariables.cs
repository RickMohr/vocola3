using System;
using Vocola;

namespace Library
{

    /// <summary>Functions for accessing system environment variables.</summary>
    public class EnvironmentVariables : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // Get

        /// <summary>Returns the value of the specified system environment variable.</summary>
        /// <param name="variableName">Name of the environment variable to get. Case insensitive.</param>
        /// <returns>The value of the specified system environment variable.</returns>
        /// <example><code title="Include a machine-specific file">
        /// $include folders_ EnvironmentVariables.Get(COMPUTERNAME) .vch;</code>
        /// Here the value of the COMPUTERNAME environment variable is used to construct the name of
        /// a file to include. For example, if COMPUTERNAME has the value "venus" the resulting statement would be
        /// <c>$include folders_venus.vch;</c>. This statement could be used to define a different list of
        /// interesting folders on different machines.
        /// </example>
        [VocolaFunction]
        static public string Get(string variableName)
        {
            string value = Environment.GetEnvironmentVariable(variableName);
            if (value == null)
                throw new VocolaExtensionException("Environment variable '{0}' not found", variableName);
            return value;
        }
        
    }

}
