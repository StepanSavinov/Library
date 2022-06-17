using System.Data;
using System.Data.SqlClient;
using Epam.Library.DAL.Interfaces;

namespace Epam.Library.DAL;

public class LoggingDao : ILoggingDao
{
    private readonly string? _connectionString;

    public LoggingDao(SqlConfig config)
    {
        _connectionString = config.ConnectionString;
    }
    
    public void Log(string description, DateTime dateTime, string username, string stackTrace)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var cmd = new SqlCommand("LogEvent", connection))
            {
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@DateTime", dateTime);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@StackTrace", stackTrace);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }
    }
}