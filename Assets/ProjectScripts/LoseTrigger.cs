using UnityEngine;

public class LoseTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlayLose();
            LevelsManager.Instance.ShowLoseCanvas();
            TimeManager.Instance.PauseGame();
        }
    }
}
