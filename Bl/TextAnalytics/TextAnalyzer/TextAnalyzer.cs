using social_analytics.Bl.TextAnalytics.MathModel.Scales;
using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;
using social_analytics.Helpers;
using System.Linq;

namespace social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer
{
    public class TextAnalyzer : ITextAnalyzer
    {
        private readonly ITagScales _tagScales;
        public TextAnalyzer(ITagScales scales)
        {
            _tagScales = scales;
        }
        public IEnumerable<double> TextSimilarity(string comparedText, int persentOfText = 100, params string[] text)
        {
            return TextSimilarity(comparedText,_tagScales,text, persentOfText);

        }
        public static IEnumerable<double> TextSimilarity(string comparedText, ITagScales frequenctDictScaler, IEnumerable<string> texts, int persentOfText)
        {
            var words = TextAnalyticsTools.GetStringEntities(comparedText).ToArray();
            int comprCount = (int)(words.Length * persentOfText / (double)100);
            WordTagsVector mainVector = new WordTagsVector(frequenctDictScaler,numberOfBest:comprCount ,words);
            IEnumerable<WordTagsVector> tags = texts.Select(text => new WordTagsVector(frequenctDictScaler,numberOfBest:comprCount, TextAnalyticsTools.GetStringEntities(text).ToArray()));
            return tags.Select(tag => TextAnalyticsTools.CalculateVectorsSimilarity(mainVector, tag));
        }
    }
}