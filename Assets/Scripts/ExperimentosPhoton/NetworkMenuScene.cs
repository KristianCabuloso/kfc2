using Photon.Bolt;
using Photon.Bolt.Matchmaking;
using System;
using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public class NetworkMenuScene : GlobalEventListener
{
    public void Command_StartServer()
    {
        BoltLauncher.StartServer();
    }

    public void Command_StartClient()
    {
        BoltLauncher.StartClient();
    }

    public override void BoltStartDone()
    {
        // 1234 = qualquer número. não sei q colocar por enquanto
        BoltMatchmaking.CreateSession("1234", sceneToLoad: "TestNetworkGameScene");
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        foreach (KeyValuePair<Guid, UdpSession> session in sessionList)
        {
            UdpSession photonSession = session.Value;
            if (photonSession.Source == UdpSessionSource.Photon)
            {
                BoltMatchmaking.JoinSession(photonSession);
            }
        }
    }
}
