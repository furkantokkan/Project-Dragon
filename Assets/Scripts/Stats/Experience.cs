using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoints = 0f;

        //public delegate void ExperienceGainedDelegate();
        //public event ExperienceGainedDelegate onExperienceGained;

        public event Action onExperienceGained;

        public void GainExperience(float amount)
        {
            experiencePoints += amount;
            onExperienceGained?.Invoke();
        }
        public float GetPoints()
        {
            return experiencePoints;
        }
        public object Capture()
        {
            return experiencePoints;
        }
        public void Restore(object state)
        {
            experiencePoints = (float)state;
        }
    }
}

