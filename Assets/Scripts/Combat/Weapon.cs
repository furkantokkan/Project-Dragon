using RPG.Core;
using RPG.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] bool isRightHandedWeapon = true;
        [Header("Weapon Attributes")]
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float weaponDamage = 10f;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [Header("Ranged Weapon Attributes")]
        [SerializeField] Projectile projectile = null;
        [SerializeField] float projectileSpeed = 8f;

        public float GetPercentageBonus { get { return percentageBonus; } }
        public float GetWeaponDamage { get { return weaponDamage; } }
        public float GetWeaponRange { get { return weaponRange; } }
        public float GetAttackRate { get { return timeBetweenAttacks; } }
        public float GetProjectileSpeed { get { return projectileSpeed; } }

        private const string weaponName = "Weapon";
        public void Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator anim)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);

            if (weaponPrefab != null)
            {
                Transform handToSpawn = GetHandTransform(rightHandTransform, leftHandTransform);

                GameObject weaponClone = Instantiate(weaponPrefab, handToSpawn);
                weaponClone.name = weaponName;
            }
            if (animatorOverride != null)
            {
                anim.runtimeAnimatorController = animatorOverride;
            }
            else
            {
                var overrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
                if (overrideController != null)
                {
                    anim.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                }
            }
        }

        private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform oldWeapon = rightHandTransform.Find(weaponName);

            if (oldWeapon == null)
            {
                oldWeapon = leftHandTransform.Find(weaponName);
            }
            if (oldWeapon == null)
            {
                return;
            }
            oldWeapon.name = "DESTROY";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform handToSpawn;

            if (isRightHandedWeapon)
            {
                handToSpawn = rightHandTransform;
            }
            else
            {
                handToSpawn = leftHandTransform;
            }

            return handToSpawn;
        }
        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float speed, float calculatedDamage)
        {
            Projectile projectileClone = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileClone.SetTarget(target, instigator, calculatedDamage);
            projectileClone.SetSpeed(speed);
        }
    }
}

