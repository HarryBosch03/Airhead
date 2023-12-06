using System;
using System.Collections.Generic;
using Airhead.Runtime.Weapons;
using UnityEngine;

namespace Airhead.Runtime.Player
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        public string[] equippedWeapons;

        private int currentWeaponIndex;
        private List<PlayerWeapon> registeredWeapons = new();

        public bool UseFlag
        {
            get => CurrentWeapon && CurrentWeapon.UseFlag;
            set
            {
                if (CurrentWeapon) CurrentWeapon.UseFlag = value;
            }
        }
        
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

            EquipWeapon(0);
        }

        private void OnEnable()
        {
            EquipWeapon(0);
        }

        private void OnDisable()
        {
            EquipWeapon(-1);
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
        
        private void EquipWeapon(int i)
        {
            if (CurrentWeapon) CurrentWeapon.gameObject.SetActive(false);
            currentWeaponIndex = i != -1 ? NameToIndex(equippedWeapons[i]) : -1;
            if (CurrentWeapon) CurrentWeapon.gameObject.SetActive(true);
        }
    }
}
