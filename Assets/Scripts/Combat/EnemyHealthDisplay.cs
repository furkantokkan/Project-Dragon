using RPG.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        FightHandler fightHandler;
        private void Awake()
        {
            fightHandler = GameObject.FindWithTag("Player").GetComponent<FightHandler>();
        }
        private void Update()
        {
            if (fightHandler.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            Health health = fightHandler.GetTarget();
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints()); // sýfýrýncý elemana decimal deðer alma
        }
    }
}