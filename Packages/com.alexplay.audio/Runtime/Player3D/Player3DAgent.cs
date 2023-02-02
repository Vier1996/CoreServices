using System;
using System.Collections;
using ACS.Audio.StaticData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Audio.Player3D
{
    [RequireComponent(typeof(AudioSource))]
    public class Player3DAgent : MonoBehaviour, IPlayer3DAgent
    {
        public event Action<Player3DAgent> PlaybackStarted;
        public event Action<Player3DAgent> PlaybackStopped;
        
        public float Pitch
        {
            get => _audioSource.pitch;
            set => _audioSource.pitch = value;
        }
        public float Volume
        {
            get => _userVolumeMultiplier;
            set
            {
                _userVolumeMultiplier = value;
                RecalculateVolume();
            }
        }

        public AudioData AudioData => _data;
        public bool IsPlaying => _audioSource.isPlaying;
        [ShowInInspector] private float _busyDuration;
        [ShowInInspector] private Transform _currentClient;
        private AudioSource _audioSource;
        private AudioPlayer3D _host;
        private Transform _transform;
        private Coroutine _synchronizeCoroutine;
        private AudioData _data;
        private bool _isBusy;
        private float _priorityVolumeMultiplier = 1;
        private float _userVolumeMultiplier = 1;

        public void Initialize(AudioPlayer3D host) => 
            _host = host;

        public void Play(AudioData data, Transform client, bool returnAfterPlay = true, bool synchronizePosition = true, float synchronizeRate = 0.2f)
        {
            if (_isBusy && _currentClient != client) 
                throw new Exception("Request from side client");
            _currentClient = client;
            _isBusy = true;
            UnsubscribeOnValidate();
            _data = data;
            Configure();
            if (_synchronizeCoroutine != null) 
                StopCoroutine(_synchronizeCoroutine);
            _synchronizeCoroutine = StartCoroutine(SyncPosition(returnAfterPlay, synchronizePosition, synchronizeRate));
            _audioSource.Play();
            PlaybackStarted?.Invoke(this);
            SubscribeOnValidate();
        }

        public void Stop()
        {
            if (this == null) return;
            if (_audioSource != null) _audioSource.Stop();
            StopAllCoroutines();
            _synchronizeCoroutine = null;
            PlaybackStopped?.Invoke(this);
        }

        public void Return()
        {
            Stop();
            _isBusy = false;
            _busyDuration = 0;
            _currentClient = null;
            _host.ReturnPlayer(this);
            _userVolumeMultiplier = 1;
        }

        public void Configure()
        {
            _data.ConfigureSource(_audioSource);
            RecalculateVolume();
        }

        public override string ToString()
        {
            return base.ToString() + _data.VolumePriority;
        }

        public void SetVolumeMultiplier(float multiplier)
        {
            _priorityVolumeMultiplier = multiplier;
            RecalculateVolume();
        }

        private void RecalculateVolume()
        {
            _audioSource.volume = _data.Volume * _priorityVolumeMultiplier * _userVolumeMultiplier;
        }
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _transform = transform;
        }

        private IEnumerator SyncPosition(bool returnAfterPlay, bool syncPosition, float syncRate)
        {
            syncRate = Mathf.Clamp(syncRate, 0.01f, float.MaxValue);
            var syncYield = new WaitForSeconds(syncRate);
            while (_isBusy)
            {
                if (_currentClient == null)
                {
                    Return();
                    yield break;
                }
                if (syncPosition)
                    _transform.position = _currentClient.position;
                    
                if (returnAfterPlay && _audioSource.isPlaying == false) 
                    Return();
                yield return syncYield;
                _busyDuration += syncRate;
            }
        }

        private void OnDestroy() => UnsubscribeOnValidate();

        private void SubscribeOnValidate()
        {
            #if UNITY_EDITOR
            _data.Validated += Configure;
            #endif
        }

        private void UnsubscribeOnValidate()
        {
            #if UNITY_EDITOR
            if (_data is null == false) _data.Validated -= Configure;
            #endif
        }
    }
}