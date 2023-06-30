namespace BrowserHistoryStreaming.Common;

public class BrowserHistoryItem
{
    public string Title { get; set; }

    public string Url { get; set; }


    public override string? ToString()
    {
        return $"Title: '{ Title }', Url: '{ Url }'.";
    }
}