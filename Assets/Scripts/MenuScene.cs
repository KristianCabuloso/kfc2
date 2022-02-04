using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkMenuScene))]
public class MenuScene : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public GameObject mainHolder;
    public GameObject waitHolder;

    public void Command_Send()
    {
        string _playerName = inputField.text;
        if (!string.IsNullOrEmpty(_playerName))
        {
            PlayerAnalytics.playerName = _playerName;

            if (_playerName == "server")
            {
                GetComponent<NetworkMenuScene>().Command_StartServer();
            }
            else
            {
                GetComponent<NetworkMenuScene>().Command_StartClient();
            }

            mainHolder.SetActive(false);
            waitHolder.SetActive(true);
        }
    }

    public void Command_Quit()
    {
        Application.Quit();
    }
}
