using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Airhead.Runtime.Player
{
    public class SpawnPoint : MonoBehaviour
    {
        public static readonly List<SpawnPoint> All = new();
        public static readonly Pose Fallback = Pose.identity;

        private void OnEnable() { All.Add(this); }
        private void OnDisable() { All.Remove(this); }

        public bool Available() => true;

        public static Pose GetSpawnPoint()
        {
            if (All.Count == 0) return Fallback;
            
            var offset = Random.Range(0, All.Count);
            for (var i0 = 0; i0 < All.Count; i0++)
            {
                var i1 = (i0 + offset) % All.Count;
                var sp = All[i1];
                if (!sp.Available()) continue;
                
                return new Pose(sp.transform.position, sp.transform.rotation);
            }

            return Fallback;
        }
    }
}