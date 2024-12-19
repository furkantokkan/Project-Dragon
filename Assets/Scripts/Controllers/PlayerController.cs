using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    [RequireComponent(typeof(FightHandler))]
    public class PlayerController : MonoBehaviour
    {
        enum CursorType
        {
            None,
            Movement,
            Combat,
            UI
        }
        private FightHandler fightHandler;

        private Health health;

        private bool isInTrnasation = false;
        public bool GetIsInTransation { get { return isInTrnasation; } set { isInTrnasation = value; } }
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        [SerializeField] CursorMapping[] cursorMappings = null;


        private void Awake()
        {
            fightHandler = GetComponent<FightHandler>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            if (isInTrnasation) return;
            if (InteractWithUI()) return;
            if (!health.IsAlive())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current == null) return false;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }

            return false;
        }

        public bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(DoRaycast());
            foreach (RaycastHit hit in hits)
            {
                CombatTargetHandler target = hit.transform.GetComponent<CombatTargetHandler>();
                if (target == null)
                {
                    continue;
                }
                if (!fightHandler.CanAttack(target.gameObject))
                {
                    continue;
                }
                if (Input.GetMouseButton(1))
                {
                    fightHandler.Attack(target.gameObject);
                }
                SetCursor(CursorType.Combat);
                return true;
            }
            return false;
        }
        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(DoRaycast(), out hit);
            if (hasHit && Vector3.Distance(transform.position, hit.point) >= 1f)
            {
                if (Input.GetMouseButton(1))
                {
                    if (hasHit)
                    {
                        GetComponent<MoveHandler>().StartMoveAction(hit.point, 1f ); //max speed
                    }
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private Ray DoRaycast()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

