using System.Diagnostics;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PauseGame()
    {
            Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
            Time.timeScale = 1f;
    }
}
