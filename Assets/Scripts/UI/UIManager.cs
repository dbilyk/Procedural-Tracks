using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public GameManager GM;

    public GameObject StartRaceBtn;

    //Pause Menu elements
    public GameObject PauseMenu;
    public GameObject PauseMenuBtn;
    public GameObject QuitRaceBtn;
    public GameObject SettingsBtn;
    public GameObject RestartRaceBtn;
    public GameObject ClosePauseMenuBtn;
    public Text Currency;

    //use this to pause game
    private float targetTimescale=1f;

    public void StartRace()
    {
        StartRaceBtn.SetActive(false);
        PauseMenuBtn.SetActive(true);

        GM.ResetGame();
        GM.GenerateNewTrackData();
        GM.GenerateLevel();
        GM.GenerateAI();
        GM.StartingCountdown();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        GM.ResetGame();
        GM.StartingCountdown();
        PauseMenu.SetActive(false);
        PauseMenuBtn.SetActive(true);
    }

    public void OpenPauseMenu()
    {
        PauseMenu.SetActive(true);
        PauseMenuBtn.SetActive(false);
        targetTimescale = 0f;
        Time.timeScale = 0;

    }

    public void ClosePauseMenu()
    {
        PauseMenu.SetActive(false);
        PauseMenuBtn.SetActive(true);
        targetTimescale = 1f;
        Time.timeScale = 1;
    }


    public User user;
    void Awake()
    {
        user.OnCurrencyAdded += UpdateCurrency;
    }
    //stuff to update when currency QTY is updated
    void UpdateCurrency(int passedValue)
    {
        Currency.GetComponent<Text>().text = passedValue.ToString();
    }

}
