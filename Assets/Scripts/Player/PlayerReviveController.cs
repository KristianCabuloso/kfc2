using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ReviveState : byte
{
    None,
    Dying,
    Reviving
}

[RequireComponent(typeof(Health))]
public class PlayerReviveController : MonoBehaviour
{
    [Tooltip("Tempo que o jogador levará para morrer")]
    public float timeToDie = 10f;
    [Tooltip("Tempo que o jogador levará para ser revivido")]
    public float timeToRevive = 5f;
    [Tooltip("Distância mínima para conseguir reviver outro jogador")]
    public float reviveDistance = 1.5f;

    BattleManager battleManager;
    Health health;

    public ReviveState State { private set; get; }
    public float DieCount { private set; get; }
    public float ReviveCount { private set; get; }
    Health otherPlayerHealth;

    void Start()
    {
        battleManager = FindObjectOfType<BattleManager>();
        health = GetComponent<Health>();

        PlayerCharacterController pcc = GetComponent<PlayerCharacterController>();
        if (pcc && pcc.entity.IsOwner)
            FindObjectOfType<PlayerReviveHUD>().Setup(this);
    }

    void Update()
    {
        switch (State)
        {
            case ReviveState.Dying:
                if (DieCount <= 0f)
                {
                    health.Die();
                    State = ReviveState.None;
                }
                else
                {
                    DieCount -= Time.deltaTime;
                }
                break;

            case ReviveState.Reviving:
                if (ReviveCount <= 0f)
                {
                    otherPlayerHealth.Revive();
                    State = ReviveState.None;
                }
                else
                {
                    ReviveCount -= Time.deltaTime;
                }
                break;
        }
    }

    public void TriggerTimeToDie()
    {
        DieCount = timeToDie;
        State = ReviveState.Dying;
    }

    public void ReceiveDamage()
    {
        DieCount -= Time.deltaTime * 2f;
    }

    /*public bool IsDying()
    {
        return State == ReviveState.Dying;
    }*/

    public void Command_Revive()
    {
        Vector3 _pos = transform.position;

        foreach (Health h in battleManager.players)
        {
            if (Vector3.Distance(_pos, h.transform.position) <= reviveDistance)
            {
                PlayerReviveController prc = h.GetComponent<PlayerReviveController>();
                if (prc && prc.State == ReviveState.Dying)
                {
                    otherPlayerHealth = h;
                    transform.LookAt(h.transform);
                    Vector3 _rot = transform.eulerAngles;
                    transform.eulerAngles = Vector3.up * _rot.y;
                    State = ReviveState.Reviving;
                    break;
                }
            }
        }
    }
}
