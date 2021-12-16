using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] spawnObjects;
    public float waitTime = 5f;
    public bool oneShot = true; // Provavelmente bugará se não for oneShot

    float count;

    void Start()
    {
        count = waitTime;

        if (!BoltNetwork.IsServer)
            enabled = false;
    }

    void Update()
    {
        count -= BoltNetwork.FrameDeltaTime;

        if (count <= 0f)
        {
            foreach (GameObject go in spawnObjects)
            {
                GameObject spawned = BoltNetwork.Instantiate(go, transform.position, Quaternion.identity);
                EnemyCharacterController enemy = spawned.GetComponent<EnemyCharacterController>();
                if (enemy)
                    enemy.baseTransform = transform;
            }

            count = waitTime;

            if (oneShot)
                enabled = false;
        }
    }
}
