namespace social_analytics.Bl.structures
{
    public interface IFrequencyDictionary<Tkey> : IEnumerable<KeyValuePair<Tkey, int>>
    {
        int? GetFrequency(Tkey key);
        void AddFrequency(Tkey key, int increment);
        public Tkey[] FindMoreThanAverageKeys();
        public int Sum();

    }
}