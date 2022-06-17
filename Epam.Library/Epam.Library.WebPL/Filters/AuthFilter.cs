using Epam.Library.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Epam.Library.WebPL.Filters;
// IAuthenticationSignInHandler
public class AuthFilter : Attribute, IAuthorizationFilter
{
    private readonly ILoggingLogic _loggingLogic;
    
    public AuthFilter(ILoggingLogic loggingLogic)
    {
        _loggingLogic = loggingLogic;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        _loggingLogic.Log("Authorization", DateTime.Now,
            "???", Environment.StackTrace);
    }
}