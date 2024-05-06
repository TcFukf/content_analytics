namespace social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer
{
    public interface ITextAnalyzer
    {
        IEnumerable<double> TextSimilarity(string comparedText, IEnumerable<string> text);
    }
}