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
                agent.TrackEvent(eventName);
            }
        }

        public void TrackEvent(string eventName, Dictionary<string, object> @params)
        {
            foreach (IAnalyticsAgent agent in _agents)
            {
                agent.TrackEvent(eventName, @params);
            }
        }

        public void TrackEventOnce(string eventName)
        {
            foreach (IAnalyticsAgent agent in _agents)
            {
                agent.TrackEventOnce(eventName);
            }
        }

        public void TrackEventOnce(string eventName, Dictionary<string, object> @params)
        {
            foreach (IAnalyticsAgent agent in _agents)
            {
                agent.TrackEventOnce(eventName, @params);
            }
        }
        private void InitializeAgents(AnalyticsServiceConfig config)
        {
            _agents = new List<IAnalyticsAgent>(config.Agents.Length);
            foreach (AnalyticsServiceConfig.AgentInfo agentInfo in config.Agents)
            {
                if (TryGetType(agentInfo, out Type type))
                    _agents.Add((IAnalyticsAgent) Activator.CreateInstance(type));
            }
        }

        private bool TryGetType(AnalyticsServiceConfig.AgentInfo info, out Type type)
        {
            Assembly assembly = Assembly.Load(info.AssemblyName);
            type = assembly.GetType(info.TypeName);
            if (type.IsClass && type.GetInterfaces().Contains(typeof(IAnalyticsAgent)))
                return true;

            throw new ArgumentException(
                $"Type '{info.TypeName}' from '{info.AssemblyName}' assembly  is not implement '{typeof(IAnalyticsAgent)} interface.'");
        }
    }
}