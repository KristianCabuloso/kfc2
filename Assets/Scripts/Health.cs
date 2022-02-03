using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Health : EntityBehaviour<IKFCPlayerState>
{
    [SerializeField] int maxHealth = 10;

    [Header("Regeneração")]
    [SerializeField] int regenerationAmount = 2;
    [SerializeField] float regenerationWaitTime = 0.5f;
    [SerializeField] float regenerationStartWaitTime = 5f;

    public PlayerReviveController ReviveController { private set; get; }
    public int MaxHealth { get => maxHealth; }

    PlayerAnalytics playerAnalytics;

    float regenerationStartCount;
    float regenerationCount;

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            state.PlayerHealth = maxHealth;

            ReviveController = GetComponent<PlayerReviveController>();
            playerAnalytics = GetComponent<PlayerAnalytics>();

            if (ReviveController) // Prevenir o bug de atribuir vida dos inimigos à HUD do servidor
                FindObjectOfType<HealthHUD>().Setup(this);
        }
    }

    public override void SimulateOwner()
    {
        if (regenerationStartCount <= 0f)
        {
            int healthAmount = state.PlayerHealth;

            if (healthAmount < maxHealth)
            {
                if (regenerationCount <= 0f)
                {
                    state.PlayerHealth = Mathf.Min(healthAmount + regenerationAmount, maxHealth);
                    regenerationCount = regenerationWaitTime;
                }
                else
                {
                    regenerationCount -= Time.deltaTime;
                }
            }
        }
        else
        {
            regenerationStartCount -= Time.deltaTime;
        }
    }

    // Retorna TRUE se após receber dano teve vida == 0
    public bool ReceiveDamage(int damage, bool damageFromPlayer)
    {
        // Não rodar código se não for o dono (o Photon nem deixa)
        if (!entity.IsOwner)
            return false;

        // Atrapalhar o jogador de reviver se ele estiver necessitando reviver
        if (ReviveController && ReviveController.TryReceiveDamage())
            return false;

        // Processo normal de perda de vida

        int health = state.PlayerHealth;

        health -= damage; // Perder vida
        health = Mathf.Max(health, 0); // Impedir que a vida fique menor que 0

        // Atualizar a vida online com a vida modificada
        state.PlayerHealth = health;

        // Se vida chegou a 0
        if (health == 0)
        {
            if (playerAnalytics)
            {
                if (damageFromPlayer)
                {
                    playerAnalytics.kodByPlayers++;
                }
                else
                {
                    playerAnalytics.kodByEnemies++;
                }
            }

            if (ReviveController)
            {
                ReviveController.TriggerTimeToDie();
                regenerationStartCount = Mathf.Infinity;
            }
            else
            {
                Die();
            }

            return true;
        }

        regenerationStartCount = regenerationStartWaitTime;
       
        return false;
    }

    public void Revive()
    {
        state.PlayerHealth = Mathf.RoundToInt(maxHealth * 0.3f);
        regenerationStartCount = regenerationStartWaitTime / 2f;
    }

    public void Die()
    {
        // Enviar analytics de morte se for jogador
        PlayerAnalytics playerAnalytics = GetComponent<PlayerAnalytics>();
        if (playerAnalytics)
            playerAnalytics.SendDieAnalytics();

        // Destruir objeto pelo Bolt se a vida foi zerada
        BoltNetwork.Destroy(gameObject);
    }
}
