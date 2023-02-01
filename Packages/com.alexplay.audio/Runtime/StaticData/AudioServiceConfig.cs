using System;
using Config;
using UnityEngine.Audio;

namespace ACS.Audio.StaticData
{
    [Serializable]
    public class AudioServiceConfig : ServiceConfigBase
    {
        public AudioMixer Mixer;
        public int SourcesLimit2D;
        public int SourcesLimit3D;
        public float MusicTransitionDuration;
        public SceneThemes[] ScenesThemes;
        public string[] MixerVolumeParameters;

        [Serializable]
        public struct SceneThemes
        {
            public string SceneName;
            public AudioData[] Themes;
        }
    }
}