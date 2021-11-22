using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterController : EntityBehaviour<IKFCPlayerState>
{

    CharacterController controller; //cria uma variavel para podermos armazenar o CharacterController

    private float speed = 12f; // Define a velocidade atual do jogador
    public float walkingSpeed = 12f; // Define a velocidade do jogador andando
    public float runningSpeed = 24f; // Define a velocidade do jogador correndo
    public float gravity = -9.81f; //For?a da gravidade
    public float jumpHeight = 2f;


    public Transform groundCheck; //usado para armazenar a posi??o do groundCheck no jogo
    public float groundDistance = 0.4f; // defini o raio de detec??o do algum objeto
    public LayerMask groundMask; // Utilizado para definir se um objeto sera reconhecido como ch?o, para saber se o personagem pode pular/ desacelerar a queda

    Vector3 velocity; // Usado para calcular a acelera??o
    bool isGrounded; // boleana para verificar se esta no ch?o



    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void Attached()
    {
        state.SetTransforms(state.PlayerTransform, transform);

        if (!entity.IsOwner)
        {
            Camera _camera = GetComponentInChildren<Camera>();
            if (_camera)
                _camera.gameObject.SetActive(false);
        }
    }

    //private void FixedUpdate()
    public override void SimulateOwner()
    {
        Movement();
        Gravity();
        GroundCheck();
    }

    // Funcao que faz o jogador se movimentar
    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * x + transform.forward * z).normalized;

        controller.Move(move * speed * BoltNetwork.FrameDeltaTime);

        controller.Move(velocity * BoltNetwork.FrameDeltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetButton("Run"))
        {
            speed = runningSpeed;
        }else
        {
            speed = walkingSpeed;
        }
        

    }

    private void Gravity()
    {
        velocity.y += gravity * BoltNetwork.FrameDeltaTime;
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