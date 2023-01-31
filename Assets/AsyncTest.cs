using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class AsyncTest : MonoBehaviour
{
    private CancellationTokenSource _cts;
    private UniTask _task;

    private void Awake() => _cts = new CancellationTokenSource();
    
    private void Start() => _task = UniTask.Create(() => Go(10));

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) 
            _cts.Cancel();
    }

    private async UniTask Go(int i)
    {
        while (true)
        {
            Debug.LogWarning("YO");
            await UniTask.Yield(cancellationToken: _cts.Token);
        }
    }

    [Button] private void ShowStatus() => Debug.Log(_task.Status);
}
