using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Services;

namespace MovieRecommender.Controllers.Frontend;

public class RecommendationController(
    AuthenticationService authenticationService,
    RecommendationService recommendationService,
    TmdbService tmdbService
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("UserToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await authenticationService.GetFromToken(token);

        var recommendations = recommendationService.GetRecommendations(user.Id);

        var globalRecommendationsTask = Task.WhenAll(recommendations.GenreRecommendedMovieIds.Select(
            tmdbService.GetMovie));
        var genreRecommendationsTask = Task.WhenAll(recommendations.GlobalRecommendedMovieIds.Select(
            tmdbService.GetMovie));

        var results = await Task.WhenAll(globalRecommendationsTask, genreRecommendationsTask);

        var globalRecommendations = results[0].Where(movie => movie != null).ToList();
        var genreRecommendations = results[1].Where(movie => movie != null).ToList();

        ViewBag.GlobalRecommendations = globalRecommendations;
        ViewBag.GenreRecommendations = genreRecommendations;

        return View();
    }
}