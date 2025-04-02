using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using System;


// Вынести работу с базой в отдельные репозиории, классы, методы
namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public void OnGet()
    {
    }

    public IActionResult OnPost(string text)
    {
    _logger.LogDebug($"Input text: '{text}'");

    if (string.IsNullOrWhiteSpace(text))
    {
        ModelState.AddModelError(string.Empty, "Ввод текста не может быть пустым.");
        return Page();
    }

    // text = text.Trim();

    // Приводим текст к нижнему регистру
    text = text.ToLower();

    // Генерация уникального ID
    string id = Guid.NewGuid().ToString();

    // Получаем базу данных Redis
    var db = _redis.GetDatabase();

    // Сохраняем текст в Redis
    string textKey = "TEXT-" + id;
    db.StringSet(textKey, text);  // Все пробелы будут учтены
    _logger.LogDebug($"Text saved with key: {textKey}");

    // Рассчитываем rank и сохраняем его в Redis
    string rankKey = "RANK-" + id;
    double rank = CalculateRank(text);
    db.StringSet(rankKey, rank);
    _logger.LogDebug($"Rank calculated: {rank}");

    // Проверяем similarity и сохраняем его в Redis
    string similarityKey = "SIMILARITY-" + id;
    int similarity = CheckSimilarity(text, db, textKey);
    db.StringSet(similarityKey, similarity);
    _logger.LogDebug($"Similarity calculated: {similarity}");

    // Перенаправляем на страницу summary с параметром id
    return Redirect($"summary?id={id}");
    }

    private double CalculateRank(string text)
    {
        int totalChars = text.Length;
        int nonAlphabetChars = text.Count(c => !char.IsLetter(c)); // Считаем неалфавитные символы
        double rank = (double)nonAlphabetChars / totalChars;
        _logger.LogDebug($"Total chars: {totalChars}, Non-alphabet chars: {nonAlphabetChars}, Rank: {rank}");
        return rank;
    }

private int CheckSimilarity(string text, IDatabase db, string currentTextKey)
{
    _logger.LogDebug($"Checking similarity for text: '{text}'");

    // Получаем все ключи в базе
    var keys = db.Multiplexer.GetServer(db.Multiplexer.GetEndPoints()[0]).Keys().ToArray();

    foreach (var key in keys)
    {
        // Пропускаем текущее значение
        if (key.ToString() == currentTextKey)
            continue;

        var storedText = db.StringGet(key.ToString());
        _logger.LogDebug($"Stored text: '{storedText}'");

        // Сравниваем тексты
        bool isEqual = storedText.ToString().Equals(text, StringComparison.Ordinal);
        _logger.LogDebug($"Comparison result: {isEqual}");

        if (isEqual)
        {
            return 1;
        }
    }

    return 0; // Если схожих строк не найдено
}
}