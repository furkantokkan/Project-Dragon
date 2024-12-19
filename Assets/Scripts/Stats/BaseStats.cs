using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] private GameObject levelUpParticule;
        [SerializeField] private bool shouldUseModifers = false;

        LazyValue<int> currentLevel;

        public event Action onLevelUp;

        private Experience experience;
        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }
        private void Start()
        {
            currentLevel.ForceInit();
        }
        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }
        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                onLevelUp?.Invoke();
                LevelUpEffect();
            }
        }
        private void LevelUpEffect()
        {
            Instantiate(levelUpParticule, transform);
        }

        public float GetStat(Stat newStat)
        {
            return (GetBaseStat(newStat) + GetAdditiveModifer(newStat)) * (1 + GetPercentageModifier(newStat) / 100);
        }

        private float GetBaseStat(Stat newStat)
        {
            return progression.GetStat(newStat, characterClass, GetLevel());
        }

        private float GetAdditiveModifer(Stat newStat)
        {
            if (!shouldUseModifers)
            {
                return 0f;
            }

            float total = 0f;
            foreach (IModiferProvider provider in GetComponents<IModiferProvider>())
            {
                foreach (float modifer in provider.GetAdditiveModifers(newStat))
                {
                    total += modifer;
                }
            }
            return total;
        }
        private float GetPercentageModifier(Stat newStat)
        {
            if (!shouldUseModifers)
            {
                return 0f;
            }

            float total = 0f;
            foreach (IModiferProvider provider in GetComponents<IModiferProvider>())
            {
                foreach (float modifer in provider.GetPercentageModifers(newStat))
                {
                    total += modifer;
                }
            }
            return total;
        }
        public int GetLevel()
        {
            //if (currentLevel.value < 1)
            //{
            //    currentLevel.value = CalculateLevel();
            //}
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null)
            {
                return startingLevel;
            }

            float currentXP = experience.GetPoints();
            int levelIndex = progression.GetLevelLength(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level < levelIndex; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (xpToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return levelIndex;
        }
    }
}

