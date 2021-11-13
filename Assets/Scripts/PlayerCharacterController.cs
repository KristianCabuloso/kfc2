using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacterController : MonoBehaviour
{
    private CharacterController controller;
    //Transform cameraTransform;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float runSpeed = 4.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float turnSpeed = 2f;
    [SerializeField] float minTurnAngle = -90f;
    [SerializeField] float maxTurnAngle = 90f;
    private float gravityValue = -9.81f;
    float rotX;

    private void Start()
    {
        //cameraTransform = GetComponentInChildren<Camera>().transform;
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // get the mouse inputs
        float rotY = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;

        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + rotY, 0);

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        float speed;
        if (Input.GetButton("Run"))
        {
            speed = runSpeed;
        }
        else
        {
            speed = moveSpeed;
        }

        //Vector3 move = (cameraTransform.right * Input.GetAxis("Horizontal")) + (cameraTransform.forward * Input.GetAxis("Vertical"));
        Vector3 move = (transform.right * Input.GetAxis("Horizontal")) + (transform.forward * Input.GetAxis("Vertical"));
        move.y = 0f;
        controller.Move(move * Time.deltaTime * speed);

        /*if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }*/

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}