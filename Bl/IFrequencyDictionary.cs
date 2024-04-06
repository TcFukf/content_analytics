namespace social_analytics.Bl
{
    public interface IFrequencyDictionary<Tkey>
    {
        int? GetFrequency(Tkey key);
        void IncreaseFrequency(Tkey key, int increment);
        public Tkey[] FindMoreThanAverageKeys();
    }
}