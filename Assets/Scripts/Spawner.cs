using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Spawner : GlobalEventListener
{
    public GameObject spawnObject;
    public int spawnObjectLenght = 4;
    public int spawnAmountPerInterval = 3;
    public float spawnWaitTimeAfterInterval = 3f;
    public float safeWaitTime = 5f;

    BattleManager battleManager;
    //WaveHUD waveHud;

    WaveEvent waveEvent;
    float count;
    int currentWave;
    bool isSpawning;

    void Start()
    {
        if (!BoltNetwork.IsServer)
        {
            enabled = false;
            return;
        }

        waveEvent = WaveEvent.Create();
        //RefreshAndSendEvent();

        battleManager = FindObjectOfType<BattleManager>();
        //waveHud = FindObjectOfType<WaveHUD>();

        count = safeWaitTime;
    }

    void Update()
    {
        /*if (!BoltNetwork.IsRunning)
            return;*/

        if (!isSpawning && battleManager.NPCs.Count == 0)//if (count <= 0f)
        {
            count -= BoltNetwork.FrameDeltaTime;
            if (count > 0f)
                return;

            StartCoroutine(Spawn());
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;

        currentWave++;
        RefreshAndSendEvent();
        //waveHud.RefreshTextString(currentWave);

        int spawnAmount = 0;

        for (int i = 0; i < spawnObjectLenght * (battleManager.players.Count + 1) * currentWave; i++)
        {
            spawnAmount++;

            GameObject spawned = BoltNetwork.Instantiate(spawnObject, transform.position + (Vector3.left * 3.5f * spawnAmount), Quaternion.identity);
            EnemyCharacterController enemy = spawned.GetComponent<EnemyCharacterController>();
            if (enemy)
                enemy.baseTransform = transform;

            if (spawnAmount == spawnAmountPerInterval)
            {
                spawnAmount = 0;
                yield return new WaitForSeconds(spawnWaitTimeAfterInterval);
            }
        }

        count = safeWaitTime;

        isSpawning = false;
    }

    void RefreshAndSendEvent()
    {
        waveEvent.CurrentWave = currentWave;
        waveEvent.Send();
    }

    public override void EntityAttached(BoltEntity entity)
    {
        if (enabled && !entity.IsOwner)
            waveEvent.Send();
    }
}
