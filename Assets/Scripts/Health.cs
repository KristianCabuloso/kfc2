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
    public int MaxHealth { get => maxHealth; }

    [SerializeField] float regenerationStartCount;
    [SerializeField] float regenerationCount;

    public override void Attached()
    {
        state.PlayerHealth = maxHealth;

        if (entity.IsOwner && GetComponent<PlayerCharacterController>()) // Prevenir o bug de atribuir vida dos inimigos à HUD do servidor
            FindObjectOfType<HealthHUD>().Setup(this);
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

    public void ReceiveDamage(int damage)
    {
        int health = state.PlayerHealth;

        health -= damage; // Perder vida
        bool zeroHealthFlag = health <= 0; // Ativar flag de vida zerada

        // Se vida chegou a 0
        if (zeroHealthFlag)
        {
            health = Mathf.Max(health, 0);

            // Chamar game over se o jogador for do atual cliente
            if (entity.IsOwner)
            {
                // TODO Alguma condição especial caso o jogador do cliente morra aqui
            }

            regenerationStartCount = Mathf.Infinity;
        }
        else
        {
            regenerationStartCount = regenerationStartWaitTime;
        }

        // Atualizar a vida online com a vida modificada
        state.PlayerHealth = health;

        // Destruir objeto pelo Bolt se a vida foi zerada
        if (zeroHealthFlag)
            BoltNetwork.Destroy(gameObject);
    }
}
