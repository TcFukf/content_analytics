namespace social_analytics.Bl
{
    public interface ITextAnalytics
    {
        Task GetMostFrequentlyNouns();
        Task GetNouns(string text);
    }
}