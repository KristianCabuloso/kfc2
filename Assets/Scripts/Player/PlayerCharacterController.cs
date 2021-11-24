using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(BoltEntity))]
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
    PlayerInputAction input;

    Vector3 velocity; // Usado para calcular a acelera??o
    bool isGrounded; // boleana para verificar se esta no ch?o
    bool isRunning;



    void Awake()
    {
        input = new PlayerInputAction();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void Attached()
    {
        // Setar o transform do Bolt (online)
        state.SetTransforms(state.PlayerTransform, transform);


        // Desligar câmera se não for o jogador sendo controlado pelo cliente
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
        /*float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");*/
        Vector2 analog = input.Player.Move.ReadValue<Vector2>();

        Vector3 move = (transform.right * analog.x + transform.forward * analog.y).normalized;

        controller.Move(move * speed * BoltNetwork.FrameDeltaTime);

        controller.Move(velocity * BoltNetwork.FrameDeltaTime);

        /*if(Input.GetButtonDown("Jump") && isGrounded) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }*/
            
        //if (Input.GetButton("Run"))
        if (isRunning)
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

    public void Command_Jump(InputAction.CallbackContext obj)
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void OnEnable()
    {
        PlayerInputAction.PlayerActions playerActions = input.Player;

        playerActions.Jump.performed += Command_Jump;
        playerActions.Run.performed += ctx => isRunning = true;
        playerActions.Run.canceled += ctx => isRunning = false;
        //playerActions.Run.performed += Command_Run;

        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }
}