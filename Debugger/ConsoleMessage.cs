using System.Diagnostics;
using UnityEngine;

namespace ModTools
{
    internal sealed class ConsoleMessage
    {
        public ConsoleMessage(string caller, string message, LogType type, StackTrace trace)
        {
            Caller = caller;
            Message = message;
            Type = type;
            Count = 1;
            Trace = trace;
        }

        public string Caller { get; }

        public string Message { get; }

        public LogType Type { get; }

        public int Count { get; set; }

        public StackTrace Trace { get; }
    }
}