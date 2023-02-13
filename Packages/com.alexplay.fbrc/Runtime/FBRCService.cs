using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACS.FBRC.StaticData;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace ACS.FBRC
{
    public class FBRCService
    {
        private readonly FirebaseRemoteConfig _remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        private readonly FBRCStaticData _staticData;

        public FBRCService(FBRCStaticData staticData)
        {
            _staticData = staticData;
            SetDefaults();
            ExecuteLoadStrategy();
        }

        public Task EnsureInitializedAsync() => 
            _remoteConfig.EnsureInitializedAsync();

        private async void ExecuteLoadStrategy()
        {
            switch (_staticData.LoadStrategy)
            {
                case LoadStrategy.ActivateThenLoadAsync:
                    await ActivateAsync();
                    await FetchAsync();
                    break;
                case LoadStrategy.LoadThenActivateAsync:
                    await FetchAsync();
                    await ActivateAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTask<bool> FetchAsync()
        {
            var fetchAsync = _remoteConfig.FetchAsync(TimeSpan.Zero);
            await fetchAsync;

            if (_remoteConfig.Info.LastFetchStatus == LastFetchStatus.Success) return true;
            Debug.LogError($"Fetching data was unsuccessful\n{nameof(_remoteConfig.Info.LastFetchStatus)}: {_remoteConfig.Info.LastFetchStatus}");
            return false;
        }

        private async UniTask ActivateAsync()
        {
            var activateAsync = _remoteConfig.ActivateAsync();
            await activateAsync;
            Debug.Log($"Remote data loaded and ready for use. Last fetch time {_remoteConfig.Info.FetchTime}.");
        }

        private void SetDefaults()
        {
            var def = new Dictionary<string, object>();

            foreach (FBRemoteConfigValue value in _staticData.Values) 
                def.Add(value.Name, GetValue(value));

            _remoteConfig.SetDefaultsAsync(def);

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
    }
}