using UnityEngine;

public class SimpleWinTrigger : MonoBehaviour
{
    private bool gameEnded = false;

    public Canvas winCanvas;
    public AudioClip winSound;

    private GameModeManager gameModeManager;

    private void Start()
    {
        // Tìm đối tượng chứa GameModeManager trong scene
        gameModeManager = FindObjectOfType<GameModeManager>();

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

            // Gọi trigger win từ GameModeManager để xử lý điểm + UI
            if (gameModeManager != null)
            {
                gameModeManager.TriggerWin();
            }
            else
            {
                // Nếu không dùng GameModeManager, fallback: chỉ bật canvas
                if (winCanvas != null)
                {
                    winCanvas.gameObject.SetActive(true);
                    Debug.Log("You win!");
                }
            }
        }
    }
}
