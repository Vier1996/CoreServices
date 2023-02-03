using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ACS.Analytics
{
    public class AnalyticsService
    {
        private List<IAnalyticsAgent> _agents;

        public AnalyticsService(AnalyticsServiceConfig config)
        {
            InitializeAgents(config);
        }

        public void RegisterAgent(IAnalyticsAgent agent) => 
            _agents.Add(agent);

        public void TrackEvent(string eventName)
        {
            foreach (IAnalyticsAgent agent in _agents)
            {
                agent.SendEvent(eventName);
            }
        }

        public void TrackEvent(string eventName, Dictionary<string, object> @params)
        {
            foreach (IAnalyticsAgent agent in _agents)
            {
                agent.SendEvent(eventName, @params);
            }
        }

        public void TrackEventOnce(string eventName)
        {
            foreach (IAnalyticsAgent agent in _agents)
            {
                agent.SendEventOnce(eventName);
            }
        }

        public void TrackEventOnce(string eventName, Dictionary<string, object> @params)
        {
            foreach (IAnalyticsAgent agent in _agents)
            {
                agent.SendEventOnce(eventName, @params);
            }
        }
        
        private void InitializeAgents(AnalyticsServiceConfig config)
        {
            _agents = new List<IAnalyticsAgent>(config.ActiveAgents.Count);

            foreach (AgentInfo agentInfo in config.ActiveAgents)
            {
                Type agentType = agentInfo.GetType();
                IAnalyticsAgent analyticsAgent = (IAnalyticsAgent) Activator.CreateInstance(agentType);

                analyticsAgent.CanTrack = agentInfo.CanTrackEvents;
                
                _agents.Add(analyticsAgent);
            }
        }
    }
}