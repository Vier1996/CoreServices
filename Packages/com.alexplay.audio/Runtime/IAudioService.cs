using ACS.Audio.Music;
using ACS.Audio.Player3D;
using UnityEngine;

namespace ACS.Audio
{
    public interface IAudioService
    {
        public AudioPlayer Player { get; }
        public AudioPlayer3D Player3D { get; }
        public Transform Transform { get; }
        public MusicPlayer MusicPlayer { get; }

        public float GetGroupVolume(string paramId);
        public void SetGroupVolumeByName(string groupName, float normalizedVolume);
        public bool IsGroupActive(string paramId);
        public void SetActiveGroup(string paramId, bool isActive);
    }
}