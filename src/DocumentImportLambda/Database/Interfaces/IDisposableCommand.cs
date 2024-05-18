using System.Data;

namespace DocumentImportLambda.Interfaces
{
    public interface IDisposableCommand : IDisposable
    {
        /// <summary>
        /// The query string associated with the command
        /// </summary>
        string QueryString { get; }

        /// <summary>
        /// Adds a parameter to the command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        void AddParameter(string name, SqlDbType type, object? value, int size = -1);

        /// <summary>
        /// Adds a parameter to the command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddParameter(string name, object? value);

        /// <summary>
        /// Executes the command as a non-query
        /// </summary>
        void ExecuteNonQuery();

        /// <summary>
        /// Executes the command and returns a reader
        /// </summary>
        /// <returns></returns>
        IDataReader ExecuteReader();

        /// <summary>
        /// Executes the command and returns a scalar value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T ExecuteScalar<T>();

        /// <summary>
        /// Opens the database connection used by the command
        /// </summary>
        /// <returns></returns>
        public Task Open();
    }
}