namespace DocumentImportLambda.Aws.Pocos
{
    /// <summary>
    /// Validation results determining course of action for an S3 event
    /// </summary>
    public enum EventValidation
    {
        /// <summary>
        /// We shouldn't get this
        /// </summary>
        Undefined,

        /// <summary>
        /// Good to process
        /// </summary>
        Valid,

        /// <summary>
        /// Something is wrong with the event
        /// </summary>
        Invalid,

        /// <summary>
        /// Nothing is wrong, but we don't want to process
        /// </summary>
        Skip
    }
}