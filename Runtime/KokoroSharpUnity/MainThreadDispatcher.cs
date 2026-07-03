using UnityEngine;
using System.Collections.Concurrent;
using System;

namespace KokoroSharpUnity
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private readonly ConcurrentQueue<Action> mainThreadActions = new();
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        private void Update()
        {
            if (mainThreadActions.TryDequeue(out Action action))
            {
                action.Invoke();
            }
        }
        public void Enqueue(Action action)
            => mainThreadActions.Enqueue(action);
    }
}