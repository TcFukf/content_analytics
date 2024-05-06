using social_analytics.Bl.TextAnalytics.MathModel;
using social_analytics.Bl.TextAnalytics.MathModel.Scales;
using social_analytics.Helpers;

namespace social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer
{
    public class TextAnalyzer : ITextAnalyzer
    {
        private readonly ITagScales _tagScales;
        public TextAnalyzer(ITagScales scales)
        {
            _tagScales = scales;
        }
        public IEnumerable<double> TextSimilarity(string comparedText, IEnumerable<string> text)
        {
            return TextSimilarity(comparedText,_tagScales,text);

        }
        public static IEnumerable<double> TextSimilarity(string comparedText, ITagScales scales, IEnumerable<string> text)
        {
            WordTags mainVector = new WordTags(TextAnalyticsTools.GetStringEntities(comparedText), scales, true);
            IEnumerable<WordTags> tags = text.Select(text => new WordTags(TextAnalyticsTools.GetStringEntities(text), scales, false));
            return tags.Select(tag => TextAnalyticsTools.CalculateTagsSimilarity(mainVector, tag, scales));
        }
    }
}