using Epam.Library.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epam.Library.WebPL.Filters;

public class ExceptionFilter : Attribute, IExceptionFilter
{
    private readonly ILoggingLogic _loggingLogic;

    public ExceptionFilter(ILoggingLogic loggingLogic)
    {
        _loggingLogic = loggingLogic;
    }

    public void OnException(ExceptionContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var exceptionStack = context.Exception.StackTrace;
        var exceptionMessage = context.Exception.Message;
        var description = $"Method {actionName} threw {exceptionMessage}";
        
        _loggingLogic.Log(description, DateTime.Now,
            context.HttpContext.User.Identity.Name, exceptionStack);

        //context.ExceptionHandled = true;
    }
}