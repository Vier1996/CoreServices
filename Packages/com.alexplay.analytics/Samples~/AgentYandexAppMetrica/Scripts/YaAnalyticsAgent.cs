using ACS.Analytics;
using UnityEngine;
using UnityEngine.Analytics;


public class YaAnalyticsAgent : IAnalyticsAgent
{
    private const string PlayerPrefsKeyPref = "YaAnalAmazingKey";
    private const string ApiKey = "INSERT_YOUR_WONDERFUL_API_KEY_HERE";
    private readonly IYandexAppMetrica _yaInstance;
    
    public YaAnalyticsAgent() : this(new YandexAppMetricaConfig(ApiKey)
    {
        SessionTimeout = 10,
        Logs = false,
        CrashReporting = false,
        LocationTracking = false,
        StatisticsSending = true,
        UserProfileID =
            string.IsNullOrEmpty(AnalyticsSessionInfo.userId) ? null : AnalyticsSessionInfo.userId,
        HandleFirstActivationAsUpdate = false
    }) { }

    public YaAnalyticsAgent(YandexAppMetricaConfig config)
    {
        AppMetrica.Instance.ActivateWithConfiguration(config);
        _yaInstance = AppMetrica.Instance;
    }

    public void TrackEvent(string eventName) => 
        _yaInstance.ReportEvent(eventName);

    public void TrackEvent(string eventName, Dictionary<string, object> @params) => 
        _yaInstance.ReportEvent(eventName, @params);

    public void TrackEventOnce(string eventName)
    {
        if (IsSent(eventName)) return;
        TrackEvent(eventName);
        MarkAsSent(eventName);
    }

    public void TrackEventOnce(string eventName, Dictionary<string, object> @params)
    {
        if (IsSent(eventName)) return;
        TrackEvent(eventName, @params);
        MarkAsSent(eventName);
    }

    public bool CanTrack { get; set; }

    private bool IsSent(string eventName) => PlayerPrefs.GetInt($"{PlayerPrefsKeyPref}_{eventName}") != 0;
    private void MarkAsSent(string eventName) => PlayerPrefs.SetInt($"{PlayerPrefsKeyPref}_{eventName}", 1023);
}