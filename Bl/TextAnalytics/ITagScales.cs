namespace social_analytics.Bl.TextAnalytics
{
    public interface ITagScales
    {
        double CalcWeight(string word);
        double CalcOrder(string word);
    }
}