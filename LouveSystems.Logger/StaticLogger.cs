using System;
using System.Collections.Generic;
using System.Text;

namespace LouveSystems.Logging
{
    public static class StaticLogger
    {
        public static Logger SingleLogger { private set; get; }

        public static void Initialize(Logger logger)
        {
            SingleLogger = logger;
        }
        public static void Trace(object msgs) { SingleLogger.Trace(msgs); }
        public static void Debug(object msgs) { SingleLogger.Debug(msgs); }
        public static void Info(object msgs) { SingleLogger.Info(msgs); }
        public static void Warn(object msgs) { SingleLogger.Warn(msgs); }
        public static void Error(object msgs) { SingleLogger.Error(msgs); }
    }
}
