using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject featsMenu;
    [SerializeField] private GameObject creditsMenu;

    private List<GameObject> menuList;

    private GameObject currentMenu;

    private void Start()
    {
        menuList = new List<GameObject>() { mainMenu, settingsMenu, featsMenu, creditsMenu };
    }

    public void StartGame()
    {
        // Load the game scene (replace "GameScene" with your game scene name)
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Quit the game (this will only work in a built version of the game)
        Application.Quit();
    }

    public void Settings()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
        currentMenu = creditsMenu;
    }

    public void Feats()
    {
        mainMenu.SetActive(false);
        featsMenu.SetActive(true);
        currentMenu = featsMenu;
    }

    public void Credits()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        currentMenu = settingsMenu;
    }

    public void BackToMainMenu()
    {
        currentMenu.SetActive(false);
        mainMenu.SetActive(true);
        currentMenu = mainMenu;
    }

}
