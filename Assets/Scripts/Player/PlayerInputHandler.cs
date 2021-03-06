using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController), typeof(WeaponController), typeof(PlayerReviveController))]
[RequireComponent(typeof(PlayerReviveController))]
public class PlayerInputHandler : MonoBehaviour
{
    PlayerCharacterController playerCharacterController;
    WeaponController weaponController;
    PlayerReviveController playerReviveController;
    //CameraLook playerCameraLook;
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
        weaponController = GetComponent<WeaponController>();
        playerReviveController = GetComponent<PlayerReviveController>();
        //playerCameraLook = GetComponentInChildren<CameraLook>();
    }

    /*void Command_Fire()
    {
        weaponController.TryShoot(playerCameraLook.transform.forward);
    }*/

    // Ao ativar o objeto/componente
    void OnEnable()
    {
        // Ativações/Atribuições necessárias para o Input System

        PlayerInputAction.PlayerActions playerActions = Input.Player;

        playerActions.Jump.performed += ctx => playerCharacterController.Command_Jump();

        playerActions.Move.performed += ctx => {
            if (playerReviveController.State != ReviveState.Dying)
                playerCharacterController.Command_Move(ctx.ReadValue<Vector2>());
            playerReviveController.Command_CancelRevive();
        };
        playerActions.Move.canceled += ctx => playerCharacterController.Command_CancelMove();

        playerActions.Run.performed += ctx => playerCharacterController.isRunning = true;
        playerActions.Run.canceled += ctx => playerCharacterController.isRunning = false;

        playerActions.Fire.performed += ctx => playerCharacterController.Command_Fire();

        playerActions.SwitchWeapon.performed += ctx => weaponController.SwitchWeapon((int)ctx.ReadValue<float>() - 1);

        playerActions.Revive.performed += ctx => playerReviveController.Command_Revive();
        playerActions.Revive.canceled += ctx => playerReviveController.Command_CancelRevive();

        Input.Enable();
    }

    // Ao desativar o objeto/componente
    void OnDisable()
    {
        // Desativações/Desatribuições necessárias para o Input System
        Input.Disable();
    }
}
