using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public GameObject gameOverPopup;
    public TextMeshProUGUI headerLabel;
    public TextMeshProUGUI currentScoreLabel;
    public TextMeshProUGUI highScoreLabel;

    private int _currentScore;
    private int _bestScore;
    private bool _newBestScore;

    /// <summary>
    /// Subscribing to Events - UpdateScore & GameOver
    /// </summary>
    private void OnEnable()
    {
        GameEvents.UpdateScores += OnUpdateScores;
        GameEvents.GameOver += OnGameOver;
    }

    /// <summary>
    /// Unsubscribing to Events - UpdateScore & GameOver
    /// </summary>
    private void OnDisable()
    {
        GameEvents.UpdateScores -= OnUpdateScores;
        GameEvents.GameOver -= OnGameOver;
    }

    /// <summary>
    /// Force the gameover popup to be inactive at game start
    /// </summary>
    private void Start()
    {
        gameOverPopup.SetActive(false);
    }

    /// <summary>
    /// On Game Over - Display the Gameover panel
    /// Display Best Score in case of new best score
    /// </summary>
    private void OnGameOver()
    {
        headerLabel.text = _newBestScore ? "Best Score!" : "Game Over!";
        gameOverPopup.SetActive(true);
        currentScoreLabel.DOCounter(0, _currentScore, 0.6f);
        highScoreLabel.DOCounter(0, _bestScore, 0.9f);
    }

    /// <summary>
    /// On Update Scores - Get the scores to update on the Gameover Panel UI.
    /// </summary>
    /// <param name="currentScore"></param>
    /// <param name="bestScore"></param>
    /// <param name="newBestScore"></param>
    private void OnUpdateScores(int currentScore, int bestScore, bool newBestScore)
    {
        _currentScore = currentScore;
        _bestScore = bestScore;
        _newBestScore = newBestScore;
    }
}
