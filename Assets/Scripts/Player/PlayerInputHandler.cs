using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerInputHandler : MonoBehaviour
{
    PlayerCharacterController playerCharacterController;
    CameraLook playerCameraLook;
    public PlayerInputAction Input { private set; get; }

    // Inicializar antes do start, independente se tiver ativado ou não
    void Awake()
    {
        // Inicializar novo input system (precisa ser no awake)
        Input = new PlayerInputAction();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerCharacterController = GetComponent<PlayerCharacterController>();
        playerCameraLook = GetComponentInChildren<CameraLook>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Ao ativar o objeto/componente
    void OnEnable()
    {
        // Ativações/Atribuições necessárias para o Input System

        PlayerInputAction.PlayerActions playerActions = Input.Player;

        playerActions.Jump.performed += ctx => playerCharacterController.Command_Jump();
        playerActions.Run.performed += ctx => playerCharacterController.isRunning = true;
        playerActions.Run.canceled += ctx => playerCharacterController.isRunning = false;
        //playerActions.Run.performed += Command_Run;

        Input.Enable();
    }

    // Ao desativar o objeto/componente
    void OnDisable()
    {
        // Desativações/Desatribuições necessárias para o Input System
        Input.Disable();
    }
}
