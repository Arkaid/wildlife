using System.IO;
using UnityEngine;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Simple class to log debug messages to a text file
    /// </summary>
    public static class LogToFile 
    {
        // --- Events -----------------------------------------------------------------------------------
        // --- Constants --------------------------------------------------------------------------------
        // --- Static Properties ------------------------------------------------------------------------
        static string LogFile { get { return Application.persistentDataPath + "/log.txt"; } }

        static StreamWriter output;

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        static public void Activate()
        {
            output = new StreamWriter(LogFile);
            Application.logMessageReceived += OnLogMessageReceived;
        }

        // -----------------------------------------------------------------------------------
        static public void Deactivate()
        {
            output.Close();
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        // -----------------------------------------------------------------------------------
        private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            output.WriteLine(type.ToString().ToUpper() + ": " + condition);
            // only log the stack trace on warnings and errors
            if (type != LogType.Log)
            {
                foreach (string line in stackTrace.Split(new char[] { '\n' }))
                    output.WriteLine(line);
                output.WriteLine("");
            }
            output.Flush();
        }

        // --- Properties -------------------------------------------------------------------------------
        // --- Methods ----------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------	
    }
}