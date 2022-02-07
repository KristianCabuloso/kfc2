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

[RequireComponent(typeof(Health), typeof(PlayerCharacterController))]
public class PlayerReviveController : EntityBehaviour<IKFCPlayerState>
{
    [Tooltip("Tempo que o jogador levará para morrer")]
    public float timeToDie = 10f;
    [Tooltip("Tempo que o jogador levará para ser revivido")]
    public float timeToRevive = 5f;
    [Tooltip("Distância mínima para conseguir reviver outro jogador")]
    public float reviveDistance = 1.5f;
    public Vector3 dieRotation;

    BattleManager battleManager;
    PlayerCharacterController playerCharacterController;
    PlayerAnalytics playerAnalytics;
    PlayerReviveHUD reviveHud;
    Health health;

    public ReviveState State { private set; get; }
    public float Count { private set; get; }
    Health otherPlayerHealth;

    public override void Attached()
    {
        battleManager = FindObjectOfType<BattleManager>();
        playerCharacterController = GetComponent<PlayerCharacterController>();
        playerAnalytics = GetComponent<PlayerAnalytics>();
        health = GetComponent<Health>();

        if (entity.IsOwner && playerAnalytics)
        {
            battleManager.owner = health;

            reviveHud = FindObjectOfType<PlayerReviveHUD>();
            reviveHud.Setup(this);
        }

        state.AddCallback("PlayerReviveEntity", OnReviveEntityChange);
    }

    void Update()
    {
        switch (State)
        {
            case ReviveState.Dying:
                if (Count <= 0f)
                {
                    State = ReviveState.None;
                    state.PlayerIsDying = false;
                    reviveHud.Clear();
                    health.Die();
                }
                else
                {
                    Count -= Time.deltaTime;
                }
                break;

            case ReviveState.Reviving:
                if (Count <= 0f)
                {
                    if (playerAnalytics)
                        playerAnalytics.revivedPlayers++;
                    state.PlayerReviveEntity = otherPlayerHealth.entity;
                    //state.PlayerRevive();
                    State = ReviveState.None;
                    reviveHud.Clear();
                }
                else
                {
                    Count -= Time.deltaTime;
                }
                break;
        }
    }

    public void TriggerTimeToDie()
    {
        Count = timeToDie;
        transform.localEulerAngles = dieRotation;
        State = ReviveState.Dying;
        state.PlayerIsDying = true;

        playerCharacterController.Command_CancelMove();
    }

    // Retorna TRUE se recebeu dano (ou seja, está revivendo ou morrendo)
    public bool TryReceiveDamage()
    {
        switch (State)
        {
            case ReviveState.Dying:
                Count -= Time.deltaTime * 2f; return true;

            case ReviveState.Reviving:
                Count += Time.deltaTime * 2f; return true;
        }

        return false;
    }

    public void Revive()
    {
        State = ReviveState.None;
        state.PlayerIsDying = false;
        if (playerAnalytics)
            playerAnalytics.revivedByPlayers++;
        reviveHud.Clear();
        transform.localEulerAngles = Vector3.zero;
        health.Revive();
    }

    void OnReviveEntityChange()
    {
        BoltEntity _entity = state.PlayerReviveEntity;

        if (_entity != null)
        {
            if (entity.IsOwner)
            {
                StartCoroutine(AnnulReviveEntityLater());
                return;
            }

            Health _owner = battleManager.owner;

            if (_entity == _owner.entity)
                _owner.ReviveController.Revive();
        }
    }

    /*public bool IsDying()
    {
        return State == ReviveState.Dying;
    }*/

    IEnumerator AnnulReviveEntityLater()
    {
        yield return new WaitForSecondsRealtime(2f);
        state.PlayerReviveEntity = null;
    }

    public void Command_Revive()
    {
        Vector3 _pos = transform.position;

        foreach (Health h in battleManager.players)
        {
            if (Vector3.Distance(_pos, h.transform.position) <= reviveDistance)
            {
                if (h.GetComponent<PlayerReviveController>() && h.state.PlayerIsDying)
                {
                    otherPlayerHealth = h;
                    transform.LookAt(h.transform);
                    Vector3 _rot = transform.eulerAngles;
                    transform.eulerAngles = Vector3.up * _rot.y;
                    Count = timeToRevive;
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
