using Amazon.Lambda.Core;

namespace DocumentImportLambda.Aws.Utilities
{
    public class LoggingStopWatch(string eventName, ILambdaLogger logger)
    {
        private readonly string _eventName = eventName;

        private readonly ILambdaLogger _logger = logger;

        public DateTime EndTime { get; private set; }

        public DateTime StartTime { get; private set; }

        public static LoggingStopWatch CreateStarted(string eventName, ILambdaLogger _logger)
        {
            return new LoggingStopWatch(eventName, _logger)
            {
                StartTime = DateTime.Now,
            };
        }

        public void Start()
        {
            StartTime = DateTime.Now;
        }

        public void Stop()
        {
            if (StartTime == DateTime.MinValue)
            {
                throw new Exception("Stopwatch was not started");
            }

            EndTime = DateTime.Now;

            _logger.LogInformation($"{_eventName}: {(int)(EndTime - StartTime).TotalMilliseconds:N}ms");
        }
    }
}