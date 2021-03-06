using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(PlayerReviveController), typeof(WeaponController), typeof(NextPlayersAnalytics))]
public class PlayerAnalytics : MonoBehaviour
{
    public static string playerName;

    float startGameTime;// = Time.time;

    string mostTimeAreaName;
    string currentAreaName;
    float mostTimeAreaTime;
    float startMostTimeAreaTime;

    [HideInInspector] public int revivedPlayers;
    [HideInInspector] public int revivedByPlayers;

    [HideInInspector] public int kodByPlayers;
    [HideInInspector] public int kodByEnemies;

    [HideInInspector] public int shotsMade;

    [HideInInspector] public int hittedEnemies;
    [HideInInspector] public int killedEnemies;

    [HideInInspector] public int hittedPlayers;
    //[HideInInspector] public int killedPlayers;
    [HideInInspector] public int kodPlayers;

    void Start()
    {
        startGameTime = Time.time;
    }

    public void SendDieAnalytics()
    {
        SendStatiscsAnalytics();
        //SendStatiscsAnalytics("Morreu");
    }

    void RefreshToNewMostTimeArea()
    {
        float _time = Time.time - startMostTimeAreaTime;
        if (_time > mostTimeAreaTime)
        {
            mostTimeAreaTime = _time;
            mostTimeAreaName = currentAreaName;
        }
    }

    void SendStatiscsAnalytics()
    {
        RefreshToNewMostTimeArea();

        int missShots = shotsMade - hittedPlayers - hittedEnemies;

        AnalyticsResult result1 = Analytics.CustomEvent("Dados basicos", new Dictionary<string, object>()
        {
            { "Nome do jogador", playerName },
            { "Posicao", transform.position },
            { "Tempo de jogo", Time.time - startGameTime },
            { "Area onde passou mais tempo", mostTimeAreaName },
        });
        print("Dados básicos | Resultado: " + result1);

        AnalyticsResult result2 = Analytics.CustomEvent("Interacao com jogadores", new Dictionary<string, object>()
        {
            { "Nome do jogador", playerName },
            { "Revivido por jogadores", revivedByPlayers },
            { "Reviveu jogadores", revivedPlayers },
            { "Nocauteado por jogadores", kodByPlayers },
            { "Nocauteado por inimigos", kodByEnemies },
            { "Tiros nao acertados", missShots },
            { "Acerto em jogadores", hittedPlayers },
            { "Nocaute em jogadores", kodPlayers },
            { "Acerto em inimigos", hittedEnemies },
            { "Matou inimigos", killedEnemies }
        });
        print("Interação com jogadores | Resultado: " + result2);

        GetComponent<NextPlayersAnalytics>().FinishAndSendAnalytics();
    }

    /*void SendAnalytics(string eventName)
    {
        AnalyticsResult result = Analytics.CustomEvent(eventName);
        print(eventName + " | " + result);
    }

    void SendAnalytics(string eventName, IDictionary<string, object> arguments)
    {
        AnalyticsResult result = Analytics.CustomEvent(eventName, arguments);
        print(eventName + " | " + result);
}*/

    void OnTriggerEnter(Collider c)
    {
        RegionArea area = c.GetComponentInParent<RegionArea>();
        if (area && area.regionName != currentAreaName)
        {
            RefreshToNewMostTimeArea();

            startMostTimeAreaTime = Time.time;
            currentAreaName = area.regionName;
        }

    }

    void OnApplicationQuit()
    {
        SendStatiscsAnalytics();
        //SendStatiscsAnalytics("Saiu");
    }

    void OnDestroy()
    {
        Destroy(GetComponent<NextPlayersAnalytics>());
    }
}
