using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;

namespace social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer
{
    public interface ITextAnalyzer
    {
        WordTagsVector CreateVector(string text);
        IEnumerable<string> GetWords(params string[] texts);
        IEnumerable<double> TextSimilarity(string comparedText, int persentOfText = 100, params string[] text);
        IEnumerable<double> VectorSimilarity(WordTagsVector vect1, IEnumerable<WordTagsVector> vectors);
        double VectorSimilarity(WordTagsVector vect1, WordTagsVector vectors);
    }
}