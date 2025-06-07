using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public Button buttonHome;
    public Button buttonReload;

    private void Start()
    {
        buttonHome.onClick.AddListener(LoadHomeScene);
        buttonReload.onClick.AddListener(ReloadCurrentScene);
    }

    private void LoadHomeScene()
    {
        SceneManager.LoadScene("GameManager"); // Tên scene "Home" phải có trong Build Settings
    }

    private void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
