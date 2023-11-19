using System;
using UnityEngine;

namespace Airhead.Runtime.Core
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instanceCache;

        public static T Instance
        {
            get
            {
                if (instanceCache) return instanceCache;
                instanceCache = new GameObject(typeof(T).Name).AddComponent<T>();
                return instanceCache;
            }
        }

        private void OnEnable()
        {
            instanceCache = this as T;
        }

        private void OnDisable()
        {
            if (instanceCache == this) instanceCache = null;
        }
    }
}