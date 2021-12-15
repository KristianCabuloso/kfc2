using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    PlayerInputAction input;

    public float mouseSensitivity = 1000f; // Para definir a sensibilidade do mouse

    public Transform playerBody; // Pega as informações de transform do "corpo" do jogador

    float xRotation = 0f; // variavel para guardar o valor de rotação

    void Start()
    {
        input = GetComponentInParent<PlayerInputHandler>().Input;

        // Tratamento de bug do novo Input System com mouse no Linux
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
        {
            // Bloquear o cursor no centro da tela se estiver em Linux
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Deixar o cursor confiando ¨¤ janela e invis¨ªvel se n?o estiver em Linux
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        MouseLook();


    }


    //Função responsavel por controlar a camera de acordo com a movimentação do mouse
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

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX * mouseSensitivity * Time.deltaTime);
    }
}
