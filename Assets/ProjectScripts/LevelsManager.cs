using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

[System.Serializable]
public class Level
{
    [SerializeField] internal GameObject levelPrefab;
    [SerializeField] internal Button levelButton;
    [SerializeField] internal bool isBlocked = true;
    [SerializeField] internal Sprite lockedImage;
    [SerializeField] internal Sprite unlockedImage;
}

public class LevelsManager : MonoBehaviour
{
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";
    public static LevelsManager Instance;

    [Header("Level Settings")]
    [SerializeField] private Level[] levels;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Canvas loseCanvas;
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private Button nextLevelButton;
    [Header("Level Settings")]
    [SerializeField] private GameObject[] LevelPages;

    private const string UnlockedLevelsKey = "UnlockedLevels";
    private GameObject currentLevel;
    private GameObject currentPlayer;
    private int currentLevelId = 0;
    private int lastUnlockedLevel = 0;

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

        LoadUnlockedLevels();
        InitializeLevelButtons();
        HideAllCanvases();
    }

    private void LoadUnlockedLevels()
    {
        lastUnlockedLevel = PlayerPrefs.GetInt(UnlockedLevelsKey, 0);
        currentLevelId = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0);

        if (levels.Length > 0 && lastUnlockedLevel == 0)
        {
            lastUnlockedLevel = 0;
            PlayerPrefs.SetInt(UnlockedLevelsKey, lastUnlockedLevel);
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, 0);
            PlayerPrefs.Save();
        }
    }

    private void InitializeLevelButtons()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            int levelId = i;
            levels[i].isBlocked = levelId > lastUnlockedLevel;

            if (levels[i].levelButton != null)
            {
                levels[i].levelButton.image.sprite = levels[i].isBlocked ?
                    levels[i].lockedImage : levels[i].unlockedImage;

                levels[i].levelButton.interactable = !levels[i].isBlocked;

                if (!levels[i].isBlocked)
                {
                    levels[i].levelButton.onClick.AddListener(() => StartLevel(levelId));
                }
            }
        }
    }

    public void StartLevel(int levelId)
    {
        CleanUpCurrentLevel();
        TimeManager.Instance.ResumeGame();
        currentLevelId = levelId;
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, currentLevelId);
        PlayerPrefs.Save();

        if (levelId >= 0 && levelId < levels.Length)
        {
            currentLevel = Instantiate(levels[levelId].levelPrefab);
            spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

            UIManager.Instance.currentLevelText.text = (currentLevelId + 1).ToString();
            if (currentLevelId + 2 > levels.Length)
            {
                UIManager.Instance.nextLevelText.text = "Max";
                nextLevelButton.interactable = false;
            }
            else
            {
                UIManager.Instance.nextLevelText.text = (currentLevelId + 2).ToString();
                nextLevelButton.interactable = true;
            }
            if (player != null && spawnPoint != null)
            {
                currentPlayer = Instantiate(player, spawnPoint.transform.position, spawnPoint.transform.rotation);
                virtualCamera.Follow = currentPlayer.transform;
            }
            HideAllCanvases();
            SkinManager.Instance.DeleteProfit();
        }
    }

    public void StartLastUnlockedLevel()
    {
        StartLevel(lastUnlockedLevel);
    }

    public void LevelCompleted()
    {
        StartCoroutine(ShowDiamondsAnimation());
        if (currentLevelId >= lastUnlockedLevel && currentLevelId < levels.Length - 1)
        {
            lastUnlockedLevel = currentLevelId + 1;
            PlayerPrefs.SetInt(UnlockedLevelsKey, lastUnlockedLevel);

            if (lastUnlockedLevel < levels.Length)
            {
                levels[lastUnlockedLevel].isBlocked = false;
                levels[lastUnlockedLevel].levelButton.image.sprite =
                    levels[lastUnlockedLevel].unlockedImage;
                levels[lastUnlockedLevel].levelButton.interactable = true;
                levels[lastUnlockedLevel].levelButton.onClick.AddListener(
                    () => StartLevel(lastUnlockedLevel));
            }
        }
    }

    public void ShowWinCanvas()
    {
        winCanvas.enabled = true;
        loseCanvas.enabled = false;
    }

    public void ShowLoseCanvas()
    {
        winCanvas.enabled = false;
        loseCanvas.enabled = true;
    }

    private void HideAllCanvases()
    {
        winCanvas.enabled = false;
        loseCanvas.enabled = false;
    }

    public void LoadNextLevel()
    {
        if (currentLevelId < levels.Length - 1)
        {
            TimeManager.Instance.ResumeGame();
            CleanUpCurrentLevel();

            currentLevelId++;
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, currentLevelId);
            PlayerPrefs.Save();

            currentLevel = Instantiate(levels[currentLevelId].levelPrefab);

            GameObject newSpawnPoint = FindSpawnPointInLevel(currentLevel);
            if (newSpawnPoint == null)
            {
                return;
            }
            spawnPoint = newSpawnPoint;

            UpdateLevelUI();

            if (player != null)
            {
                currentPlayer = Instantiate(player, spawnPoint.transform.position, spawnPoint.transform.rotation);
                virtualCamera.Follow = currentPlayer.transform;
            }

            HideAllCanvases();
            SkinManager.Instance.DeleteProfit();
        }
        else
        {
            ReturnToMenu();
        }
    }

    public void RestartLevel()
    {
        StartLevel(currentLevelId);
    }

    public void ReturnToMenu()
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
            SkinManager.Instance.DeleteProfit();
            if (currentPlayer != null) Destroy(currentPlayer);
            if(spawnPoint!= null) Destroy(spawnPoint);
            currentLevel = null;
        }
        HideAllCanvases();
    }

    private IEnumerator ShowDiamondsAnimation()
    {
        ShowWinCanvas();

        foreach (var img in UIManager.Instance.diamondImages)
        {
            img.gameObject.SetActive(false);
        }

        for (int i = 0; i < SkinManager.Instance.diamondCount; i++)
        {
            UIManager.Instance.diamondImages[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        TimeManager.Instance.PauseGame();
    }

    public int GetLastUnlockedLevel()
    {
        return lastUnlockedLevel;
    }

    public void SetLastUnlockedLevel(int level)
    {
        lastUnlockedLevel = level;
    }

    private int currentPageIndex = 0;
    public void PageButton(int direction)
    {
        if (direction != 1 && direction != -1)
        {
            return;
        }

        int newIndex = currentPageIndex + direction;

        if (newIndex < 0)
        {
            return;
        }
        else if (newIndex >= LevelPages.Length)
        {
            return;
        }
        LevelPages[currentPageIndex].SetActive(false);
        currentPageIndex = newIndex;
        LevelPages[currentPageIndex].SetActive(true);
    }

    private void CleanUpCurrentLevel()
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel);
            currentLevel = null;
        }

        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        spawnPoint = null;
    }

    private GameObject FindSpawnPointInLevel(GameObject level)
    {
        foreach (Transform child in level.transform)
        {
            if (child.CompareTag("SpawnPoint"))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void UpdateLevelUI()
    {
        UIManager.Instance.currentLevelText.text = (currentLevelId + 1).ToString();

        if (currentLevelId + 2 > levels.Length)
        {
            UIManager.Instance.nextLevelText.text = "Max";
            nextLevelButton.interactable = false;
        }
        else
        {
            UIManager.Instance.nextLevelText.text = (currentLevelId + 2).ToString();
            nextLevelButton.interactable = true;
        }
    }
}