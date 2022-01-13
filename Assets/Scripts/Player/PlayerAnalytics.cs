using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(PlayerCharacterController), typeof(NextPlayersAnalytics))]
public class PlayerAnalytics : MonoBehaviour
{
    float startGameTime;// = Time.time;

    string mostTimeAreaName;
    string currentAreaName;
    float mostTimeAreaTime;
    float startMostTimeAreaTime;


    void Start()
    {
        startGameTime = Time.time;
    }

    public void SendDieAnalytics()
    {
        RefreshToNewMosTimeArea();

        SendAnalytics("Morreu", new Dictionary<string, object>()
        {
            { "Posicao", transform.position },
            { "Tempo de jogo", Time.time - startGameTime },
            { "Area onde jogador passa mais tempo", mostTimeAreaName }
        });

        GetComponent<NextPlayersAnalytics>().FinishAndSendAnalytics();
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

    void SendAnalytics(string eventName)
    {
        AnalyticsResult result = Analytics.CustomEvent(eventName);
        print(eventName + " | " + result);
    }

    void SendAnalytics(string eventName, IDictionary<string, object> arguments)
    {
        AnalyticsResult result = Analytics.CustomEvent(eventName, arguments);
        print(eventName + " | " + result);
    }

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

    void OnDestroy()
    {
        Destroy(GetComponent<NextPlayersAnalytics>());
    }
}
