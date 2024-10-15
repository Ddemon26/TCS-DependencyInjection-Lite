using UnityEngine;
namespace TCS.DependencyInjection.Lite {
    internal static class Logger {
        const string CLASS_NAME = "Injector-lite";
        const string LOG_COLOR = "green";
        const string LOG_COLOR_WARNING = "yellow";
        const string LOG_COLOR_ERROR = "red";
        const string LOG_COLOR_ASSERT = "magenta";
        const string LOG_COLOR_EXCEPTION = "orange";
        
        public static bool IsDebugMode { get; set; } = true;

        static string SetPrefix(this string newString, LogType logType) {
            string color = logType switch {
                LogType.Warning => LOG_COLOR_WARNING,
                LogType.Error => LOG_COLOR_ERROR,
                LogType.Assert => LOG_COLOR_ASSERT,
                LogType.Exception => LOG_COLOR_EXCEPTION,
                _ => LOG_COLOR
            };
            return $"<color={color}>[{newString}]</color>";
        }

        static void LogInternal(string message, LogType logType, Object context = null) {
            if (!IsDebugMode) return;
            var formattedMessage = $"{CLASS_NAME.SetPrefix(logType)} {message}";
            #if PROJECT_DEBUG
            switch (logType) {
                case LogType.Warning:
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogType.Error:
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    Debug.LogError(formattedMessage);
                    break;
                case LogType.Assert:
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    Debug.LogAssertion(formattedMessage, context);
                    break;
                case LogType.Exception:
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    Debug.LogException(new System.Exception(formattedMessage));
                    break;
                case LogType.Log:
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    Debug.Log(formattedMessage);
                    break;
                default:
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    Debug.Log(formattedMessage);
                    break;
            }
            #endif
        }

        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        public static void Log(string message) => LogInternal(message, LogType.Log);
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        public static void LogWarning(string message) => LogInternal(message, LogType.Warning);
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        public static void LogError(string message) => LogInternal(message, LogType.Error);
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        public static void LogAssert(string message, Object ctx) => LogInternal(message, LogType.Assert, ctx);
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        public static void LogException(string message) => LogInternal(message, LogType.Exception);
    }
}