using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Services;
using MovieRecommender.Views.Api.Movie;

namespace MovieRecommender.Controllers.Api;

[ApiController]
[Route("/api/recommendation")]
public class RecommendationController(
    RecommendationService recommendationService,
    AuthenticationService authenticationService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<RecommendationResponse>> GetRecommendations()
    {
        var user = await authenticationService.GetUserFromRequest(Request);

        return recommendationService.GetRecommendations(user.Id);
    }
}