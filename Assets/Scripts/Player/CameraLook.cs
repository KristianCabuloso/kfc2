using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{

    public float mouseSensitivity = 1000f; // Para definir a sensibilidade do mouse

    public Transform playerBody; // Pega as informa��es de transform do "corpo" do jogador

    float xRotation = 0f; // variavel para guardar o valor de rota��o

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MouseLook();


    }


    //Fun��o responsavel por controlar a camera de acordo com a movimenta��o do mouse
    private void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
