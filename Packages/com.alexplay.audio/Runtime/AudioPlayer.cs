using System;
using System.Collections.Generic;
using ACS.Audio.StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACS.Audio
{
    public class AudioPlayer
    {
        private readonly int _sourcesLimit;
        private readonly List<AudioSource> _audioSources;
        private readonly GameObject _sourcesParent;

        public AudioPlayer(AudioService audioService, int sourcesLimit)
        {
            _audioSources = new List<AudioSource>();
            _sourcesLimit = sourcesLimit;
            _sourcesParent = new GameObject("Audio Player 2D");
            _sourcesParent.transform.SetParent(audioService.Transform);
            CreateSources();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public AudioSource PlaySound(AudioData data, AudioService.Priority priority = AudioService.Priority.Default)
        {
            AudioSource source = _audioSources.Find(s => s.isPlaying == false);

            if (source == null)
            {
                switch (priority)
                {
                    case AudioService.Priority.Default:
                        return null;
                    case AudioService.Priority.High:
                        source = CreateSoundSource();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
                }
            }
            
            data.ConfigureSource(source);
            source.Play();
            return source;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            foreach (AudioSource source in _audioSources)
            {
                source.Stop();
            }
        }

        private void CreateSources()
        {
            for (int i = 0; i < _sourcesLimit; i++)
                CreateSoundSource();
        }

        private AudioSource CreateSoundSource()
        {
            var newSource = _sourcesParent.gameObject.AddComponent<AudioSource>();
            _audioSources.Add(newSource);
            newSource.playOnAwake = false;
            newSource.loop = false;
            newSource.spatialBlend = 0f;
            return newSource;
        }
    }
}