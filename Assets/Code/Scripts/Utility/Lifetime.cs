using System;
using UnityEngine;

namespace Airhead.Runtime.Utility
{
    public class Lifetime : MonoBehaviour
    {
        public float lifetime;

        private void Awake()
        {
            Destroy(gameObject, lifetime);
        }
    }
}