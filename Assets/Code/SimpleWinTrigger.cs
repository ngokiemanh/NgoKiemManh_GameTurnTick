using UnityEngine;

public class SimpleWinTrigger : MonoBehaviour
{
    private bool gameEnded = false;

    public Canvas winCanvas;
    public AudioClip winSound;

    private GameModeManager gameModeManager;
    private ClockwiseGame clockwiseGame; // <-- thêm để tắt âm đồng hồ

    private void Start()
    {
        gameModeManager = FindObjectOfType<GameModeManager>();
        clockwiseGame = FindObjectOfType<ClockwiseGame>();

        if (gameModeManager == null)
        {
            Debug.LogWarning("Không tìm thấy GameModeManager trong scene.");
        }
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
