using ACS.Audio.Player3D;
using ACS.Audio.StaticData;
using ACS.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Alexplay.Samples.Audio.Scripts
{
    public class AudioSampleButton : MonoBehaviour
    {
        [SerializeField] private AudioData _audioData;
        [SerializeField] private float _pitch;
        [SerializeField] private float _volume;
        private IPlayer3DAgent _agent;

        [Button]
        public void AudioPlayerTest()
        {
            Core.Instance.AudioService.Player.PlaySound(_audioData);
        }

        [Button]
        public void AudioPlayer3DTest()
        {
            if (_agent == null && 
                Core.Instance.AudioService.Player3D.TryGetAgent(out _agent) == false) return;
            
            _agent.Play(_audioData, transform, false);
            _agent.Pitch = _pitch;
            _agent.Volume = _volume;
            
        }

        [Button]
        public void ReturnAgent() => 
            _agent?.Return();
    }
}