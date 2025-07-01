using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Model;
using MovieRecommender.Services;

namespace MovieRecommender.Controllers.Frontend;

public class UserController(UserService userService, AuthenticationService authenticationService) : Controller
{
    [HttpGet]
    public IActionResult Register()
    {
        var user = userService.CreateNewUser();

        ViewBag.GeneratedToken = user.Token;

        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        if (HttpContext.Session.GetString("UserToken") == null)
        {
            return RedirectToAction("Index", "Home");
        }

        HttpContext.Session.Remove("UserToken");

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Home", model);
        }

        if (authenticationService.IsValidToken(model.Token).Result)
        {
            HttpContext.Session.SetString("UserToken", model.Token);
            return RedirectToAction("Dashboard", "Home");
        }

        ModelState.AddModelError("", "Invalid token");

        return RedirectToAction("Index", "Home", model);
    }
}