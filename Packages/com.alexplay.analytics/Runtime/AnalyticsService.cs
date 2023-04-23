using System;
using System.Collections.Generic;

namespace ACS.Analytics
{
    public class AnalyticsService : IAnalyticsService
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
                agent.TrackEvent(eventName);
        }

        public void TrackEvent(string eventName, string paramName, string paramValue)
        {
            foreach (IAnalyticsAgent agent in _agents) 
                agent.TrackEvent(eventName, paramName, paramValue);
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

        public void TrackEventOnce(string eventName, string paramName, string paramValue)
        {
            foreach (IAnalyticsAgent agent in _agents) 
                agent.TrackEventOnce(eventName, paramName, paramValue);
        }
        
        public void TrackEventOnce(string eventName, Dictionary<string, object> @params)
        {
            foreach (IAnalyticsAgent agent in _agents) 
                agent.TrackEventOnce(eventName, @params);
        }
        
        private void InitializeAgents(AnalyticsServiceConfig config)
        {
            _agents = new List<IAnalyticsAgent>(config.ActiveAgents.Count);
            
            List<Type> availableAgentTypes = AnalyticsUtils.GetAllAvailableAgents();

            foreach (Type agentType in availableAgentTypes)
            {
                AgentInfo agentInfo = config.ActiveAgents.Find(ag => ag.TypeName == agentType.Name);
                
                if (agentInfo != null && agentInfo.CanTrackEvents)
                {
                    IAnalyticsAgent analyticsAgent = (IAnalyticsAgent) Activator.CreateInstance(agentType);
                    analyticsAgent.CanTrack = agentInfo.CanTrackEvents;
                    _agents.Add(analyticsAgent);
                }
            }
        }
    }
}