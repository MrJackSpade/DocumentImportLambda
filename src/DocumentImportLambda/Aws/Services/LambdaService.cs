using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using DocumentImportLambda.Aws.Dtos.Json;
using DocumentImportLambda.Aws.Extensions;
using System.Text.Json;

namespace DocumentImportLambda.Aws.Services
{
    /// <summary>
    /// An intermediary class that breaks out the SQS message and extracts the S3 event
    /// so that each message can be processed in isolation. This was originally in the
    /// function definition before moving from DLL to EXE based lambda
    /// </summary>
    public static class LambdaService
    {
        /// <summary>
        /// Handle the SQS event by breaking it into records and processing each record individually
        /// </summary>
        /// <param name="sqsEvent"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            using LambdaEventHandler eventHandler = await LambdaEventHandler.Create(context);

            await TryEach(sqsEvent, context, eventHandler.Handle);
        }

        /// <summary>
        /// This logic is broken out of the main function handler so that it can be leveraged by a dummy handler
        /// for testing. The dummy handler has been removed, but its easier to leave this distinct incase
        /// errors come up that require adding it back.
        /// </summary>
        /// <param name="sqsEvent"></param>
        /// <param name="context"></param>
        /// <param name="toInvoke"></param>
        /// <returns></returns>
        private static async Task TryEach(SQSEvent sqsEvent, ILambdaContext context, Func<S3EventNotificationRecord, Task> toInvoke)
        {
            // Iterate over each SQS message
            foreach (SQSEvent.SQSMessage? message in sqsEvent.Records)
            {
                //Validate the message
                if (!message.Validate(out List<string> errors))
                {
                    context.Logger.LogErrors(errors);
                    continue;
                }

                context.Logger.LogInformation(message.Body);

                //Deserialize the S3 event from the SQS message body
                S3Event? s3Event = JsonSerializer.Deserialize<S3Event>(message.Body);

                //Validate the event
                if (!s3Event.Validate(out errors))
                {
                    context.Logger.LogErrors(errors);
                    continue;
                }

                //Process the records
                foreach (S3EventNotificationRecord? record in s3Event!.Records)
                {
                    await toInvoke.Invoke(record);
                }
            }
        }
    }
}