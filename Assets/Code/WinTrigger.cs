using UnityEngine;
using System.Collections.Generic;

public class WinTrigger : MonoBehaviour
{
    private bool gameEnded = false;

    public Canvas winCanvas;

    public List<GameObject> bellOffList = new List<GameObject>();
    public List<GameObject> bellOnList = new List<GameObject>();

    public AudioClip winSound;
    public AudioClip bellOnSound;
    public AudioClip bellOffSound;

    public float bellCooldown = 1f;

    private float lastGlobalBellInteractionTime = -10f;

    private GameModeManager gameModeManager;
    private ClockwiseGame clockwiseGame; // <-- thêm để tìm và tắt âm kim

    void Start()
    {
        // Gán trạng thái bell ban đầu
        for (int i = 0; i < bellOffList.Count; i++)
        {
            if (bellOffList[i] != null) bellOffList[i].SetActive(true);
            if (i < bellOnList.Count && bellOnList[i] != null) bellOnList[i].SetActive(false);
        }

        gameModeManager = FindObjectOfType<GameModeManager>();
        if (gameModeManager == null)
        {
            Debug.LogWarning("Không tìm thấy GameModeManager trong scene.");
        }

        clockwiseGame = FindObjectOfType<ClockwiseGame>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameEnded) return;

        if (other.CompareTag("Goal"))
        {
            gameEnded = true;

            if (winSound != null)
                AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);

            if (clockwiseGame != null)
            {
                var audio = clockwiseGame.GetComponent<AudioSource>();
                if (audio != null) audio.Stop();
            }

            Time.timeScale = 0;

            if (winCanvas != null)
                winCanvas.enabled = true;

            if (gameModeManager != null)
                gameModeManager.TriggerWin();
        }
    }
}
