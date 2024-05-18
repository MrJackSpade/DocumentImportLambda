using Amazon.Lambda.Core;

namespace DocumentImportLambda.Aws.Extensions
{
    public static class ILambdaLoggerExtensions
    {
        /// <summary>
        /// Logs a single error with message, and stack trace
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void LogError(this ILambdaLogger logger, string message, Exception exception)
        {
            logger.LogError(message);
            logger.LogError(exception.Message);
            logger.LogTrace(exception.StackTrace);
        }

        /// <summary>
        /// Logs an IEnumerable of errors, for clarity
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="errors"></param>
        public static void LogErrors(this ILambdaLogger logger, IEnumerable<string> errors)
        {
            foreach (string error in errors)
            {
                logger.LogError(error);
            }
        }
    }
}