using System.Collections.Generic;
using ACS.Core;
using ACS.Core.ServicesContainer;
using ACS.SignalBus.SignalBus;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tests.SignalBus
{
    public class SignalBusTest : MonoBehaviour
    {
        public int CountLenght = 100000;

        private ISignalBusService signalBusService;
        
        private List<int> largeList_1 = new List<int>();
        private List<int> largeList_2 = new List<int>();
        private List<int> largeList_3 = new List<int>();
        private List<int> largeList_4 = new List<int>();
        private List<int> largeList_5 = new List<int>();

        private void OnEnable()
        {
            ServiceContainer.Core.Get(out signalBusService);

            signalBusService.Subscribe<RSignal>(GetLargeList);
            signalBusService.Subscribe<RSignal>(GetLargeList_2);
            signalBusService.Subscribe<RSignal>(GetLargeList_3);
            signalBusService.Subscribe<RSignal>(GetLargeList_4);
            signalBusService.Subscribe<RSignal>(GetLargeList_5);
        }
        
        [Button] private void Broadcast()
        {
            signalBusService.Fire(new RSignal());
        }
        
        private void GetLargeList(RSignal signal)
        {
            List<int> largeList = new List<int>(CountLenght);
            for (int i = 0; i < CountLenght; i++)
            {
                largeList.Add(i);
            }

            largeList_1 = largeList;

            Debug.Log("Recieved 1");
        }
        
        private void GetLargeList_2(RSignal signal)
        {
            List<int> largeList = new List<int>(CountLenght);
            for (int i = 0; i < CountLenght; i++)
            {
                largeList.Add(i);
            }
            
            largeList_2 = largeList;
            
            Debug.Log("Recieved 2");
        }
        
        private void GetLargeList_3(RSignal signal)
        {
            List<int> largeList = new List<int>(CountLenght);
            for (int i = 0; i < CountLenght; i++)
            {
                largeList.Add(i);
            }
            
            largeList_3 = largeList;
            
            Debug.Log("Recieved 3");
        }
        
        private void GetLargeList_4(RSignal signal)
        {
            List<int> largeList = new List<int>(CountLenght);
            for (int i = 0; i < CountLenght; i++)
            {
                largeList.Add(i);
            }
            
            largeList_4 = largeList;
            
            Debug.Log("Recieved 4");
        }
        
        private void GetLargeList_5(RSignal signal)
        {
            List<int> largeList = new List<int>(CountLenght);
            for (int i = 0; i < CountLenght; i++)
            {
                largeList.Add(i);
            }
            
            largeList_5 = largeList;
            
            Debug.Log("Recieved 5");
        }
        
        private void OnDisable()
        {
            signalBusService.Unsubscribe<RSignal>(GetLargeList);
            signalBusService.Unsubscribe<RSignal>(GetLargeList_2);
            signalBusService.Unsubscribe<RSignal>(GetLargeList_3);
            signalBusService.Unsubscribe<RSignal>(GetLargeList_4);
            signalBusService.Unsubscribe<RSignal>(GetLargeList_5);
        }
    }
}
