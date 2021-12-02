using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO Fazer essa solução funcionar online
public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    [Tooltip("health já inicia (Start()) com maxHealth"), SerializeField]
    int health;

    void Start()
    {
        health = maxHealth;
    }

    public void ReceiveDamage(int damage)
    {
        // Perder vida
        health -= damage;

        // Se vida chegou a 0
        if (health <= 0)
        {
            health = Mathf.Max(health, 0);

            // Chamar game over se o jogador for do atual cliente
            PlayerCharacterController _player = GetComponent<PlayerCharacterController>();
            if (_player && _player.entity.IsOwner)
            {
                // TODO chamar game over
            }

            // TODO adaptar para online
            Destroy(gameObject);
        }
    }
}
