using System.Collections.Generic;
using System.Linq;
using Airhead.Runtime.Entities;
using UnityEngine;

namespace Airhead.Runtime.Player
{
    public abstract class PlayerWeapon : MonoBehaviour
    {
        public const int ViewportLayer = 3;

        public Camera MainCam { get; private set; }
        public PlayerAvatar Player { get; private set; }

        public virtual string DisplayName => name;
        public bool UseFlag { get; set; }
        public abstract string AmmoString { get; }
        public abstract bool IsReloading { get; }
        public abstract float ReloadPercent { get; }
        
        protected virtual void Awake()
        {
            MainCam = Camera.main;
            Player = GetComponentInParent<PlayerAvatar>();
        }

        protected bool Raycast(Ray ray, out RaycastHit hit, float maxDistance)
        {
            hit = default;
            var list = Physics.RaycastAll(ray, maxDistance).OrderBy(e => e.distance);

            foreach (var e in list)
            {
                if (e.collider.transform.IsChildOf(Player.transform)) continue;
                
                hit = e;
                return true;
            }
            
            return false;
        }
    }
}
