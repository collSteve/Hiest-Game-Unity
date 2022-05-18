using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;

    private bool gameIsOver;

    // Start is called before the first frame update
    void Start()
    {
        gameIsOver = false;
        Guard.OnGuardSpottedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().OnPlayerWin += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ShowGameLoseUI()
    {
        OnGameOver(gameLoseUI);
    }

    void ShowGameWinUI()
    {
        OnGameOver(gameWinUI);
    }

    void OnGameOver(GameObject displayUI)
    {
        displayUI.SetActive(true);
        gameIsOver = true;
        Guard.OnGuardSpottedPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player>().OnPlayerWin -= ShowGameWinUI;
    }
}
