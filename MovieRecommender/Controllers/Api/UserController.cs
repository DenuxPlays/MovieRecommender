using Microsoft.AspNetCore.Mvc;
using MovieRecommender.Contexts;
using MovieRecommender.Entities;
using MovieRecommender.Interfaces;
using MovieRecommender.Views.Api.User;

namespace MovieRecommender.Controllers.Api;

[ApiController]
[Route("/api/user")]
public class UserController(ApplicationDbContext context, IWordListService wordListService)
    : ControllerBase
{
    [HttpPost]
    public ActionResult<UserCreatedResponse> CreateUser()
    {
        var token = GenerateSecureToken();
        var user = new UserEntity { Token = token };

        context.Users.Add(user);
        context.SaveChanges();

        return new UserCreatedResponse
        {
            Id = user.Id,
            Token = user.Token
        };
    }

    private string GenerateSecureToken()
    {
        const int wordCount = 6;
        // Get 6 random words from the service
        var words = wordListService.GetWords(wordCount);
        if (words.Count == 0)
        {
            throw new InvalidOperationException("No words available");
        }

        return string.Join("-", words);
    }
}