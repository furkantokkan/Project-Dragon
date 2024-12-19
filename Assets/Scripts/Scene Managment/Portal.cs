using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;
using RPG.SceneManagement;
public class Portal : MonoBehaviour
{
    enum DestinationIdentifier
    {
        Forest,
        TownEnterence,
    }
    public int sceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentifier destination;
    [SerializeField] float fadeOutTime = 1f;
    [SerializeField] float fadeInTime = 1f;
    [SerializeField] float fadeWaitTime = 0.5f;
    private GameObject newPlayer;


    public static bool routineWorking = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (sceneToLoad == -1)
            {
                return;
            }
            else
            {
                if (!routineWorking)
                {
                    StartCoroutine(Transition());
                }
            }
        }
    }

    public IEnumerator Transition()
    {
        if (sceneToLoad < 0)
        {
            Debug.LogError("Scene to load not set.");
            yield break;
        }
        routineWorking = true;
        DontDestroyOnLoad(this.gameObject);

        Fader fader = FindObjectOfType<Fader>();
        SavingWarpper savingWarper = FindObjectOfType<SavingWarpper>();
        yield return fader.FadeOut(fadeOutTime); // 0-1

        savingWarper.Save();

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        savingWarper.Load();

        newPlayer = GameObject.FindWithTag("Player");
        newPlayer.GetComponent<PlayerController>().GetIsInTransation = true;

        Portal otherPortal = GetOtherPortal();
        UpdateSpawnPosition(otherPortal);

        savingWarper.Save();

        yield return new WaitForSeconds(fadeWaitTime); // Delay

        newPlayer.GetComponent<PlayerController>().GetIsInTransation = false;
        yield return fader.FadeIn(fadeInTime); // 1-0
        routineWorking = false;
        Destroy(this.gameObject);
    }

    private void UpdateSpawnPosition(Portal otherPortal)
    {
        newPlayer.GetComponent<NavMeshAgent>().enabled = false;
        newPlayer.transform.position = otherPortal.spawnPoint.transform.position;
        newPlayer.transform.rotation = otherPortal.spawnPoint.rotation;
        newPlayer.GetComponent<NavMeshAgent>().enabled = true;
    }

    private Portal GetOtherPortal()
    {
        foreach (Portal portal in FindObjectsOfType<Portal>())
        {
            if (portal == this)
            {
                continue;
            }
            if (portal.destination != destination)
            {
                continue;
            }
            return portal;
        }
        return null;
    }
}
