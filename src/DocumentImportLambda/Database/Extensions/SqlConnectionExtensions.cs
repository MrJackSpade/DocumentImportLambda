using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace DocumentImportLambda.Database.Extensions
{
    public static class SqlConnectionExtensions
    {
        /// <summary>
        /// Attempts to open a connection to the database the specified number of times, with the specified delay on failure
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tries"></param>
        /// <param name="delayMs"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static async Task TryOpen(this SqlConnection connection, int tries, int delayMs = 5000)
        {
            do
            {
                try
                {
                    switch (connection.State)
                    {
                        //If already open, do nothing
                        case ConnectionState.Open:
                            return;

                        //If closed, open
                        case ConnectionState.Closed:
                            connection.Open();
                            break;

                        //If not open or closed, something went wrong
                        default: throw new NotSupportedException();
                    }

                    return;
                }
                //Sql exception is possibly transient
                catch (SqlException ex)
                {
                    Debug.Write(ex);
                    await Task.Delay(delayMs);
                }
                //Timeout is transient
                catch (InvalidOperationException iex) when (iex.Message.Contains("Timeout expired"))
                {
                    Debug.Write(iex);
                    await Task.Delay(delayMs);
                }
                //Anything else we bomb
                catch (Exception e)
                {
                    Debug.Write(e);
                    throw;
                }
            } while (tries-- > 0);
        }
    }
}