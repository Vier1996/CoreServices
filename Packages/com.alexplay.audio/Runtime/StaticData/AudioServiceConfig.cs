using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace ACS.Audio.StaticData
{
    [Serializable]
    public class AudioServiceConfig : ServiceConfigBase
    {        
        [ShowIf("@IsEnabled == true")]
        public AudioMixer Mixer;
        [ShowIf("@IsEnabled == true")]
        public int SourcesLimit2D;
        [ShowIf("@IsEnabled == true")]
        public int SourcesLimit3D;
        [ShowIf("@IsEnabled == true")]
        public float MusicTransitionDuration;
        [ShowIf("@IsEnabled == true")]
        public SceneThemes[] ScenesThemes;
        [ShowIf("@IsEnabled == true")]
        public string[] MixerVolumeParameters;
        [ShowIf("@IsEnabled == true")]
        public int PlaybackPrioritiesAmount;

        protected override string PackageName => "com.alexplay.audio";

        [Serializable]
        public struct SceneThemes
        {
            public string SceneName;
            public AudioData[] Themes;
        }
    }
}