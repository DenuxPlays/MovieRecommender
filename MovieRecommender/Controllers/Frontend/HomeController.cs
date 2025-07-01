using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Model;
using MovieRecommender.Models;
using MovieRecommender.Services;

namespace MovieRecommender.Controllers.Frontend;

public class HomeController(AuthenticationService authenticationService, WatchlistService watchlistService) : Controller
{
    public IActionResult Index()
    {
        var existingToken = HttpContext.Session.GetString("UserToken");
        if (string.IsNullOrEmpty(existingToken)) return View();

        if (authenticationService.IsValidToken(existingToken).Result)
        {
            return RedirectToAction("Dashboard");
        }

        HttpContext.Session.Remove("UserToken");

        return View(new LoginModel());
    }

    public async Task<IActionResult> Dashboard()
    {
        var token = HttpContext.Session.GetString("UserToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index");
        }

        var user = authenticationService.GetFromTokenOptional(token).Result;
        if (user == null)
        {
            HttpContext.Session.Remove("UserToken");
            return RedirectToAction("Index");
        }

        var watchlist = await watchlistService.GetRenderableWatchlist(user.Id);
        ViewBag.User = user;

        return View(watchlist);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}