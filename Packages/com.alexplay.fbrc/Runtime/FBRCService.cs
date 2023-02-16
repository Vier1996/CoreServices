using System;
using System.Collections.Generic;
using ACS.FBRC.StaticData;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace ACS.FBRC
{
    public class FBRCService
    {
        private readonly FirebaseRemoteConfig _remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        private readonly FBRCConfig _config;
        private bool _isDefaultsInitialized;
        private bool _isFetchedDataActivated;


        public FBRCService(FBRCConfig config)
        {
            _config = config;
            InitializeAsync().Forget();
        }

        public UniTask EnsureInitialized() => UniTask.WhenAll(EnsureFBRCInitializedAsync(), EnsureDefaultsInitialized());
        
        public UniTask EnsureFetchedDataActivated() => UniTask.WaitUntil(() => _isFetchedDataActivated);

        private UniTask EnsureDefaultsInitialized() => UniTask.WaitUntil(() => _isDefaultsInitialized);
        
        private async UniTask EnsureFBRCInitializedAsync() => 
            await _remoteConfig.EnsureInitializedAsync();

        private async UniTaskVoid InitializeAsync()
        {
            await SetDefaults();
            switch (_config.LoadingStrategy)
            {
                case LoadStrategy.ActivateThenLoad:
                // await ActivateAsync();
                // await FetchAsync();
                // break;
                case LoadStrategy.LoadThenActivate:
                    await FetchAsync();
                    await ActivateAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask FetchAsync()
        {
            Log("Fetching started");
            var fetchAsync = _remoteConfig.FetchAsync(TimeSpan.FromSeconds(_config.CacheExpirationInSeconds));
            await fetchAsync;

            if (_remoteConfig.Info.LastFetchStatus == LastFetchStatus.Success)
            {
                Log($"Fetching successfully completed. {_remoteConfig.Info.FetchTime}");
                return;
            }
            LogError($"Fetching data was unsuccessful\n{nameof(_remoteConfig.Info.LastFetchStatus)}: {_remoteConfig.Info.LastFetchStatus}");
        }

        private async UniTask ActivateAsync()
        {
            var activateAsync = _remoteConfig.ActivateAsync();
            await activateAsync;
            _isFetchedDataActivated = true;
            Log(activateAsync.Result
                ? $"Remote data loaded and ready for use. Last fetch time {_remoteConfig.Info.FetchTime}."
                : $"Hashed data loaded and ready for use. Last fetch time {_remoteConfig.Info.FetchTime}.");
        }

        private async UniTask SetDefaults()
        {
            Log("Setup defaults started");
            var def = new Dictionary<string, object>();

            foreach (FBRemoteConfigValue value in _config.Values) 
                def.Add(value.Name, GetValue(value));

            await _remoteConfig.SetDefaultsAsync(def);

            _isDefaultsInitialized = true;
            Log("Setup defaults completed");
            
            object GetValue(FBRemoteConfigValue value)
            {
                switch (value.Type)
                {
                    case ValType.Bool: return value.BoolValue;
                    case ValType.Long: return value.LongValue;
                    case ValType.Double: return value.DoubleValue;
                    case ValType.String: return value.StringValue;
                    case ValType.Json: return value.JsonValue;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private void Log(string message)
        {
            if (_config.IsLoggingEnabled) Debug.Log($"[FBRC] {message}");
        }
        
        private void LogError(string message)
        {
            if (_config.IsLoggingEnabled) Debug.LogError($"[FBRC] {message}");
        }
    }
}