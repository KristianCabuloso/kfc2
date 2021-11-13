using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterController : MonoBehaviour
{

    public CharacterController controller; //cria uma variavel para podermos armazenar o CharacterController

    public float speed = 12f; // Define a velocidade do jogador
    public float gravity = -9.81f; //Força da gravidade
    public float jumpHeight = 3f;


    public Transform groundCheck; //usado para armazenar a posição do groundCheck no jogo
    public float groundDistance = 0.4f;
    public LayerMask groundMask; // Utilizado para definir se um objeto sera reconhecido como chão, para saber se o personagem pode pular/ desacelerar a queda

    Vector3 velocity; // Usado para calcular a aceleração
    bool isGrounded; // boleana para verificar se esta no chão



    private void Start()
    {
        
    }

    private void Update()
    {
        Movement();
        Gravity();
        GroundCheck();

    }
    // Função que faz o jogador se movimentar
    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

    }

}