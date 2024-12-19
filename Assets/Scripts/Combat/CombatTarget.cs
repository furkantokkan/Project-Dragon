using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using RPG.Core;
using RPG.Resources;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController sender)
        {
            var fightHandler = sender.GetComponent<FightHandler>();

            if (!fightHandler.CanAttack(gameObject))
            {
                return false;
            }

            if (Input.GetMouseButton(1))
            {
                fightHandler.Attack(gameObject);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }
    }

}
