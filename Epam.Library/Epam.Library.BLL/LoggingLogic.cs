using Epam.Library.BLL.Interfaces;
using Epam.Library.DAL.Interfaces;

namespace Epam.Library.BLL;

public class LoggingLogic : ILoggingLogic
{
    private readonly ILoggingDao _loggingDao;

    public LoggingLogic(ILoggingDao loggingDao)
    {
        _loggingDao = loggingDao;
    }

    public void Log(string description, DateTime dateTime, string username, string stackTrace)
    {
        _loggingDao.Log(description, dateTime, username, stackTrace);
    }
}