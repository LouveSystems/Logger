using System;
using System.Collections.Generic;
using System.Text;

namespace LouveSystems.Logging
{
    public static class StaticLogger
    {
        public static Logger singleLogger { private set; get; }

        // SINGLE LOGGER
        public static void Initialize(string programName, bool outputToFile = false, bool outputToConsole = true)
        {
            singleLogger = new Logger(programName, outputToFile, outputToConsole);
        }
        public static void Trace(params object[] msgs) { singleLogger.Trace(msgs); }
        public static void Debug(params object[] msgs) { singleLogger.Debug(msgs); }
        public static void Info(params object[] msgs) { singleLogger.Info(msgs); }
        public static void Warn(params object[] msgs) { singleLogger.Warn(msgs); }
        public static void Error(params object[] msgs) { singleLogger.Error(msgs); }
        public static void Fatal(Exception e)
        {
            Error(new string[1] { "================== FATAL ==================" });
            Error(e);
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
