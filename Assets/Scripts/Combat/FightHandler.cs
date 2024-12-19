using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Resources;
using RPG.Stats;
using System.Collections.Generic;
using Utils;
using System;

namespace RPG.Combat
{
    public class FightHandler : MonoBehaviour, IAction, ISaveable, IModiferProvider
    {
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;

        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";
        LazyValue<Weapon> currentWeapon;

        private MoveHandler moveHandler;
        private float timeSinceLastAttack = Mathf.Infinity;

        private Health target;
        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            moveHandler = GetComponent<MoveHandler>();
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }
        private void Start()
        {
            currentWeapon.ForceInit();
        }
        private void Update()
        {

            timeSinceLastAttack += Time.deltaTime;

            if (target != null && target.IsAlive())
            {
                if (!InRange())
                {
                    moveHandler.MoveTo(target.transform.position, 1f); // max speed
                }
                else
                {
                    moveHandler.Cancel();
                    AttackBehaviour();
                }
            }

        }
        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }
        public void EquipWeapon(Weapon weaponType)
        {
            currentWeapon.value = weaponType;
            AttachWeapon(weaponType);
        }

        private void AttachWeapon(Weapon weaponType)
        {
            if (anim == null)
            {
                anim = GetComponent<Animator>();
            }
            weaponType.Spawn(rightHandTransform, leftHandTransform, anim);
        }

        public Health GetTarget()
        {
            return target;
        }
        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            if (timeSinceLastAttack >= currentWeapon.value.GetAttackRate)
            {
                anim.ResetTrigger("StopAttack");
                anim.SetTrigger("Attack");
                timeSinceLastAttack = 0f;
            }
        }
        private bool InRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.value.GetWeaponRange;
        }
        public bool CanAttack(GameObject newTarget)
        {
            if (newTarget == null)
            {
                return false;
            }
            Health tempTarget = newTarget.GetComponent<Health>();
            return tempTarget != null && tempTarget.IsAlive();
        }
        public void Attack(GameObject newTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = newTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            moveHandler.Cancel();
        }
        public IEnumerable<float> GetAdditiveModifers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetWeaponDamage;
            }
        }
        public IEnumerable<float> GetPercentageModifers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus;
            }
        }
        public void Hit()
        {
            if (target != null)
            {
                float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

                if (currentWeapon.value.HasProjectile())
                {
                    currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, currentWeapon.value.GetProjectileSpeed, damage);
                }
                else
                {
                    target.TakeDamage(gameObject, damage);
                }
            }
        }
        public void Shoot()
        {
            Hit();
        }
        public void StopAttack()
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("StopAttack");
        }

        public object Capture()
        {
            return currentWeapon.value.name;
        }

        public void Restore(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}

