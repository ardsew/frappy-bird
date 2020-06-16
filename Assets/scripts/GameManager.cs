using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    // two below need static
    public static event GameDelegate OnGameStarted, OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState {
        None,
        Start,
        GameOver,
        Countdown
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver {
        get {
            return gameOver;
        }
    }

    void Awake() {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable() {
        CountdownText.OnCountdownFinished += OnCountdownFinished;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
    }

    void onDisable() {
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
    }

    void OnCountdownFinished() {
        SetPageState(PageState.None);
        OnGameStarted(); // event sent to TapController
        score = 0;
        gameOver = false;
    }

    void OnPlayerDied() {
        gameOver = true;
        int saveScore = PlayerPrefs.GetInt("HighScore");
        if(score > saveScore) {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SetPageState(PageState.GameOver);
    }

    void OnPlayerScored() {
        score++;
        scoreText.text = score.ToString();
    }

    void SetPageState(PageState state) {
        switch(state) {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
        }

    }

    public void ConfirmGameOver() {
        // works when replay button is hit
        SetPageState(PageState.Start);
        scoreText.text = "0";
        OnGameOverConfirmed(); // event sent to TapController
    }

    public void StartGame() {
        // works when play button is hit
        SetPageState(PageState.Countdown);
    }
}
