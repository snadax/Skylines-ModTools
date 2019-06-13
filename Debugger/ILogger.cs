using UnityEngine;

namespace ModTools
{
    internal interface ILogger
    {
        void Log(string message, LogType type);
    }
}