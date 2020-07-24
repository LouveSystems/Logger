using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace LouveSystems.Logging
{
    public class Logger
    {
        public enum LEVEL { TRACE, DEBUG, WARNING, INFO, ERROR };
        
        readonly string logFilePath = @"logs/{0}{1}.log";

        LEVEL level;
        CultureInfo culture = new CultureInfo("fr-FR");
        Dictionary<LEVEL, ConsoleColor> colors = new Dictionary<LEVEL, ConsoleColor>()
        {
            {LEVEL.TRACE, ConsoleColor.Magenta },
            {LEVEL.DEBUG, ConsoleColor.Gray },
            {LEVEL.INFO, ConsoleColor.White },
            {LEVEL.WARNING, ConsoleColor.Yellow },
            {LEVEL.ERROR, ConsoleColor.Red }
        };
        int flushEvery = 1000;

        bool outputToFile = false;
        bool outputToConsole = true;

        FileStream logFileStream = null;
        string programName;
        Timer flushTimer;
        Action<object> logFunction = (Action<object>)Console.WriteLine;

        public Logger(string programName = null, bool outputToFile = false, bool outputToConsole = true)
        {
            if (programName == null) programName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);

            this.Initialize(programName, outputToFile, outputToConsole);
        }

        void Initialize(string programName, bool outputToFile = false, bool outputToConsole = true)
        {
            this.programName = programName;
            this.outputToFile = outputToFile;
            this.outputToConsole = outputToConsole;

            if (outputToFile) {
                var filePath = string.Format(logFilePath, this.programName, "");
                Directory.CreateDirectory(
                Path.GetDirectoryName(
                        filePath
                    )
                );

                if (flushTimer != null) flushTimer.Dispose();
                if (logFileStream != null) logFileStream.Dispose();
                logFileStream = null;

                int i = 0;
                while (logFileStream == null) {
                    try {
                        filePath = string.Format(logFilePath, this.programName, i == 0 ? "" : i.ToString());
                        if (File.Exists(filePath)) File.Delete(filePath);

                        logFileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                    catch (IOException) {
                        // File is locked - increment and retry
                        i++;
                    }

                    if (i > 10) {
                        Console.WriteLine("After " + i + " attempts, could not access file " + logFilePath + ", giving up.");
                        return;
                    }
                }

                flushTimer = new Timer(
                    e => {
                        logFileStream.Flush();
                    },
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromMilliseconds(flushEvery));
            }
        }

        public void SetLevel(LEVEL level)
        {
            this.level = level;
        }

        public void SetConsoleFunction(Action<object> function)
        {
            this.logFunction = function;
        }

        public void Trace(params object[] msgs) { LogMessage(LEVEL.TRACE, msgs); }
        public void Debug(params object[] msgs) { LogMessage(LEVEL.DEBUG, msgs); }
        public void Info(params object[] msgs) { LogMessage(LEVEL.INFO, msgs); }
        public void Warn(params object[] msgs) { LogMessage(LEVEL.WARNING, msgs); }
        public void Error(params object[] msgs) { LogMessage(LEVEL.ERROR, msgs); }
        public void Fatal(Exception e)
        {
            LogMessage(LEVEL.ERROR, new string[1] { "================== FATAL ==================" });
            LogMessage(LEVEL.ERROR, e);
            Console.ReadKey();
            Environment.Exit(1);
        }

        void LogMessage(LEVEL msgLevel, params object[] msgs)
        {
            if (msgLevel < level) {
                return;
            }

            string caller = "---";

#if DEBUG
            StackFrame sf = new StackFrame(2, true);
            string file = sf.GetMethod().DeclaringType.Name;
            string method = sf.GetMethod().Name;
            try {
                caller = string.Format("{0}   {1}",
                    file.Substring(0, Math.Min(file.Length, 8)).PadRight(8),
                    method.Substring(0, Math.Min(method.Length, 14)).PadRight(14)
                );
            }
            catch (NullReferenceException) {
                caller = "???";
            }
#endif

            // Debug line formatting
            string line = "{0} [{1}] [{2}]:{3}";
            line = string.Format(line, DateTime.Now.ToString(culture.DateTimeFormat.LongTimePattern), msgLevel.ToString(), caller, string.Join(" ", msgs));

            if (outputToConsole) {
                Console.ForegroundColor = colors[msgLevel];
                logFunction(line);
            }

            if (outputToFile) {
                using (StreamWriter sw = new StreamWriter(logFileStream, Encoding.UTF8, 1024, leaveOpen: true)) {
                    sw.WriteLine(line);
                }
            }
        }
    }
}