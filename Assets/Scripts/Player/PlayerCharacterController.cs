using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(BoltEntity))]
public class PlayerCharacterController : EntityBehaviour<IKFCPlayerState>
{
    BattleManager battleManager;
    CharacterController controller; //cria uma variavel para podermos armazenar o CharacterController
    WeaponController weaponController;
    PlayerInputAction input;

    private float speed = 12f; // Define a velocidade atual do jogador
    public float walkingSpeed = 12f; // Define a velocidade do jogador andando
    public float runningSpeed = 24f; // Define a velocidade do jogador correndo
    public float gravity = -9.81f; //For?a da gravidade
    public float jumpHeight = 2f;

    [SerializeField, Tooltip("Cabeça do jogador que se moverá com a câmera")]
    Transform playerHead;
    public float mouseSensitivity = 1000f; // Para definir a sensibilidade do mouse

    public Transform groundCheck; //usado para armazenar a posi??o do groundCheck no jogo
    public float groundDistance = 0.4f; // defini o raio de detec??o do algum objeto
    public LayerMask groundMask; // Utilizado para definir se um objeto sera reconhecido como ch?o, para saber se o personagem pode pular/ desacelerar a queda

    Vector3 velocity; // Usado para calcular a acelera??o
    bool isGrounded; // boleana para verificar se esta no ch?o
    float xRotation;
    Vector2 moveAxis;
    [HideInInspector] public bool isRunning;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        //weaponController = GetComponent<WeaponController>();
        //input = GetComponent<PlayerInputHandler>().Input;

        // Tratamento de bug do novo Input System com mouse no Linux
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
        {
            // Bloquear o cursor no centro da tela se estiver em Linux
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Deixar o cursor confiando à janela e invisível se n?o estiver em Linux
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }

    // Inicializar assim que o Bolt atribuir o objeto (Start do Bolt)
    public override void Attached()
    {
        // Setar o transform do Bolt (online)
        state.SetTransforms(state.PlayerTransform, transform);
        if (playerHead)
            state.SetTransforms(state.PlayerHeadTransform, playerHead);

        battleManager = FindObjectOfType<BattleManager>();
        weaponController = GetComponent<WeaponController>();

        if (entity.IsOwner)
        {
            //WeaponController weaponController = GetComponent<WeaponController>();
            if (weaponController)
                FindObjectOfType<WeaponHUD>().Setup(weaponController, playerHead);

            PlayerInputHandler inputHandler = GetComponent<PlayerInputHandler>();
            if (inputHandler)
            {
                Debug.LogWarning("PlayerInputHandler detectado em " + name +
                ". Considerando a forma como o script opera, não é recomendado que PlayerInputHandler seja inicializado pela interface da Unity, e sim adicionado por outro script.");
            }
            else
            {
                inputHandler = gameObject.AddComponent<PlayerInputHandler>();
            }
            input = inputHandler.Input;
        }
        else
        {
            // Desligar câmera se não for o jogador sendo controlado pelo cliente
            Camera _camera = GetComponentInChildren<Camera>();
            if (_camera)
                _camera.gameObject.SetActive(false);

            // Destruir script que envia dados de telemetria, pois apenas o próprio cliente pode enviá-los
            PlayerAnalytics playerAnalytics = GetComponent<PlayerAnalytics>();
            if (playerAnalytics)
                Destroy(playerAnalytics);

            /*if (inputHandler)
                Destroy(inputHandler);*/
        }

        Health health = GetComponent<Health>();
        if (health)
            battleManager.players.Add(health);
    }

    // Atualizar (Bolt) apenas para o jogador sendo controlado pelo cliente
    public override void SimulateOwner()
    {
        Movement();
        Gravity();
        GroundCheck();
        MouseLook();
    }

    // Funcao que faz o jogador se movimentar
    private void Movement()
    {
        //Vector2 analog = input.Player.Move.ReadValue<Vector2>();

        Vector3 move = (transform.right * moveAxis.x + transform.forward * moveAxis.y).normalized;

        controller.Move(move * speed * BoltNetwork.FrameDeltaTime);

        controller.Move(velocity * BoltNetwork.FrameDeltaTime);

        if (isRunning)
        {
            speed = runningSpeed;
        }
        else
        {
            speed = walkingSpeed;
        }
    }

    private void Gravity()
    {
        velocity.y += gravity * BoltNetwork.FrameDeltaTime;
    }

    //Fun玢o responsavel por controlar a camera de acordo com a movimenta玢o do mouse
    private void MouseLook()
    {
        //float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        //float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseX, mouseY;

        // Corre??o de bug no Linux
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
        {
            // Usa o antigo input system para usar o mouse se estiver no Linux
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }
        else
        {
            // Usa o novo input system para o mouse se n?o estiver no Linux
            mouseX = input.Player.CameraX.ReadValue<float>();
            mouseY = input.Player.CameraY.ReadValue<float>();
        }

        xRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerHead.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

    }

    public void Command_Move(Vector2 moveAxis)
    {
        this.moveAxis = moveAxis;
    }

    public void Command_CancelMove()
    {
        this.moveAxis = Vector2.zero;
    }

    // Callback de quando pressiona o botão de pulo
    public void Command_Jump()
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void Command_Fire()
    {
        if (weaponController)
            weaponController.TryShoot(playerHead.forward);
    }

    public override void Detached()
    {
        Health health = GetComponent<Health>();
        if (health)
            battleManager.players.Remove(health);
    }
}
