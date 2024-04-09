namespace social_analytics.Bl
{
    public interface IFrequencyDictionary<Tkey>: IEnumerable<KeyValuePair<Tkey, int>>
    {
        int? GetFrequency(Tkey key);
        void AddFrequency(Tkey key, int increment);
        public Tkey[] FindMoreThanAverageKeys();
    }
}