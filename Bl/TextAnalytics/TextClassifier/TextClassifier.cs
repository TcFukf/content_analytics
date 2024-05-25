using social_analytics.Bl.structures;
using social_analytics.Bl.TextAnalytics.MathModel.WordVectorModel;
using social_analytics.Bl.TextAnalytics.TextAnalyzer.TextAnalyzer;
using System.Collections.Generic;
using TelegramWrapper.Models;

namespace social_analytics.Bl.TextAnalytics.TextClassifier
{
    public class TextClassifier
    {
        public static IEnumerable<TextCategory<MessageModel>> Classify(IEnumerable<MessageModel> messages, ITextAnalyzer analyzer, double Epsilon = 1 / 100d)
        {
            if (Math.Abs(Epsilon) > 1  || Epsilon < 0)
            {
                throw new ArgumentException("Epsilon must be 0<=eps<=1");
            }
            // OPTIMAZe
            List<CategoryNode<WordTagsVector>> vectorsNodes = new();
            Dictionary<MessageModel, WordTagsVector> messVector = new();
            foreach (var msg in messages)
            {
                CategoryNode<WordTagsVector> node = new() { Key = analyzer.CreateVector(msg.Text) };
                messVector.Add(msg,node.Key);
                vectorsNodes.Add(node);
            }
            Dictionary<WordTagsVector, CategoryGraph<WordTagsVector>> categoriesTable = new();

            foreach (var mainVector in vectorsNodes.OrderBy(v => -v.Key.GroupSumTagsOfVector))
            {
                if (!categoriesTable.ContainsKey(mainVector.Key))
                {
                    CategorizeByVector(analyzer, Epsilon, vectorsNodes, categoriesTable, mainVector);
                }
            }
            Dictionary<WordTagsVector,TextCategory<MessageModel>> categories = new();
            foreach (var msg in messVector)
            {
                (MessageModel Message, CategoryGraph<WordTagsVector> Category) tp = (msg.Key, categoriesTable[msg.Value]);
                if (categories.ContainsKey(tp.Category.Head.Key))
                {
                    categories[tp.Category.Head.Key].Values.Add(tp.Message);
                }
                else
                {
                    categories.Add(tp.Category.Head.Key,new TextCategory<MessageModel>() { HeadVector = tp.Category.Head.Key,Values = new() { tp.Message} });
                }
            }
            return categories.Values;


        }

        public static double RateCategory(TextCategory<MessageModel> category)
        {
            double countRate = 2*category.Values.Count() / ( 1 + Math.Pow((0.02d* category.Values.Count() - 0.35),2) );
            return countRate * Math.Sqrt( category.HeadVector.GroupSumTagsOfVector );
        }

        private static void CategorizeByVector(ITextAnalyzer analyzer, double Eps, List<CategoryNode<WordTagsVector>> vectorsNodes, Dictionary<WordTagsVector, CategoryGraph<WordTagsVector>> categories, CategoryNode<WordTagsVector>? mainVector)
        {
            CategoryGraph<WordTagsVector> category = new() { Head = mainVector };
            categories.Add(mainVector.Key, category);

            foreach (var vect in vectorsNodes)
            {
                if (vect == mainVector)
                {
                    continue;
                }
                double simil = analyzer.VectorSimilarity(vect.Key, mainVector.Key);
                if (simil >= Eps)
                {
                    if (!categories.ContainsKey(vect.Key))
                    {
                        mainVector.RelateNodesUnidirectionally(vect);
                        categories.Add(vect.Key, category);
                    }
                    else
                    {
                        category = categories[vect.Key];
                        if (mainVector.Nodes != null)
                        {
                            foreach (var node in mainVector.Nodes)
                            {
                                categories[node.Key] = category;
                            }
                        }
                        vect.RelateNodesUnidirectionally(mainVector);
                    }
                }
            }
        }
    }
}
