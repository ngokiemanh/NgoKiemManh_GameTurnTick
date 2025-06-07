using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject canvasStart;
    public GameObject canvasMode;     // 👈 canvas chọn chế độ
    public GameObject canvasManChoi;
    public GameObject canvasHelp;

    [Header("Buttons in Canvas_Start")]
    public Button buttonPlay;
    public Button buttonHelp;

    [Header("Buttons in Canvas_Mode")]
    public Button buttonModeTime;
    public Button buttonModeNoTime;

    [Header("Buttons in Canvas_Help")]
    public Button buttonHome;

    [Header("Buttons in Canvas_ManChoi")]
    public Button[] manChoiButtons;

    private void Start()
    {
        ShowCanvas(canvasStart);

        // Gán sự kiện
        buttonPlay.onClick.AddListener(ShowMode);
        buttonHelp.onClick.AddListener(ShowHelp);
        buttonHome.onClick.AddListener(ShowStart);

        buttonModeTime.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("GameMode", 1); // 1: có thời gian
            PlayerPrefs.Save();
            ShowCanvas(canvasManChoi);
        });

        buttonModeNoTime.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("GameMode", 2); // 2: không thời gian
            PlayerPrefs.Save();
            ShowCanvas(canvasManChoi);
        });

        for (int i = 0; i < manChoiButtons.Length; i++)
        {
            int sceneIndex = i + 1;
            manChoiButtons[i].onClick.AddListener(() => LoadScene(sceneIndex));
        }
    }

    private void ShowCanvas(GameObject target)
    {
        canvasStart.SetActive(false);
        canvasMode.SetActive(false);
        canvasManChoi.SetActive(false);
        canvasHelp.SetActive(false);

        target.SetActive(true);
    }

    private void ShowStart() => ShowCanvas(canvasStart);
    private void ShowMode() => ShowCanvas(canvasMode);
    private void ShowHelp() => ShowCanvas(canvasHelp);

    private void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
