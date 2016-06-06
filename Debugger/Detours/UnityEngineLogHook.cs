using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using ModTools.Redirection;
using UnityEngine;

namespace ModTools.Detours
{
    [TargetType(typeof(UnityEngine.Debug))]
    public class UnityEngineLogHook
    {
        private static long loopPreventer = 0;

        private static MethodInfo logMethod = typeof(UnityEngine.Debug).GetMethod("Internal_Log", BindingFlags.NonPublic | BindingFlags.Static);
        private static MethodInfo logExceptionMethod = typeof(UnityEngine.Debug).GetMethod("Internal_LogException", BindingFlags.NonPublic | BindingFlags.Static);
        private static RedirectCallsState logState;
        private static RedirectCallsState logExceptionState;


        public static void Deploy()
        {
            if (!Redirector<UnityEngineLogHook>.IsDeployed())
            {
                logState = RedirectionHelper.RedirectCalls(typeof(UnityEngineLogHook).GetMethod("Internal_Log", BindingFlags.NonPublic | BindingFlags.Static), logMethod);
                logExceptionState = RedirectionHelper.RedirectCalls(typeof(UnityEngineLogHook).GetMethod("Internal_LogException", BindingFlags.NonPublic | BindingFlags.Static), logExceptionMethod);
            }
            Redirector<UnityEngineLogHook>.Deploy();
        }

        public static void Revert()
        {
            if (Redirector<UnityEngineLogHook>.IsDeployed())
            {
                RedirectionHelper.RevertRedirect(typeof(UnityEngineLogHook).GetMethod("Internal_Log", BindingFlags.NonPublic | BindingFlags.Static), logState);
                RedirectionHelper.RevertRedirect(typeof(UnityEngineLogHook).GetMethod("Internal_LogException", BindingFlags.NonPublic | BindingFlags.Static), logExceptionState);
            }
            Redirector<UnityEngineLogHook>.Revert();
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Internal_Log(int level, string msg, System.Object obj)
        {
            UnityEngine.Debug.Log("Failed to detour Internal_Log()");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Internal_LogException(Exception exception, System.Object obj)
        {
            UnityEngine.Debug.Log("Failed to detour Internal_LogException()");
        }

        [RedirectMethod]
        public static void Log(object message)
        {
            if (loopPreventer > 0)
            {
                return;
            }
            loopPreventer++;
            try
            {
                if (message == null)
                {
                    message = "Null";
                }
                Internal_Log(0, message.ToString(), null);
                if (ModTools.Instance.console == null)
                {
                    return;
                }
                ModTools.Instance.console.AddMessage(message.ToString(), LogType.Log);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                loopPreventer = 0;
            }
        }

        [RedirectMethod]
        public static void LogFormat(string format, params object[] args)
        {
            if (loopPreventer > 0)
            {
                return;
            }
            loopPreventer++;
            var message = string.Format(format, args);
            Log(message);
            loopPreventer = 0;
        }

        [RedirectMethod]
        public static void LogWarning(object message)
        {
            if (loopPreventer > 0)
            {
                return;
            }
            loopPreventer++;
            try
            {
                if (message == null)
                {
                    message = "Null";
                }
                Internal_Log(1, message.ToString(), null);
                if (ModTools.Instance.console == null)
                {
                    return;
                }
                ModTools.Instance.console.AddMessage(message.ToString(), LogType.Warning);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                loopPreventer = 0;
            }
        }

        [RedirectMethod]
        public static void LogWarningFormat(string format, params object[] args)
        {
            if (loopPreventer > 0)
            {
                return;
            }
            loopPreventer++;
            var message = string.Format(format, args);
            LogWarning(message);
            loopPreventer = 0;
        }

        [RedirectMethod]
        public static void LogError(object message)
        {
            if (loopPreventer > 0)
            {
                return;
            }
            loopPreventer++;
            try
            {
                if (message == null)
                {
                    message = "Null";
                }
                Internal_Log(2, message.ToString(), null);

                if (ModTools.Instance.console == null)
                {
                    return;
                }
                ModTools.Instance.console.AddMessage(message.ToString(), LogType.Error);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                loopPreventer = 0;
            }
        }

        [RedirectMethod]
        public static void LogErrorFormat(string format, params object[] args)
        {
            if (loopPreventer > 0)
            {
                return;
            }
            loopPreventer++;
            var message = string.Format(format, args);
            LogError(message);
            loopPreventer = 0;
        }

        [RedirectMethod]
        public static void LogException(Exception exception)
        {
            if (loopPreventer > 0)
            {
                return;
            }
            loopPreventer++;
            try
            {
                Internal_LogException(exception, null);
                if (ModTools.Instance.console == null)
                {
                    return;
                }
                var message = exception?.ToString() ?? "Null exception";
                ModTools.Instance.console.AddMessage(message, LogType.Exception);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                loopPreventer = 0;
            }
        }

    }

}
