using System.Collections;
using Airhead.Runtime.Core;
using UnityEngine;

namespace Airhead.Runtime.Level.Props
{
    public class PropManager : Singleton<PropManager>
    {
        public void SpawnWithDelay(DamageableProp next)
        {
            StartCoroutine(routine());

            IEnumerator routine()
            {
                yield return new WaitForSeconds(next.respawnTime);
                next.gameObject.SetActive(true);
            }
        }
    }
}