using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Services;

namespace MovieRecommender.Controllers.Frontend;

public class WatchlistController(AuthenticationService authenticationService, WatchlistService watchlistService)
    : Controller
{
    [HttpPost]
    public async Task<IActionResult> RemoveFromWatchlistForm(int movieId)
    {
        var token = HttpContext.Session.GetString("UserToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await authenticationService.GetFromToken(token);

        await watchlistService.RemoveFromWatchlist(user.Id, movieId);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> AddToWatchlistForm(int movieId)
    {
        var token = HttpContext.Session.GetString("UserToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await authenticationService.GetFromToken(token);

        await watchlistService.AddToWatchlist(user.Id, movieId);

        return RedirectToAction("Index", "Home");
    }
}