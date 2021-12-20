using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnObject;
    public int spawnObjectLenght = 4;
    //public GameObject[] spawnObjects;
    public float safeWaitTime = 5f;
    /*public Vector3 minSpawnOffset = Vector3.zero;
    public Vector3 maxSpawnOffset = Vector3.one * 1.5f;*/
    //public bool oneShot = true; // Provavelmente bugará se não for oneShot

    BattleManager battleManager;
    WaveHUD waveHud;
    float count;
    int currentWave;

    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        waveHud = FindObjectOfType<WaveHUD>();

        count = safeWaitTime;

        if (!BoltNetwork.IsServer)
            enabled = false;
    }

    void Update()
    { 
        if (battleManager.NPCs.Count == 0)//if (count <= 0f)
        {
            count -= BoltNetwork.FrameDeltaTime;
            if (count > 0f)
                return;
                
            currentWave++;
            waveHud.RefreshTextString(currentWave);

            for (int i = 0; i < spawnObjectLenght * battleManager.players.Count * currentWave; i++)
            {
                /*Vector3 spawnPosition = transform.position +
                                        new Vector3(Random.Range(minSpawnOffset.x, maxSpawnOffset.x),
                                                    Random.Range(minSpawnOffset.y, maxSpawnOffset.y),
                                                    Random.Range(minSpawnOffset.z, maxSpawnOffset.z));*/


                GameObject spawned = BoltNetwork.Instantiate(spawnObject, transform.position + (Vector3.left * 3.5f * i), Quaternion.identity);
                EnemyCharacterController enemy = spawned.GetComponent<EnemyCharacterController>();
                if (enemy)
                    enemy.baseTransform = transform;
            }

            /*foreach (GameObject go in spawnObjects)
            {
                GameObject spawned = BoltNetwork.Instantiate(go, transform.position, Quaternion.identity);
                EnemyCharacterController enemy = spawned.GetComponent<EnemyCharacterController>();
                if (enemy)
                    enemy.baseTransform = transform;
            }*/

            count = safeWaitTime;

            /*if (oneShot)
                enabled = false;*/
        }
    }
}
