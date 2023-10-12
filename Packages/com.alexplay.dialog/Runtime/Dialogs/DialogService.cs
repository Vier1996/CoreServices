using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using ACS.Dialog.Dialogs.Arguments;
using ACS.Dialog.Dialogs.Config;
using ACS.Dialog.Dialogs.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#if COM_ALEXPLAY_ZENJECT_EXTENSION
using Zenject;
#endif

namespace ACS.Dialog.Dialogs
{
    public class DialogService : IDialogService, IDisposable
    {
        public event Action<Type> DialogShown;
        public event Action<Type> DialogHide;
        public event Action<int> OnCountChanged;
        public event Action<RenderMode> RenderModeChanged;

        public bool HasActiveDialog
        {
            get => _activeDialogs.Count > 0;
            set
            {
                
            }
        }

        private readonly ObservableCollection<DialogView> _activeDialogs = new ObservableCollection<DialogView>();
        
#if COM_ALEXPLAY_ZENJECT_EXTENSION
        private readonly DiContainer _projectContextContainer;
        private DiContainer _sceneContextContainer;
#endif
        
        private readonly DialogsServiceConfig _dialogsServiceConfig;
        private readonly RectTransform _dialogsParent;
        private GameObject _raycastLocker;
        
#if COM_ALEXPLAY_ZENJECT_EXTENSION
        public DialogService(DiContainer diContainer, DialogsServiceConfig dialogsServiceConfig, RectTransform rectForDialogs)
        {
            _projectContextContainer = diContainer;
            _dialogsServiceConfig = dialogsServiceConfig;
            _dialogsParent = rectForDialogs;
            _activeDialogs.CollectionChanged += OnDialogsCountChanged;
            SceneManager.activeSceneChanged += OnSceneChanged;

            OnSceneChanged(default, default);
        }
#endif
     
        public DialogService(DialogsServiceConfig dialogsServiceConfig, RectTransform rectForDialogs)
        {
            _dialogsServiceConfig = dialogsServiceConfig;
            _dialogsParent = rectForDialogs;
            _activeDialogs.CollectionChanged += OnDialogsCountChanged;
            SceneManager.activeSceneChanged += OnSceneChanged;

            OnSceneChanged(default, default);
        }

        public void PrepareService() => CreateRaycastLocker();

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
        
        public async void CallDialog(Type dialogType, SourceContext context = SourceContext.PROJECT_CONTEXT) => 
            ShowDialog(await InstantiateDialog<DialogArgs>(dialogType, null, context));

        public async void CallDialog<TArgs>(Type dialogType, TArgs args, SourceContext context = SourceContext.PROJECT_CONTEXT) 
            where TArgs : DialogArgs => 
            ShowDialog(await InstantiateDialog<TArgs>(dialogType, args, context));

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

        public void ChangeRenderMode(RenderMode mode) => RenderModeChanged?.Invoke(mode);

        public void CloseAllDialogs()
        {
            while (_activeDialogs.Count > 0) 
                _activeDialogs[0].Hide();
        }

        private async UniTask<DialogView> InstantiateDialog<TArgs>(Type dialogType, TArgs args, SourceContext context = SourceContext.PROJECT_CONTEXT) where TArgs : DialogArgs
        {
            _raycastLocker.gameObject.SetActive(true);

            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(_dialogsServiceConfig.DefaultResources + dialogType.Name);
            GameObject dialogPrefab = await resourceRequest as GameObject;
#if COM_ALEXPLAY_ZENJECT_EXTENSION
            DiContainer container = null;

            switch (context)
            {
                case SourceContext.SCENE_CONTEXT: container = _sceneContextContainer; break;
                case SourceContext.PROJECT_CONTEXT: container = _projectContextContainer; break;
            }

            if (dialogPrefab != null) 
            {
                if (container == null)
                    container = _projectContextContainer;
                
                DialogView instance = container.InstantiatePrefab(dialogPrefab).GetComponent<DialogView>();
                (instance as IReceiveArgs<TArgs>).SetArgs(args);
                return instance;
            }
#else
            if (dialogPrefab != null) 
            {
                DialogView instance = Object.Instantiate(dialogPrefab).GetComponent<DialogView>();
                (instance as IReceiveArgs<TArgs>).SetArgs(args);
                return instance;
            }
#endif
            throw new Exception("You try to instantiate dialog that has no instance or contains name not equal to its type");
        }

        private void ShowDialog(DialogView dialogView)
        {
            dialogView
                .AddShownHandler(OnDialogShown)
                .AddHiddenHandler(OnDialogHidden);
        
            _raycastLocker.gameObject.SetActive(false);

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
            _activeDialogs.Remove(dialogView);
            DialogHide?.Invoke(dialogView.GetType());
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
        
        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
#if COM_ALEXPLAY_ZENJECT_EXTENSION
            _sceneContextContainer = UnityEngine.Object.FindObjectOfType<SceneContext>().Container;
#endif
        }

        public void Dispose()
        {
            _activeDialogs.CollectionChanged -= OnDialogsCountChanged;
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }
    }
    
    public enum SourceContext { SCENE_CONTEXT, PROJECT_CONTEXT }
}
