using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator.Pages;

public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public SummaryModel(ILogger<SummaryModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }

    public void OnGet(string id)
    {
        _logger.LogDebug($"Requested summary for ID: {id}");

        // Получаем базу данных Redis
        var db = _redis.GetDatabase();

        // Формируем ключи для Rank и Similarity
        string rankKey = "RANK-" + id;
        string similarityKey = "SIMILARITY-" + id;

        // Получаем значения из Redis
        var rankValue = db.StringGet(rankKey);
        var similarityValue = db.StringGet(similarityKey);

        // Преобразуем значения в double и присваиваем свойствам
        if (rankValue.HasValue)
        {
            Rank = (double)rankValue;
            _logger.LogDebug($"Rank retrieved: {Rank}");
        }

        if (similarityValue.HasValue)
        {
            Similarity = (double)similarityValue;
            _logger.LogDebug($"Similarity retrieved: {Similarity}");
        }
    }
}