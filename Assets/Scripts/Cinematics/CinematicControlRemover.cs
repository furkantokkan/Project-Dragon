using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
       private PlayableDirector playDirector;
       private GameObject player;

        private void Awake()
        {
            playDirector = GetComponent<PlayableDirector>();
            player = GameObject.FindWithTag("Player");
        }
        private void OnEnable()
        {
            playDirector.played += DisableControl; //director on start
            playDirector.stopped += EnableControl; //director on stop
        }
        private void OnDisable()
        {
            playDirector.played -= DisableControl; //director on start
            playDirector.stopped -= EnableControl; //director on stop
        }
        void DisableControl(PlayableDirector pd)
        {
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }
        void EnableControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
