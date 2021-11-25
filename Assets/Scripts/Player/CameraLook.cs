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
        //Cursor.lockState = CursorLockMode.Confined;
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
        float mouseX = input.Player.CameraX.ReadValue<float>() * mouseSensitivity* Time.deltaTime;
        float mouseY = input.Player.CameraY.ReadValue<float>() * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
