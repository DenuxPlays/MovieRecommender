using System.ComponentModel.DataAnnotations;

namespace MovieRecommender.Model;

public class LoginModel
{
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; }
}
