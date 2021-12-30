using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Spawner : GlobalEventListener
{
    public GameObject spawnObject;
    public int spawnObjectLenght = 4;
    public float safeWaitTime = 5f;

    BattleManager battleManager;
    //WaveHUD waveHud;

    WaveEvent waveEvent;
    float count;
    int currentWave;

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

        if (battleManager.NPCs.Count == 0)//if (count <= 0f)
        {
            count -= BoltNetwork.FrameDeltaTime;
            if (count > 0f)
                return;
                
            currentWave++;
            RefreshAndSendEvent();
            //waveHud.RefreshTextString(currentWave);
            
            for (int i = 0; i < spawnObjectLenght * battleManager.players.Count * currentWave; i++)
            {
                GameObject spawned = BoltNetwork.Instantiate(spawnObject, transform.position + (Vector3.left * 3.5f * i), Quaternion.identity);
                EnemyCharacterController enemy = spawned.GetComponent<EnemyCharacterController>();
                if (enemy)
                    enemy.baseTransform = transform;
            }

            count = safeWaitTime;
        }
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
