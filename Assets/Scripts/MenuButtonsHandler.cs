using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtonsHandler : MonoBehaviour
{

    public GameObject loadingGameObject;
    public Slider loadingSlider;

    /// <summary>
    /// Load next scene and display loading screen with Loading Bar
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        
        StartCoroutine(LoadSceneASync(sceneName));
    }

    /// <summary>
    /// 1. Store the LoadSceneAsync operation for further processing
    /// 2. Activate the loadingGameObject to make it visible
    /// 3. Run the function till asyncOperation is done.
    /// 4. Update the slider value and on completion end the function.
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    IEnumerator LoadSceneASync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingGameObject.SetActive(true);

        while (!asyncOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            loadingSlider.value = progressValue;
            yield return null;
        }
    }
}
