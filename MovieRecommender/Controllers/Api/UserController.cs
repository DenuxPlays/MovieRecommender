using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Services;
using MovieRecommender.Views.Api.User;

namespace MovieRecommender.Controllers.Api;

[ApiController]
[Route("/api/user")]
public class UserController(UserService userService)
    : ControllerBase
{
    [HttpPost]
    public ActionResult<UserCreatedResponse> CreateUser()
    {
        var user = userService.CreateNewUser();
        return new UserCreatedResponse
        {
            Id = user.Id,
            Token = user.Token
        };
    }
}