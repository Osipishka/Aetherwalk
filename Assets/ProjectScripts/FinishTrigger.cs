using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlayWin();
            SkinManager.Instance.AddCoins();
            LevelsManager.Instance.LevelCompleted();
        }
    }
}
