using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;

    private static bool hasSpawned = false;

    private void Awake()
    {
        if (hasSpawned)
        {
            return;
        }
        SpawnPersistentObjects();
        hasSpawned = true;
    }

    private void SpawnPersistentObjects()
    {
        GameObject objectClone = Instantiate(objectPrefab);
        DontDestroyOnLoad(objectClone);
    }
}
