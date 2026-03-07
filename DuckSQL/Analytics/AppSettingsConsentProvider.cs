namespace DuckSQL.Analytics
{
    public class AppSettingsConsentProvider : IConsentProvider
    {
        public bool AnalyticsDataGatheringConsent => AppSettings.AnalyticsDataGatheringConsent;
    }
}