namespace DuckSQL.Analytics
{
    public interface IConsentProvider
    {
        public bool AnalyticsDataGatheringConsent { get; }
    }
}