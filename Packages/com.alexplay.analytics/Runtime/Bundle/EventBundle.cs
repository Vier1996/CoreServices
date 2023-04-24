using System.Collections.Generic;

namespace ACS.Analytics.Bundle
{
    public class EventBundle : IEventBundle
    {
        private readonly AnalyticsService _analyticsService;
        
        public string Name {get; private set;}
        public Dictionary<string, object> Params {get; private set;}

        public EventBundle(AnalyticsService analyticsService, int paramsCapacity = 5)
        {
            _analyticsService = analyticsService;
            Params = new Dictionary<string, object>(paramsCapacity);
        }

        public IEventBundle SetName(string name)
        {
            Name = name;
            return this;
        }

        public IEventBundle AddParam(string paramName, object paramValue)
        {
            Params.Add(paramName, paramValue);
            return this;
        }

        public void TrackOnce()
        {
            if (Params.Count > 0) 
                _analyticsService.TrackEventOnce(Name, Params);
            else
                _analyticsService.TrackEventOnce(Name);
            Reset();
            _analyticsService.ReturnBundle(this);
        }

        public void Track()
        {
            if (Params.Count > 0) 
                _analyticsService.TrackEvent(Name, Params);
            else
                _analyticsService.TrackEvent(Name);
            Reset();
            _analyticsService.ReturnBundle(this);
        }

        private void Reset()
        {
            Name = string.Empty;
            Params.Clear();
        }
    }
}