using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Health owner;
    public List<Health> players = new List<Health>();
    public List<Health> NPCs = new List<Health>();


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
