namespace DocumentImportLambda.Database.Models
{
    /// <summary>
    /// Represents a record in the client File table used by SEA/RQS
    /// </summary>
    public struct FileRecord(string description, string type, int size, long fxEnteredById, DateTime enteredDate, int? statusInd = null)
    {
        public string Description { get; set; } = description;

        public DateTime EnteredDate { get; set; } = enteredDate;

        public long FxEnteredById { get; set; } = fxEnteredById;

        public int Size { get; set; } = size;

        public int? StatusInd { get; set; } = statusInd;

        public string Type { get; set; } = type;
    }
}