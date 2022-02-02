using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(PlayerReviveController), typeof(WeaponController), typeof(NextPlayersAnalytics))]
public class PlayerAnalytics : MonoBehaviour
{
    float startGameTime;// = Time.time;

    string mostTimeAreaName;
    string currentAreaName;
    float mostTimeAreaTime;
    float startMostTimeAreaTime;

    [HideInInspector] public int revivedPlayers;
    [HideInInspector] public int revivedByPlayers;

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
        SendStatiscsAnalytics("Morreu");
    }

    void RefreshToNewMosTimeArea()
    {
        float _time = Time.time - startMostTimeAreaTime;
        if (_time > mostTimeAreaTime)
        {
            mostTimeAreaTime = _time;
            mostTimeAreaName = currentAreaName;
        }
    }

    void SendStatiscsAnalytics(string eventName)
    {
        RefreshToNewMosTimeArea();

        AnalyticsResult result = Analytics.CustomEvent(eventName, new Dictionary<string, object>()
        {
            { "Posicao", transform.position },
            { "Tempo de jogo", Time.time - startGameTime },
            { "Area onde passou mais tempo", mostTimeAreaName },
            { "Revivido por jogadores", revivedByPlayers },
            { "Reviveu jogadores", revivedPlayers },
            { "Acerto em jogadores", hittedPlayers },
            { "Nocauteou jogadores", kodPlayers },
            { "Acerto em inimigos", hittedEnemies },
            { "Matou inimigos", killedEnemies }
        });
        print(eventName + " | " + result);

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
            RefreshToNewMosTimeArea();

            startMostTimeAreaTime = Time.time;
            currentAreaName = area.regionName;
        }

    }

    void OnApplicationQuit()
    {
        SendStatiscsAnalytics("Saiu");
    }

    void OnDestroy()
    {
        Destroy(GetComponent<NextPlayersAnalytics>());
    }
}
