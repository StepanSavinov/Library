namespace Epam.Library.BLL.Interfaces;

public interface ILoggingLogic
{
    void Log(string description, DateTime dateTime, string username, string stackTrace);
}