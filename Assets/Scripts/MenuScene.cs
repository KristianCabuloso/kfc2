using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScene : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public GameObject mainHolder;
    public GameObject gameOptionsHolder;
    public GameObject waitHolder;

    public void Command_Send()
    {
        string _playerName = inputField.text;
        if (!string.IsNullOrEmpty(_playerName))
        {
            PlayerAnalytics.playerName = _playerName;

            /*if (_playerName == "server")
            {
                GetComponent<NetworkMenuScene>().Command_StartServer();
            }
            else
            {
                GetComponent<NetworkMenuScene>().Command_StartClient();
            }*/

            mainHolder.SetActive(false);
            gameOptionsHolder.SetActive(true);
        }
    }

    public void Command_Quit()
    {
        Application.Quit();
    }

    public void Command_Back()
    {
        mainHolder.SetActive(true);
        gameOptionsHolder.SetActive(false);
    }

    public void Command_StartGame()
    {
        gameOptionsHolder.SetActive(false);
        waitHolder.SetActive(true);
    }
}
