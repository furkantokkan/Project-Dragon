using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Stats;
using System;
using Utils;
namespace RPG.Resources
{
    [System.Serializable]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 70;

        [SerializeField] LazyValue<float> healthPoints;
        
        private bool isAlive = true;

        private BaseStats baseStats;
        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitalHealth);
            //if (currentHealth < 0)
            //{
            //    currentHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            //}
            //baseStats = GetComponent<BaseStats>();
        }
        private void Start()
        {
            healthPoints.ForceInit();
        }
        private void OnEnable()
        {
            if (baseStats != null)
            {
                baseStats.onLevelUp += RegenerateHealth;
            }
        }
        private void OnDisable()
        {
            if (baseStats != null)
            {
                baseStats.onLevelUp -= RegenerateHealth;
            }
        }
        private float GetInitalHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public bool IsAlive()
        {
            return isAlive;
        }
        public object Capture()
        {
            return healthPoints.value;
        }
        public void Restore(object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value <= 0)
            {
                Die();
            }
            else
            {
                Revive();
            }
        }
        public void TakeDamage(GameObject attacker, float amount)
        {
            print(gameObject.name + " Took Damage: " + amount);

            healthPoints.value = Mathf.Max(healthPoints.value - amount, 0);
            if (healthPoints.value == 0 && isAlive)
            {
                Die();
                AwardExperience(attacker);
            }
        }
        public float GetHealthPoints()
        {
            return healthPoints.value;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null)
            {
                return;
            }
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }
        public void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }
        public float GetPercentage()
        {
            return 100 * (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }
        private void Die()
        {
            isAlive = false;
            GetComponent<Animator>().SetBool("Die", true);
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }
        private void Revive()
        {
            isAlive = true;
            GetComponent<Animator>().SetBool("Die", false);
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<CapsuleCollider>().enabled = true;
        }
    }
}

