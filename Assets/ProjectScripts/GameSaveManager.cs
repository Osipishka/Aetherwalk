using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    [Header("Autosave Settings")]
    [SerializeField] private float autoSaveInterval = 60f;

    private const string COINS_KEY = "PlayerCoins";
    private const string UNLOCKED_LEVELS_KEY = "UnlockedLevels";
    private const string PURCHASED_SKINS_KEY = "PurchasedSkins";
    private const string EQUIPPED_SKIN_KEY = "EquippedSkin";

    private Coroutine autoSaveCoroutine;
    private List<int> purchasedSkinIndices = new List<int>();
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(Initialize());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => SkinManager.Instance != null && LevelsManager.Instance != null);

        LoadAllData();
        InitializeAutoSave();
        isInitialized = true;
    }

    private void InitializeAutoSave()
    {
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveInterval);
            if (isInitialized) SaveAllData();
        }
    }

    public void SaveAllData()
    {
        if (!isInitialized) return;

        SaveCoins();
        SaveUnlockedLevels();
        SavePurchasedSkins();
        SaveEquippedSkin();
        PlayerPrefs.Save();
    }

    public void LoadAllData()
    {
        LoadCoins();
        LoadUnlockedLevels();
        LoadPurchasedSkins();
        LoadEquippedSkin();
    }

    #region Coins Save/Load
    private void SaveCoins()
    {
        if (SkinManager.Instance != null)
        {
            PlayerPrefs.SetInt(COINS_KEY, SkinManager.Instance.CoinsCount);
        }
    }

    private void LoadCoins()
    {
        if (PlayerPrefs.HasKey(COINS_KEY) && SkinManager.Instance != null)
        {
            SkinManager.Instance.CoinsCount = PlayerPrefs.GetInt(COINS_KEY);
        }
    }
    #endregion

    #region Levels Save/Load
    private void SaveUnlockedLevels()
    {
        if (LevelsManager.Instance != null)
        {
            PlayerPrefs.SetInt(UNLOCKED_LEVELS_KEY, LevelsManager.Instance.GetLastUnlockedLevel());
        }
    }

    private void LoadUnlockedLevels()
    {
        if (PlayerPrefs.HasKey(UNLOCKED_LEVELS_KEY) && LevelsManager.Instance != null)
        {
            int lastUnlocked = PlayerPrefs.GetInt(UNLOCKED_LEVELS_KEY);
            LevelsManager.Instance.SetLastUnlockedLevel(lastUnlocked);
        }
    }
    #endregion

    #region Skins Save/Load
    private void SavePurchasedSkins()
    {
        SkinSlot[] skinSlots = FindObjectsOfType<SkinSlot>();
        if (skinSlots == null || skinSlots.Length == 0) return;

        purchasedSkinIndices.Clear();
        foreach (var slot in skinSlots)
        {
            if (slot != null && slot.isPurchased)
            {
                purchasedSkinIndices.Add(slot.slotIndex);
            }
        }

        PlayerPrefs.SetString(PURCHASED_SKINS_KEY, string.Join(",", purchasedSkinIndices));
    }

    private void LoadPurchasedSkins()
    {
        if (!PlayerPrefs.HasKey(PURCHASED_SKINS_KEY)) return;

        string purchasedSkinsString = PlayerPrefs.GetString(PURCHASED_SKINS_KEY);
        if (string.IsNullOrEmpty(purchasedSkinsString)) return;

        SkinSlot[] skinSlots = FindObjectsOfType<SkinSlot>();
        if (skinSlots == null || skinSlots.Length == 0) return;

        purchasedSkinIndices = new List<int>();
        string[] indices = purchasedSkinsString.Split(',');
        foreach (string index in indices)
        {
            if (int.TryParse(index, out int skinIndex))
            {
                purchasedSkinIndices.Add(skinIndex);
            }
        }

        foreach (var slot in skinSlots)
        {
            if (slot == null) continue;

            bool isPurchased = purchasedSkinIndices.Contains(slot.slotIndex) || slot.isDefault;
            slot.isPurchased = isPurchased;

            if (slot.priceUI != null) slot.priceUI.SetActive(!isPurchased);
            if (slot.buyButton != null) slot.buyButton.gameObject.SetActive(!isPurchased);
            if (slot.equipButton != null) slot.equipButton.gameObject.SetActive(isPurchased);
        }
    }

    private void SaveEquippedSkin()
    {
        if (SkinManager.Instance == null) return;

        Sprite equippedSkin = SkinManager.Instance.GetCurrentSkin();
        if (equippedSkin == null) return;

        SkinSlot[] skinSlots = FindObjectsOfType<SkinSlot>();
        if (skinSlots == null || skinSlots.Length == 0) return;

        foreach (var slot in skinSlots)
        {
            if (slot != null && slot.skinSprite == equippedSkin)
            {
                PlayerPrefs.SetInt(EQUIPPED_SKIN_KEY, slot.slotIndex);
                return;
            }
        }
    }
    private void LoadEquippedSkin()
    {
        if (!PlayerPrefs.HasKey(EQUIPPED_SKIN_KEY)) return;
        if (SkinManager.Instance == null) return;

        int equippedSkinIndex = PlayerPrefs.GetInt(EQUIPPED_SKIN_KEY);
        SkinSlot[] skinSlots = FindObjectsOfType<SkinSlot>();
        if (skinSlots == null || skinSlots.Length == 0) return;

        foreach (var slot in skinSlots)
        {
            if (slot != null && slot.slotIndex == equippedSkinIndex)
            {
                SkinManager.Instance.ChangeSkin(slot.skinSprite);
                break;
            }
        }
    }
    #endregion

    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveAllData();
        }
    }

    public void SetAutoSaveInterval(float newInterval)
    {
        autoSaveInterval = newInterval;
        InitializeAutoSave();
    }
}