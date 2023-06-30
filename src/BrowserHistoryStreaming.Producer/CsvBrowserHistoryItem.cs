namespace BrowserHistoryStreaming.Producer;

public class CsvBrowserHistoryItem
{
    public const string IdColumnKey = "id";
    public const string DateColumnKey = "date";
    public const string TimeColumnKey = "time";
    public const string TitleColumnKey = "title";
    public const string UrlColumnKey = "url";

    public string Id { get; set; }

    public DateTime Date { get; set; }

    public TimeSpan Time { get; set; }

    public String Title { get; set; }

    public string Url { get; set; }

    public string GetHostFromUrl() => new Uri(Url).Host;
}