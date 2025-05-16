using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Services;
using MovieRecommender.Views.Api.Movie;

namespace MovieRecommender.Controllers.Api;

[ApiController]
[Route("/api/movie")]
public class MovieController(TmdbService tmdbService) : Controller
{
    [HttpGet("search", Name = "Search")]
    public ActionResult<List<MovieResponse>> Search([FromQuery(Name = "query")] string query)
    {
        return new ActionResult<List<MovieResponse>>(tmdbService.Search(query));
    }

    [HttpGet("{id:int}", Name = "GetMovie")]
    public ActionResult<MovieResponse> GetMovie(int id)
    {
        var movie = tmdbService.GetMovie(id).Result;

        return movie == null ? NotFound() : new ActionResult<MovieResponse>(movie);
    }
}