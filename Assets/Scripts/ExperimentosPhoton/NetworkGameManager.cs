using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkGameManager : GlobalEventListener
{
    [SerializeField] GameObject networkPlayerPrefab;

    public override void SceneLoadLocalDone(string scene, IProtocolToken token)
    {
        BoltNetwork.Instantiate(networkPlayerPrefab, new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)), Quaternion.identity);
    }
}
