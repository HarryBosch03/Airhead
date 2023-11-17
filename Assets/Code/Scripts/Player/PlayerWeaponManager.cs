using System.Collections.Generic;
using UnityEngine;

namespace Airhead.Runtime.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        public string[] equippedWeapons;

        private int currentWeaponIndex;
        private List<PlayerWeapon> registeredWeapons = new();

        public PlayerWeapon CurrentWeapon => currentWeaponIndex >= 0 && currentWeaponIndex < registeredWeapons.Count ? registeredWeapons[currentWeaponIndex] : null;
        
        private void Awake()
        {
            var parent = transform.Find("View/Weapons");
            foreach (Transform e in parent)
            {
                var weapon = e.GetComponent<PlayerWeapon>();
                if (!weapon) continue;
                registeredWeapons.Add(weapon);
                weapon.gameObject.SetActive(false);
            }

            EquipWeapons(0);
        }

        private int NameToIndex(string name)
        {
            for (var i = 0; i < registeredWeapons.Count; i++)
            {
                var other = registeredWeapons[i].name;
                if (other != name) continue;
                return i;
            }
            return -1;
        }
        
        private void EquipWeapons(int i)
        {
            if (CurrentWeapon) CurrentWeapon.gameObject.SetActive(false);
            currentWeaponIndex = NameToIndex(equippedWeapons[i]);
            if (CurrentWeapon) CurrentWeapon.gameObject.SetActive(true);
        }
    }
}
