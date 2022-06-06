using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public TextMeshProUGUI currentScore;
    public TextMeshProUGUI bestScore;

    private bool _newBestScore;
    private int _currentScore;
    private int _bestScore;

    private int _tempCurrentScore;
    private int _tempBestScore;

    /// <summary>
    /// Subscribing to Events - AddScore & GameOver
    /// </summary>
    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScore;
    }

    /// <summary>
    /// Unsubscribing to Events - AddScore & GameOver
    /// </summary>
    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScore;
    }

    /// <summary>
    /// 1. Loading BestScore and Storing the same in a temp variable (for animation)
    /// 2. Loading CurrentScore and Storing the same in a temp variable (for animation)
    /// 3. Changes status if currentScore surpasses newBestScore.
    /// 4. Update the UI with the retrieved info
    /// </summary>
    private void Start()
    {
        _bestScore = PlayerPrefs.HasKey("BestScore") ? PlayerPrefs.GetInt("BestScore") : 0;
        _tempBestScore = PlayerPrefs.HasKey("BestScore") ? PlayerPrefs.GetInt("BestScore") : 0;
     
        _currentScore = 0;
        _tempCurrentScore = 0;
        
        _newBestScore = false;
        UpdateScoreText();
    }

    /// <summary>
    /// 1. Increse Score - Event is called on Row/Col Deletion and successful Block Placement
    /// 2. Change the newBestScore status to true if currentScore surpasses bestScore
    /// 3. Save the bestScore as it has been updated
    /// 4. Update the UI with the retrieved info
    /// 5. Store the score as last frame for animation through UI
    /// </summary>
    /// <param name="score"></param>
    private void AddScores(int score)
    {
        _currentScore += score;

        if (_currentScore > _bestScore)
        {
            _newBestScore = true;
            _bestScore = _currentScore;
            SaveBestScore();
        }

        UpdateScoreText();
        _tempCurrentScore = _currentScore;
        _tempBestScore = _bestScore;
    }

    /// <summary>
    /// 1. Update the TextMeshPro Text through Counter Animation of DOTween
    /// 2. Call UpdateScores Event. Any function subscribing to event will be called
    /// </summary>
    private void UpdateScoreText()
    {
        currentScore.DOCounter(_tempCurrentScore, _currentScore, 0.5f);
        bestScore.DOCounter(_tempBestScore, _bestScore, 0.5f); 
        GameEvents.UpdateScores(_currentScore, _bestScore, _newBestScore);
    }

    /// <summary>
    /// Store the bestScore as a PlayerPref - BestScore
    /// </summary>
    private void SaveBestScore()
    {
        PlayerPrefs.SetInt("BestScore", _bestScore);
    }
}
