using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        Debug.Log("start");
    }
    public void EnterSettings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        
    }
    public void LeaveSettings()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
