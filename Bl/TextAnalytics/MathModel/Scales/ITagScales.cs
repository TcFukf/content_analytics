namespace social_analytics.Bl.TextAnalytics.MathModel.Scales
{
    public interface ITagScales
    {
        double CalcWeight(string word);
        double CalcOrder(string word);
        string GetScalse(string word);
    }
}