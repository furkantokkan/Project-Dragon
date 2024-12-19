using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Weapon weaponToTake = null;
        [SerializeField] private float respawnTime = 5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.GetComponent<FightHandler>());
            }
        }

        private void Pickup(FightHandler fighter)
        {
            fighter.EquipWeapon(weaponToTake);
            StartCoroutine(HideForSeconds(respawnTime));
        }

        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool show)
        {
            GetComponent<Collider>().enabled = show;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(show);
            }
        }

        public bool HandleRaycast(PlayerController sender)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(sender.GetComponent<FightHandler>());
            }

            return true;
        }
        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}
