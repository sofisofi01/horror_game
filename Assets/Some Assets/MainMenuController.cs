using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame()
    {
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    public void SettingsGame()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Single);
    }

    public void MenuGame()
    {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
    // Update is called once per frame
    public void ExitGame()
    {
        Application.Quit();
    }
}
