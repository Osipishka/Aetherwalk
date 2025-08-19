using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    [SerializeField] private SpriteRenderer ball;
    [SerializeField] private int coinsCount;

    internal int diamondCount;
    private int currentCoinCount;

    public event System.Action<int> OnCoinsChanged;

    public int CoinsCount
    {
        get => coinsCount;
        set
        {
            coinsCount = Mathf.Max(0, value);
            PlayerPrefs.SetInt("PlayerCoins", coinsCount);
            UpdateCoinsUI();
            OnCoinsChanged?.Invoke(coinsCount);
        }
    }

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

    public void UpdateCoinsUI()
    {
        UIManager.Instance.gameCoinsCountText.text = coinsCount.ToString();
        UIManager.Instance.shopCoinsCountText.text = coinsCount.ToString();
    }

    public void ChangeSkin(Sprite newSkin)
    {
        ball.sprite = newSkin;
    }

    public bool TryBuySkin(int price)
    {
        if (CoinsCount >= price)
        {
            CoinsCount -= price;
            return true;
        }
        return false;
    }

    public void AddCoins()
    {
        coinsCount += currentCoinCount;
        UIManager.Instance.winCoinsCountText.text = currentCoinCount.ToString();
        UpdateCoinsUI();
    }

    public void PossibleProfit(int amount)
    {
        currentCoinCount += amount;
    }

    public void AddDiamond()
    {
        diamondCount++;
    }

    public void DeleteProfit()
    {
        currentCoinCount = 0;
        diamondCount = 0;
    }

    public Sprite GetCurrentSkin()
    {
        return ball.sprite;
    }
}