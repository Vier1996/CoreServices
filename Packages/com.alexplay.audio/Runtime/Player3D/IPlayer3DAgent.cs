using ACS.Audio.StaticData;
using UnityEngine;

namespace ACS.Audio.Player3D
{
    public interface IPlayer3DAgent
    {
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public void Play(AudioData data, Transform client, bool returnAfterPlay = true, bool synchronizePosition = true, float synchronizeRate = 0.2f);
        public void Configure();
        public void Stop();
        public void Return();
    }
}