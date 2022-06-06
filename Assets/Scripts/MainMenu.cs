using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject gameTitle;
    public GameObject tapToPlay;
    public GameObject gameExitMenu;
    public GameObject gameSettingsMenu;
    public TextMeshProUGUI bestScoreReference;
    [Space()]
    [Header("Game Title Animations")]
    public float titleTargetScale = 0.95f;
    public float titleDuration = 1.0f;
    public Ease titleEaseType = Ease.OutQuad;
    [Space()]
    [Header("Tap to Play Animations")]
    public float ttpDuration = 0.75f;
    public float ttpShakeRotAmount = 2.0f;
    [Space()]
    [Header("Panel Menu Animations")]
    public float panelMenuDuration = 0.5f;
    public Ease panelMenuEaseType = Ease.OutQuad;

    /// <summary>
    /// 1. Set Repeating Animation of Scaling Up and Down on The GameTitle GameObject
    /// 2. Includes - GameTitle/AuthorTitle/BestScore Text
    /// 3. Set Repeating Animation of Slight shake on the Tap to Play Text
    /// 4. Update the Best Score and Update the Text in Field
    /// </summary>
    private void Start()
    {
        gameTitle.transform.DOScale(titleTargetScale, titleDuration).SetLoops(-1, LoopType.Yoyo).SetEase(titleEaseType);

        Tween ttp_Shake = tapToPlay.transform.DOShakeRotation(ttpDuration, new Vector3(0f, 0f, ttpShakeRotAmount));
        Sequence seq = DOTween.Sequence().SetLoops(-1, LoopType.Yoyo);
        seq.PrependInterval(2.0f);
        seq.Append(ttp_Shake);

        RetrieveBestScore();
    }

    /// <summary>
    /// If Best Score Key is not present show score as 0 else display stored score.
    /// </summary>
    private void RetrieveBestScore()
    {
        bestScoreReference.text = string.Format("Best Score: {0}", PlayerPrefs.HasKey("BestScore") ? PlayerPrefs.GetInt("BestScore") : "0");
    }

    /// <summary>
    /// Exit the Application
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 1. Set PlayerPrefs score to 0
    /// 2. Update the Best Score Text Field
    /// 3. Close the Best Score Panel on Success.
    /// </summary>
    public void ResetScore()
    {
        if(PlayerPrefs.HasKey("BestScore"))
            PlayerPrefs.SetInt("BestScore", 0);
     
        RetrieveBestScore();
        
        PanelSequence(gameSettingsMenu.transform, false).Play();
    }

    /// <summary>
    /// Animation of Scaling Up and Down the given Panel.
    /// Can Scale Up or Scale down depending on rewind value
    /// </summary>
    /// <param name="panelMenuTransform"></param>
    /// <param name="rewind"></param>
    /// <returns></returns>
    private Sequence PanelSequence(Transform panelMenuTransform, bool rewind)
    {
        Sequence seq = DOTween.Sequence();
        if (rewind)
        {
            seq.Append(panelMenuTransform.DOScale(1.0f, 0.5f).SetEase(panelMenuEaseType));
        }
        else
        {
            seq.Append(panelMenuTransform.DOScale(0.0f, 0.5f).SetEase(panelMenuEaseType));
        }
        return seq;
    }

    /// <summary>
    /// Player PanelSequence Forward for ExitMenu
    /// </summary>
    public void DisplayQuitMenu()
    {
        PanelSequence(gameExitMenu.transform, true).Play();
    }

    /// <summary>
    /// Player PanelSequence Forward for GameSettingsMenu
    /// </summary>
    public void DisplaySettingsMenu()
    {
        PanelSequence(gameSettingsMenu.transform, true).Play();
    }

    /// <summary>
    /// Player PanelSequence Backwards for ExitMenu
    /// </summary>
    public void HideQuitMenu()
    {
        PanelSequence(gameExitMenu.transform, false).Play();
    }

    /// <summary>
    /// Player PanelSequence Backwards for GameSettingsMenu
    /// </summary>
    public void HideSettingsMenu()
    {
        PanelSequence(gameSettingsMenu.transform, false).Play();
    }
}
