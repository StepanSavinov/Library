using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Epam.Library.BLL.Interfaces;
using Epam.Library.Entities;
using Epam.Library.WebPL.Filters;
using Epam.Library.WebPL.Helpers;
using Microsoft.AspNetCore.Mvc;
using Epam.Library.WebPL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Epam.Library.WebPL.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserLogic _userLogic;
    private const int PageSize = 20;

    public UserController(ILogger<UserController> logger, IUserLogic userLogic)
    {
        _logger = logger;
        _userLogic = userLogic;
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(AuthFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);
        var user = new User(model.Username.ToLower(), GetHashedPassword(model.Password),
            _userLogic.GetUserByUsername(model.Username).Role);

        if (_userLogic.Auth(user, out var errors))
        {
            var claims = new List<Claim> {new(ClaimTypes.Name, user.Username), new(ClaimTypes.Role, user.Role)};

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true, ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20), IsPersistent = true
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Library");
        }

        ModelState.AddModelError(string.Empty, "Incorrect username or password");
        return View(model);
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [HttpGet]
    public IActionResult Register() => View();

    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid && _userLogic.GetUserByUsername(model.Username) is null)
        {
            if (_userLogic.Register(
                    new User(model.Username.ToLower(), GetHashedPassword(model.Password)), out var errors))
            {
                return RedirectToAction("Login", "User");
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Fill in all the input fields");
        return View(model);
    }

    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [Authorize(Policy = "Admin")]
    public IActionResult AddUser() => View();
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [Authorize(Policy = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddUser(UserViewModel viewModel)
    {
        if (ModelState.IsValid && _userLogic.GetUserByUsername(viewModel.Username) is null)
        {
            if (_userLogic.Register(
                    new User(viewModel.Username.ToLower(), GetHashedPassword(viewModel.Password), viewModel.Role),
                    out var errors))
            {
                return View(viewModel);
            }
            
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }

            return View(viewModel);
        }
        
        ModelState.AddModelError(string.Empty, "Fill in all the input fields");
        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [Authorize]
    [HttpGet]
    public ActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Library");
    }
    
    //[ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [Authorize(Policy = "Admin")]
    [HttpGet]
    public IActionResult GetAllUsers(int page = 1)
    {
        const int pageSize = 20;
        var users = _userLogic.GetAllUsers();
        var items = CommonMethods.CreatePagination(users, page, PageSize);
        
        var pageViewModel = new PageViewModel(users.Count, page, pageSize);
        var viewModel = new UsersPageViewModel()
        {
            PageViewModel = pageViewModel,
            Users = items
        };
        
        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [Authorize(Policy = "Admin")]
    public IActionResult UpdateUser(int id, UserViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        
        var user = _userLogic.GetUserById(id);
        
        user.Username = viewModel.Username.ToLower();
        user.Password = GetHashedPassword(viewModel.Password);
        user.Role = viewModel.Role;

        if (_userLogic.UpdateUser(user, out var errors))
        {
            return RedirectToAction("GetAllUsers");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error.Message);
        }

        return View(viewModel);
    }
    
    [ServiceFilter(typeof(ExceptionFilter))]
    [ServiceFilter(typeof(ActionFilter))]
    [Authorize(Policy = "Admin")]
    public IActionResult DeleteUser(int id)
    {
        _userLogic.RemoveUser(id, out _);
        return RedirectToAction("GetAllUsers");
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
     public IActionResult Error()
     {
         return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
     }

     private string GetHashedPassword(string password)
     {
         using var sha = SHA512.Create();
         var sb = new StringBuilder();
         foreach (var item in sha.ComputeHash(Encoding.Unicode.GetBytes(password)))
         {
             sb.Append(item.ToString("X2"));
         }

         return sb.ToString();
     }
}