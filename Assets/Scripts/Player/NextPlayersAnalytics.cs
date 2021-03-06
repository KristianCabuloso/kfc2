using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class NextPlayersAnalytics : MonoBehaviour
{
    [SerializeField, Tooltip("Distânca mínima para considerar que o jogador está próximo de algum(ns) outro(s) jogador(es)")]
    float nextToPlayersMinimumDistance = 5f;
    [SerializeField, Tooltip("Tempo de intervalo para considerar que o jogador trocou de grupo, caso haja mudança nos membros do grupo")]
    float changeGroupTimeTolerance = 10f;

    BattleManager battleManager;

    List<NextPlayersAnalyticsGroup> nextPlayersAnalyticsGroups = new List<NextPlayersAnalyticsGroup>{ new NextPlayersAnalyticsGroup() };
    float currentGroupStartTime;

    float distantFromPlayersTotalTime;
    //bool emptyGroupFlag = true;

    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
    }

    void Update()
    {
        NextPlayersAnalyticsGroup currentGroup = nextPlayersAnalyticsGroups[nextPlayersAnalyticsGroups.Count - 1];

        Vector3 _pos = transform.position;
        List<Health> closerPlayers = currentGroup.players;

        float currentGroupTimeCount = Time.time - currentGroupStartTime;

        // Verificar se é necessário remover algum jogador do grupo se o tempo de troca de grupo passou
        if (currentGroupTimeCount >= changeGroupTimeTolerance)
        {
            foreach (Health otherPlayer in closerPlayers)
            {
                if (otherPlayer == null || Vector3.Distance(_pos, otherPlayer.transform.position) > nextToPlayersMinimumDistance)
                {
                    // Iniciar um novo grupo
                    NextPlayersAnalyticsGroup newGroup = new NextPlayersAnalyticsGroup();
                    nextPlayersAnalyticsGroups.Add(newGroup);
                    currentGroup.totalTime = currentGroupTimeCount;
                    currentGroup = newGroup;
                    currentGroupStartTime = Time.time;
                    closerPlayers = newGroup.players;
                    break;
                }
            }
        }

        // Verificar se há algum jogador próximo
        foreach (Health otherPlayer in battleManager.players)
        {
            if (!closerPlayers.Contains(otherPlayer) && Vector3.Distance(_pos, otherPlayer.transform.position) <= nextToPlayersMinimumDistance)
                closerPlayers.Add(otherPlayer);
        }

        // Se não houver jogadores próximos (após operações que add/rem jogadores à lista)
        if (closerPlayers.Count == 0)
            distantFromPlayersTotalTime += Time.deltaTime;
    }

    public void FinishAndSendAnalytics()
    {
        float nextToPlayersTotalTime = 0f;
        int maxNextPlayersCount = 0;
        int totalPlayers = 0;

        foreach (NextPlayersAnalyticsGroup group in nextPlayersAnalyticsGroups)
        {
            nextToPlayersTotalTime += group.totalTime;

            int _count = group.players.Count;
            totalPlayers += _count;

            if (_count > maxNextPlayersCount)
                maxNextPlayersCount = _count;
        }

        int groupsCount = nextPlayersAnalyticsGroups.Count;

        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            {"Nome do jogador", PlayerAnalytics.playerName},
            {"Tempo total proximo de jogadores", nextToPlayersTotalTime},
            {"Media tempo proximo de jogadores", nextToPlayersTotalTime / groupsCount},
            {"Numero maximo de jogadores proximos", maxNextPlayersCount},
            {"Media numero de jogadores proximos", totalPlayers / groupsCount},
            {"Tempo total distante de jogadores", distantFromPlayersTotalTime},
        };
        AnalyticsResult result = Analytics.CustomEvent("Proximodade de jogadores", dict);
        print("Proximodade de jogadores | Resultado: " + result);
        string printMessage = "";
        foreach (KeyValuePair<string, object> k in dict)
        {
            printMessage += '\n' + k.Key + " : " + k.Value;
        }
        print(printMessage);

        enabled = false;
    }
}