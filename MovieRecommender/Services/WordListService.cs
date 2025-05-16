using MovieRecommender.Interfaces;

namespace MovieRecommender.Services;

public class WordListService(IConfiguration configuration, ILogger<WordListService> logger, HttpClient httpClient)
    : IWordListService
{
    private List<string> _lines = [];
    private readonly Random _random = new();

    private const string FileName = "tokenwordlist.txt";

    public async Task InitializeAsync()
    {
        try
        {
            var fileUrl = configuration["TextFileResource:WordListUrl"];
            if (string.IsNullOrEmpty(fileUrl))
            {
                logger.LogCritical("Text file URL is not configured");
                return;
            }

            if (File.Exists(FileName))
            {
                logger.LogInformation($"Loading word list from local file: {FileName}");
                LoadFromFile();

                logger.LogInformation("Loaded word list with {LinesCount} entries.", _lines.Count);
                return;
            }

            logger.LogInformation("Downloading word list from URL: {FileUrl}", fileUrl);
            await LoadFromUrlAndWriteToFile(fileUrl);

            logger.LogInformation("Loaded word list with {LinesCount} entries.", _lines.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download or parse text file");
            _lines = [];
        }
    }

    public List<string> GetWords(int count)
    {
        if (_lines.Count == 0)
            return [];

        count = Math.Min(count, _lines.Count);

        var indices = new HashSet<int>();

        while (indices.Count < count)
        {
            indices.Add(_random.Next(_lines.Count));
        }

        return indices.Select(index => _lines[index]).ToList();
    }

    private async Task LoadFromUrlAndWriteToFile(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException("The URL cannot be null or empty.", nameof(url));
        }

        var content = await httpClient.GetStringAsync(url);

        _lines = content.Split(
            ["\r\n", "\r", "\n"],
            StringSplitOptions.RemoveEmptyEntries
        ).Select(line => ReplaceUmlauts(line.Trim())).ToList();

        WriteToFile(_lines);
    }

    private static void WriteToFile(List<string> words)
    {
        if (words == null || words.Count == 0)
        {
            throw new ArgumentException("The list of words cannot be null or empty.", nameof(words));
        }

        File.WriteAllLines(FileName, words);
    }

    private static string ReplaceUmlauts(string input)
    {
        return input
            .Replace("ä", "ae")
            .Replace("Ä", "Ae")
            .Replace("ö", "oe")
            .Replace("Ö", "Oe")
            .Replace("ü", "ue")
            .Replace("Ü", "Ue")
            .Replace("ß", "ss");
    }

    private void LoadFromFile()
    {
        if (!File.Exists(FileName))
        {
            throw new FileNotFoundException($"The file {FileName} does not exist.");
        }

        if (new FileInfo(FileName).Length == 0)
        {
            logger.LogError(
                $"File {FileName} is empty. Remove the file and restart the application to download it again.");
            throw new InvalidOperationException($"The file {FileName} is empty.");
        }

        _lines = File.ReadAllLines(FileName).ToList();
    }
}