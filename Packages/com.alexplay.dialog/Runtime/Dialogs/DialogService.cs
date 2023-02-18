using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ACS.Dialog.Dialogs.Arguments;
using ACS.Dialog.Dialogs.Config;
using ACS.Dialog.Dialogs.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ACS.Dialog.Dialogs
{
    public class DialogService : IDialogService, IDisposable
    {
        public event Action<int> OnCountChanged;
        
        public bool HasActiveDialog
        {
            get => _activeDialogs.Count > 0;
            set
            {
                
            }
        }

        private readonly ObservableCollection<DialogView> _activeDialogs = new ObservableCollection<DialogView>();
        
        private readonly DiContainer _diContainer;
        private readonly DialogsServiceConfig _dialogsServiceConfig;
        private readonly RectTransform _dialogsParent;
        
        public DialogService(DiContainer diContainer, DialogsServiceConfig dialogsServiceConfig, RectTransform rectForDialogs)
        {
            _diContainer = diContainer;
            _dialogsServiceConfig = dialogsServiceConfig;
            _dialogsParent = rectForDialogs;
            _activeDialogs.CollectionChanged += OnDialogsCountChanged;
        }
        
        public void PrepareService()
        {
            Canvas dialogsCanvas = _dialogsParent.GetComponent<Canvas>();
            dialogsCanvas.sortingLayerName = _dialogsServiceConfig.GetLayerName();
            dialogsCanvas.sortingOrder = _dialogsServiceConfig.DialogSortingOrder;
        }

        public async void CallDialog(Type dialogType) => 
            ShowDialog(await InstantiateDialog<DialogArgs>(dialogType, null));

        public async void CallDialog<TArgs>(Type dialogType, TArgs args) where TArgs : DialogArgs => 
            ShowDialog(await InstantiateDialog<TArgs>(dialogType, args));

        private async UniTask<DialogView> InstantiateDialog<TArgs>(Type dialogType, TArgs args) where TArgs : DialogArgs
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(_dialogsServiceConfig.DefaultResources + dialogType.Name);
            GameObject dialogPrefab = await resourceRequest as GameObject;
            
            if (dialogPrefab != null)
            {
                DialogView instance = _diContainer.InstantiatePrefab(dialogPrefab).GetComponent<DialogView>();
                (instance as IReceiveArgs<TArgs>).SetArgs(args);
                return instance;
            }

            throw new Exception("You try to instantiate dialog that has no instance or contains name not equal to its type");
        }

        private void ShowDialog(DialogView dialogView)
        {
            dialogView
                .AddShownHandler(OnDialogShown)
                .AddHiddenHandler(OnDialogHidden);
            
            dialogView.SetParent(_dialogsParent);
            dialogView.Show();
        }
        
        private void OnDialogShown(DialogView dialogView) => _activeDialogs.Add(dialogView);

        private void OnDialogHidden(DialogView dialogView) => _activeDialogs.Remove(dialogView);

        private void OnDialogsCountChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    OnCountChanged?.Invoke(e.NewItems.Count);
                    break;
            }
        }

        public void Dispose()
        {
            _activeDialogs.CollectionChanged -= OnDialogsCountChanged;
        }
    }
}
