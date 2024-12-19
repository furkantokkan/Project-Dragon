using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool alreadyTriggerd = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (!alreadyTriggerd)
                {
                    GetComponent<PlayableDirector>().Play();
                    alreadyTriggerd = true;
                }
            }
        }
    }
}
