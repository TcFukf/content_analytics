namespace social_analytics.Bl.structures
{
    public interface IFrequencyDictionary<Tkey> : IEnumerable<KeyValuePair<Tkey, int>>
    {
        public int TotalCount { get; }
        int GetKeyCount(Tkey key);
        void AddFrequency(Tkey key, int increment);
        public Tkey[] FindMoreThanAverageKeys();
        public int Sum();
        double GetKeyFrequency(Tkey key);
    }
}