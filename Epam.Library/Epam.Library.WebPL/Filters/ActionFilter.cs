using Epam.Library.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epam.Library.WebPL.Filters;

public class ActionFilter : Attribute, IActionFilter
{
    private readonly ILoggingLogic _loggingLogic;

    public ActionFilter(ILoggingLogic loggingLogic)
    {
        _loggingLogic = loggingLogic;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName;
        var stackTrace = Environment.StackTrace;

        _loggingLogic.Log(actionName, DateTime.Now, context.HttpContext.User.Identity.Name, stackTrace);
    }
}