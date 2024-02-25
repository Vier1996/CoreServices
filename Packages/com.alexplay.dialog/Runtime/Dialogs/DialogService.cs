using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using ACS.Dialog.Dialogs.Arguments;
using ACS.Dialog.Dialogs.Config;
using ACS.Dialog.Dialogs.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ACS.Dialog.Dialogs
{
    public class DialogService : IDialogService, IDisposable
    {
        public event Action<Type> DialogShown;
        public event Action<Type> DialogHide;
        public event Action<int> OnCountChanged;

        public bool HasActiveDialog
        {
            get => _activeDialogs.Count > 0;
            set { }
        }

        private GameObject _raycastLocker;
        private readonly RectTransform _dialogsParent;
        private readonly DialogsServiceConfig _dialogsServiceConfig;
        private readonly ObservableCollection<DialogView> _activeDialogs = new ObservableCollection<DialogView>();
        private readonly Dictionary<string, AsyncOperationHandle<DialogView>> _dialogHandles = new Dictionary<string, AsyncOperationHandle<DialogView>>();

        private Action<RenderMode> _renderModeChangeDelegate;
     
        public DialogService(DialogsServiceConfig dialogsServiceConfig, RectTransform rectForDialogs)
        {
            _dialogsServiceConfig = dialogsServiceConfig;
            _dialogsParent = rectForDialogs;
            _activeDialogs.CollectionChanged += OnDialogsCountChanged;
            
            CreateRaycastLocker();
        }

        public DialogService AddRenderModeChangeDelegate(Action<RenderMode> renderModeChangeDelegate)
        {
            _renderModeChangeDelegate = renderModeChangeDelegate;

            return this;
        }

        private void CreateRaycastLocker()
        {
            _raycastLocker = new GameObject("RaycastLocker");
            _raycastLocker.transform.SetParent(_dialogsParent);
            _raycastLocker.gameObject.SetActive(false);
            
            RectTransform lockerRect = _raycastLocker.AddComponent<RectTransform>();
            lockerRect.sizeDelta = new Vector2(Screen.width, Screen.height) * 1.5f;
            
            Image lockerImage = _raycastLocker.AddComponent<Image>();
            lockerImage.color = Color.clear;
        }
        
        public async void CallDialog(Type dialogType) => 
            ShowDialog(await InstantiateDialog<DialogArgs>(dialogType, null));

        public async void CallDialog<TArgs>(Type dialogType, TArgs args) 
            where TArgs : DialogArgs =>
            ShowDialog(await InstantiateDialog(dialogType, args));

        public DialogView GetDialog(Type dialogType)
        {
            DialogView dialogView = _activeDialogs.FirstOrDefault(dlg => dlg.GetType() == dialogType);
            
            if (dialogView == default)
                return null;

            return dialogView;
        }
        
        public bool TryGetDialog<T>(out T dialog) where T : DialogView
        {
            DialogView dialogView = _activeDialogs.FirstOrDefault(dlg => dlg.GetType() == typeof(T));
            
            if (dialogView == default)
            {
                dialog = null;
                return false;
            }
            
            dialog = (T)dialogView;
            return true;
        }

        public void ChangeRenderMode(RenderMode mode) => _renderModeChangeDelegate?.Invoke(mode);

        public void CloseAllDialogs()
        {
            while (_activeDialogs.Count > 0) 
                _activeDialogs[0].Hide();
        }

        private async UniTask<DialogView> InstantiateDialog<TArgs>(Type dialogType, TArgs args) where TArgs : DialogArgs
        {
            _raycastLocker.gameObject.SetActive(true);
            
            DialogInfo dialogInfo = _dialogsServiceConfig.ActiveDialogs.FirstOrDefault(adt => adt.TypeFullName == dialogType.FullName);
            AsyncOperationHandle<DialogView> instantiateHandle = dialogInfo.AddressableReference.InstantiateAsync();
            
            await instantiateHandle.Task;
            
            _dialogHandles[dialogInfo.TypeFullName] = instantiateHandle;
            
            return ((IReceiveArgs<TArgs>)instantiateHandle.Result).SetArgs(args);
        }

        private void ShowDialog(DialogView dialogView)
        {
            dialogView
                .AddShownHandler(OnDialogShown)
                .AddHiddenHandler(OnDialogHidden);
        
            _raycastLocker.gameObject.SetActive(false);

            RectTransform dialogRect = (RectTransform)dialogView.transform;
            dialogRect.position = Vector3.zero;
            dialogRect.sizeDelta = _dialogsParent.sizeDelta;
            dialogView.SetParent(_dialogsParent);
            dialogView.Show();
        }
        
        private void OnDialogShown(DialogView dialogView)
        {
            _activeDialogs.Add(dialogView);
            DialogShown?.Invoke(dialogView.GetType());
        }

        private void OnDialogHidden(DialogView dialogView)
        {
            string typeName = dialogView.GetType().FullName;

            _activeDialogs.Remove(dialogView);

            Object.Destroy(dialogView.gameObject);

            ReleaseAssetReference(typeName);

            DialogHide?.Invoke(dialogView.GetType());
        }

        private void ReleaseAssetReference(string typeName)
        {
            if (typeName != null && _dialogHandles.TryGetValue(typeName, out AsyncOperationHandle<DialogView> handle))
            {
                _dialogHandles.Remove(typeName);
                
                Addressables.Release(handle);
            }
        }

        private void OnDialogsCountChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    OnCountChanged?.Invoke(_activeDialogs.Count);
                    break;
            }
        }

        public void Dispose() => _activeDialogs.CollectionChanged -= OnDialogsCountChanged;
    }
}
