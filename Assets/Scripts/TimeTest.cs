using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTest : MonoBehaviour
{
    [SerializeField] float storedTime;
    [SerializeField] float totalTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            storedTime = Time.time;
            print("Salvo: " + storedTime);
        }
        /*else if (Input.GetKeyDown(KeyCode.B))
        {
            print("Tempo total: " + (Time.time - storedTime));
        }*/

        totalTime = Time.time - storedTime;
    }
}
