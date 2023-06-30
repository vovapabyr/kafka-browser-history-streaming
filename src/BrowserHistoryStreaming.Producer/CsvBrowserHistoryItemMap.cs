using CsvHelper.Configuration;

namespace BrowserHistoryStreaming.Producer;

public class CsvBrowserHistoryItemMap : ClassMap<CsvBrowserHistoryItem>
{
    public CsvBrowserHistoryItemMap()
    {
        Map(m => m.Id).Name(CsvBrowserHistoryItem.IdColumnKey);
        Map(m => m.Date).TypeConverterOption.Format("M/d/yyyy").Name(CsvBrowserHistoryItem.DateColumnKey);
        Map(m => m.Time).TypeConverterOption.Format("c").Name(CsvBrowserHistoryItem.TimeColumnKey);
        Map(m => m.Title).Name(CsvBrowserHistoryItem.TitleColumnKey);
        Map(m => m.Url).Name(CsvBrowserHistoryItem.UrlColumnKey);
    }
}