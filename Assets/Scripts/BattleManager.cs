using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public List<Health> players = new List<Health>();
    public List<Health> NPCs = new List<Health>();

    /*void Start()
    {
        PlayerCharacterController[] _players = FindObjectsOfType<PlayerCharacterController>();
        EnemyCharacterController[] _enemies = FindObjectsOfType<EnemyCharacterController>();

        foreach (PlayerCharacterController p in _players)
            players.Add(p.GetComponent<Health>());

        foreach (EnemyCharacterController e in _enemies)
            NPCs.Add(e.GetComponent<Health>());
    }*/

    /*public void AddPlayer(Health health)
    {
        players.Add(health);
    }

    public void RemovePlayer(Health health)
    {
        players.Remove(health);
    }

    public void AddNPC(Health health)
    {
        NPCs.Add(health);
    }

    public void RemoveNPC(Health health)
    {
        NPCs.Remove(health);
    }*/
}
