using MovieRecommender.Contexts;
using MovieRecommender.Entities;
using MovieRecommender.Interfaces;

namespace MovieRecommender.Services;

public class UserService(ApplicationDbContext context, IWordListService wordListService)
{
    public UserEntity CreateNewUser()
    {
        var token = GenerateSecureToken();
        var user = new UserEntity { Token = token };

        context.Users.Add(user);
        context.SaveChanges();

        return user;
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