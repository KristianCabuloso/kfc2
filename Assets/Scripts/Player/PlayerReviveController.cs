using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public enum ReviveState : byte
{
    None,
    Dying,
    Reviving
}

[RequireComponent(typeof(Health))]
public class PlayerReviveController : EntityBehaviour<IKFCPlayerState>
{
    [Tooltip("Tempo que o jogador levará para morrer")]
    public float timeToDie = 10f;
    [Tooltip("Tempo que o jogador levará para ser revivido")]
    public float timeToRevive = 5f;
    [Tooltip("Distância mínima para conseguir reviver outro jogador")]
    public float reviveDistance = 1.5f;

    BattleManager battleManager;
    PlayerAnalytics playerAnalytics;
    PlayerReviveHUD reviveHud;
    Health health;

    public ReviveState State { private set; get; }
    public float DieCount { private set; get; }
    public float ReviveCount { private set; get; }
    Health otherPlayerHealth;

    public override void Attached()
    {
        battleManager = FindObjectOfType<BattleManager>();
        playerAnalytics = GetComponent<PlayerAnalytics>();
        health = GetComponent<Health>();

        if (entity.IsOwner && playerAnalytics)
        {
            reviveHud = FindObjectOfType<PlayerReviveHUD>();
            reviveHud.Setup(this);
        }
            
        state.OnPlayerRevive = Photon_Revive;
    }

    void Update()
    {
        switch (State)
        {
            case ReviveState.Dying:
                if (DieCount <= 0f)
                {
                    State = ReviveState.None;
                    state.PlayerIsDying = false;
                    reviveHud.Clear();
                    health.Die();
                }
                else
                {
                    DieCount -= Time.deltaTime;
                }
                break;

            case ReviveState.Reviving:
                if (ReviveCount <= 0f)
                {
                    if (playerAnalytics)
                        playerAnalytics.revivedPlayers++;
                    state.PlayerReviveEntity = otherPlayerHealth.entity;
                    state.PlayerRevive();
                    State = ReviveState.None;
                    reviveHud.Clear();
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
        state.PlayerIsDying = true;
    }

    public void ReceiveDamage()
    {
        DieCount -= Time.deltaTime * 2f;
    }

    public void Revive()
    {
        State = ReviveState.None;
        state.PlayerIsDying = false;
        if (playerAnalytics)
            playerAnalytics.revivedByPlayers++;
        reviveHud.Clear();
        health.Revive();
    }

    void Photon_Revive()
    {
        //Color color;

        BoltEntity _entity = state.PlayerReviveEntity;
        if (_entity.IsOwner)
            _entity.GetComponent<PlayerReviveController>().Revive();
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
                if (prc && prc.state.PlayerIsDying == true)
                {
                    otherPlayerHealth = h;
                    transform.LookAt(h.transform);
                    Vector3 _rot = transform.eulerAngles;
                    transform.eulerAngles = Vector3.up * _rot.y;
                    ReviveCount = timeToRevive;
                    State = ReviveState.Reviving;
                    break;
                }
            }
        }
    }

    public void Command_CancelRevive()
    {
        if (State == ReviveState.Reviving)
        {
            State = ReviveState.None;
            reviveHud.Clear();
        }
    }
}
