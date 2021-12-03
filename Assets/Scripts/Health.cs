using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

public class Health : EntityBehaviour<IKFCPlayerState>
{
    [SerializeField] int maxHealth = 10;

    public override void Attached()
    {
        state.PlayerHealth = maxHealth;
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
        }

        // Atualizar a vida online com a vida modificada
        state.PlayerHealth = health;

        // Destruir objeto pelo Bolt se a vida foi zerada
        if (zeroHealthFlag)
            BoltNetwork.Destroy(gameObject);
    }
}
