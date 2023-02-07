using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Config
{
    [Serializable]
    public abstract class ServiceConfigBase
    {
        protected abstract string PackageName { get; }
        public event Action Enabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
        [HideInInspector] [SerializeField] private bool _isEnabled;
        private ListRequest _listRequest;
        private PackageInfo _packageInfo;
        private RemoveRequest _removeRequest;
        private AddRequest _addRequest;

        [Button(ButtonSizes.Medium)]
        [ShowIf("@_isEnabled == false")]
        [GUIColor(0, 1, 0)]
        public void Enable()
        {
            _isEnabled = true;
            
            Enabled?.Invoke();
        }

        [ShowIf("@_isEnabled == true")]
        [Button(ButtonSizes.Medium)]
        [GUIColor(1, 0, 0)]
        public void Disable() => 
            _isEnabled = false;

        [Button]
        public void Reinstall()
        {
            if (_listRequest != null && _listRequest.Status == StatusCode.InProgress) return;
            if (_removeRequest != null && _listRequest.Status == StatusCode.InProgress) return;
            _listRequest = Client.List();
            EditorApplication.update += SearchCallback;
        }

        private void SearchCallback()
        {
            if (!_listRequest.IsCompleted) return;
            
            EditorApplication.update -= SearchCallback;
            if (_listRequest.Status != StatusCode.Success)
            {
                _listRequest = null;
                return;
            }

            _packageInfo = _listRequest.Result.FirstOrDefault(r => r.name.Equals(PackageName));
            if (_packageInfo == null)
            {
                return;
            }

            _removeRequest = Client.Remove(_packageInfo.name);
            EditorApplication.update += RemoveCallback;
        }

        private void RemoveCallback()
        {
            if (_removeRequest.IsCompleted == false) return;
            EditorApplication.update -= RemoveCallback;

            if (_removeRequest.Status != StatusCode.Success)
            {
                _listRequest = null;
                return;
            }

            _addRequest = Client.Add(_packageInfo.repository.url);
            EditorApplication.update += AddCallback;
        }

        private void AddCallback()
        {
            if (_addRequest.IsCompleted == false) return;
            EditorApplication.update -= AddCallback;
            Debug.Log(_addRequest.Status);
            _listRequest = null;
        }
    }
}