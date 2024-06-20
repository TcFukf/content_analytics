using social_analytics.Bl.TextAnalytics.MathModel.Scales;
using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;
using social_analytics.Helpers;
using System.Linq;

namespace social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer
{
    public class TextAnalyzer : ITextAnalyzer
    {
        private readonly ITagScales _tagScales;

        public ITagScales Scales => _tagScales;

        public TextAnalyzer(ITagScales scales)
        {
            _tagScales = scales;
        }
        public IEnumerable<double> TextSimilarity(string comparedText, int persentOfText = 100, params string[] text)
        {
            return TextSimilarity(comparedText, _tagScales, text, persentOfText);

        }
        public static IEnumerable<double> TextSimilarity(string comparedText, ITagScales frequenctDictScaler, IEnumerable<string> texts, int persentOfText)
        {
            var words = TextAnalyticsTools.GetWordsFromStrings(comparedText).ToArray();
            int comprCount = (int)(words.Length * persentOfText / (double)100);
            WordTagsVector mainVector = new WordTagsVector(frequenctDictScaler, maxLength: comprCount, words);
            IEnumerable<WordTagsVector> tags = texts.Select(text => new WordTagsVector(frequenctDictScaler, maxLength: comprCount, TextAnalyticsTools.GetWordsFromStrings(text).ToArray()));
            return tags.Select(tag => TextAnalyticsTools.CalculateVectorsSimilarity(mainVector, tag));
        }
        public IEnumerable<double> VectorSimilarity(WordTagsVector vect1, IEnumerable<WordTagsVector> vectors)
        {
            return vectors.Select(vect => TextAnalyticsTools.CalculateVectorsSimilarity(vect1, vect)); ;
        }

        public IEnumerable<string> GetWords(params string[] texts)
        {
            return TextAnalyticsTools.GetWordsFromStrings(texts);
        }

        public WordTagsVector CreateVector(string text)
        {
            return new WordTagsVector(_tagScales, 20, GetWords(text).ToArray());
        }

        public double VectorSimilarity(WordTagsVector vect1, WordTagsVector vectors)
        {
            return TextAnalyticsTools.CalculateVectorsSimilarity(vect1,vectors);
        }
    }
}