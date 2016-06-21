using Grpc.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dbg = System.Diagnostics.Debug;

namespace GoogleCloudSamples.Models
{
    public class DebugLogger : ILogger
    {
        public void Debug(string message)
        {
            Dbg.WriteLine("Debug: " + message);
        }

        public void Debug(string format, params object[] formatArgs)
        {
            Dbg.WriteLine("Debug: " + string.Format(format, formatArgs));
        }

        public void Error(string message)
        {
            Dbg.WriteLine("Error: " + message);
        }

        public void Error(Exception exception, string message)
        {
            Dbg.WriteLine("Error: " + exception + " Message:" + message);
        }

        public void Error(string format, params object[] formatArgs)
        {
            Dbg.WriteLine("Error: " + string.Format(format, formatArgs));
        }

        public ILogger ForType<T>()
        {
            return this;
        }

        public void Info(string message)
        {
            Dbg.WriteLine("Info: " + message);
        }

        public void Info(string format, params object[] formatArgs)
        {
            Dbg.WriteLine("Info: " + string.Format(format, formatArgs));
        }

        public void Warning(string message)
        {
            Dbg.WriteLine("Warning: " + message);
        }

        public void Warning(Exception exception, string message)
        {
            Dbg.WriteLine("Warning: " + exception + " Message:" + message);
        }

        public void Warning(string format, params object[] formatArgs)
        {
            Dbg.WriteLine("Warning: " + string.Format(format, formatArgs));
        }
    }
}