using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public CharacterController player;


    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType(typeof(CharacterController)) as CharacterController;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        transform.LookAt(player.transform.position);
        
    }
}
