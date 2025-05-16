namespace MovieRecommender.Interfaces;

public interface IWordListService
{
    Task InitializeAsync();
    List<string> GetWords(int count);
}