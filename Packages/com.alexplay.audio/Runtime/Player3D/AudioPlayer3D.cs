using System.Collections.Generic;
using ACS.Audio.StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACS.Audio.Player3D
{
    public class AudioPlayer3D
    {
        private readonly List<Player3DAgent> _agents;
        private readonly List<Player3DAgent> _freeAgents;
        private readonly Transform _parent;
        public int AgentsAmount => _agents.Count;
        public int FreeAgentsAmount => _freeAgents.Count;
        
        public AudioPlayer3D(AudioService audioService, int sourcesLimit)
        {
            _agents = new List<Player3DAgent>(sourcesLimit + 1);
            _freeAgents = new List<Player3DAgent>(_agents.Capacity);
            _parent = new GameObject("Audio Player 3D").transform;
            _parent.SetParent(audioService.Transform);
            CreateAgents(sourcesLimit);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void Play(AudioData data, Transform client, AudioService.Priority priority = AudioService.Priority.Default)
        {
            if (TryGetFreeAgent(priority, out var agent)) 
                agent.Play(data, client);
        }

        public bool IsAudioDataPlaying(AudioData audioData)
        {
            if (audioData is null) return false;
            foreach (var agent in _agents)
            {
                if (agent.CurrentAudioData == audioData && agent.IsPlaying) return true;
            }
            return false;
        }

        public void ReturnPlayer(Player3DAgent agent)
        {
            if (_freeAgents.Contains(agent)) 
                return;
            _freeAgents.Add(agent);
        }

        public bool TryGetAgent(out IPlayer3DAgent agent, AudioService.Priority priority = AudioService.Priority.Default) => 
            TryGetFreeAgent(priority, out agent);

        private void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
        {
            foreach (Player3DAgent source in _agents) source.Return();
        }

        private void CreateAgents(int sourcesLimit)
        {
            for (int i = 0; i < sourcesLimit; i++) 
                InstantiatePlayer();
        }

        private bool TryGetFreeAgent(AudioService.Priority priority, out IPlayer3DAgent agent)
        {
            agent = null;
            if (_freeAgents.Count == 0)
            {
                if (priority == AudioService.Priority.Default) return false;
                InstantiatePlayer();
            }
            agent = _freeAgents[0];
            _freeAgents.Remove((Player3DAgent) agent);
            return true;
        }

        private void InstantiatePlayer()
        {
            Player3DAgent agent =  new GameObject("AudioAgent", typeof(AudioSource))
                .AddComponent<Player3DAgent>();
            agent.transform.SetParent(_parent);
            agent.Initialize(this);
            _agents.Add(agent);
            _freeAgents.Add(agent);
        }
    }
}