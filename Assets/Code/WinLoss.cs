using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoss : MonoBehaviour
{
    public GameObject winCanvas;
    public GameObject loseCanvas;

    public void OnHomeButton()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1.0f;
    }

    public void OnPlayButton()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            Time.timeScale = 1.0f;
        }
        else
        {
            Debug.LogWarning("Next scene not found in Build Settings.");
        }
    }

    public void OnReplayButton()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1.0f;
    }
}
