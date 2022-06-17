namespace Epam.Library.DAL.Interfaces;

public interface ILoggingDao
{
    void Log(string description, DateTime dateTime, string username, string stackTrace);
}