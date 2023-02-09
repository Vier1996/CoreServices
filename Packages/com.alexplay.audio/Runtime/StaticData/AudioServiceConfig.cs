using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace ACS.Audio.StaticData
{
    [Serializable]
    public class AudioServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.audio";

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

        [Serializable]
        public struct SceneThemes
        {
            public string SceneName;
            public AudioData[] Themes;
        }
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}