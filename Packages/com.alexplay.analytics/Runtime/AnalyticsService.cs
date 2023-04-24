using System;
using System.Collections.Generic;
using ACS.Analytics.Agent;
using ACS.Analytics.Bundle;

namespace ACS.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ObjectPool<EventBundle> _bundles;
        private List<IAnalyticsAgent> _agents;

        public AnalyticsService(AnalyticsServiceConfig config)
        {
            _bundles = new ObjectPool<EventBundle>(() => new EventBundle(this));
            InitializeAgents(config);
        }

        public void RegisterAgent(IAnalyticsAgent agent) => 
            _agents.Add(agent);

        public IEventBundle Event(string eventName) => 
            _bundles.Get().SetName(eventName);

        public void ReturnBundle(EventBundle bundle) => 
            _bundles.Return(bundle);

        public void TrackEvent(string eventName)
        {
            foreach (IAnalyticsAgent agent in _agents) 
                agent.TrackEvent(eventName);
        }

        public void TrackEvent(string eventName, Dictionary<string, object> @params)
        {
            foreach (IAnalyticsAgent agent in _agents) 
                agent.TrackEvent(eventName, @params);
        }
        
        public void TrackEventOnce(string eventName)
        {
            foreach (IAnalyticsAgent agent in _agents) 
                agent.TrackEventOnce(eventName);
        }

        public void TrackEventOnce(string eventName, Dictionary<string, object> @params)
        {
            foreach (IAnalyticsAgent agent in _agents) 
                agent.TrackEventOnce(eventName, @params);
        }
        
        private void InitializeAgents(AnalyticsServiceConfig config)
        {
            _agents = new List<IAnalyticsAgent>(config.ActiveAgents.Count);
            
            foreach (AgentInfo agentInfo in config.ActiveAgents)
                if (agentInfo.CanTrackEvents)
                    _agents.Add((IAnalyticsAgent) Activator.CreateInstance(agentInfo.GetAgentType()));
        }
    }
}