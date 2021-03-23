using Amazon.Lambda.Core;

namespace yrtt_lambda_tasks_web_api.Logging
{
    public static class Logger
    {
        public static string LogLevel { get; set; }

        public static void LogDebug(string message, string method, string className)
        {
            if (LogLevel.ToUpper() == "DEBUG")
            {
                LambdaLogger.Log($"CUSTOM DEBUG: |ClassName: {className} |Method: {method} |Message: {message}");
            }
        }
        public static void LogInformation(string message, string method, string className)
        {
            LambdaLogger.Log($"CUSTOM INFORMATION: |ClassName: {className} |Method: {method} |Message: {message}");
        }
        public static void LogError(string message, string method, string className, string exceptionMessage)
        {
            LambdaLogger.Log($"CUSTOM ERROR: |ClassName: {className} |Method: {method} |Message: {message} |Exception: {exceptionMessage}");
        }
    }
}
