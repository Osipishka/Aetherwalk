using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinSlot : MonoBehaviour
{
    [Header("Skin Settings")]
    [SerializeField] internal int slotIndex;
    [SerializeField] internal int price;
    [SerializeField] internal Sprite skinSprite;
    [SerializeField] internal bool isDefault;

    [Header("UI Elements")]
    [SerializeField] internal Button buyButton;
    [SerializeField] internal Button equipButton;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] internal GameObject priceUI;

    internal bool isEquiped;
    internal bool isPurchased;

    private void Start()
    {
        priceText.text = price.ToString();
        isPurchased = isDefault;

        if (isDefault)
        {
            priceUI.SetActive(false);
            equipButton.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(false);
        }
        else
        {
            equipButton.gameObject.SetActive(false);
            buyButton.gameObject.SetActive(true);
        }

        buyButton.onClick.AddListener(BuySkin);
        equipButton.onClick.AddListener(EquipSkin);
    }

    public void Update()
    {
        if(SkinManager.Instance != null)
        {
            if (price > SkinManager.Instance.CoinsCount)
            {
                buyButton.interactable = false;
            }
            else
            {
                buyButton.interactable = true;
            }
        }

    }

    private void BuySkin()
    {
        if (SkinManager.Instance.TryBuySkin(price))
        {
            isPurchased = true;
            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);
            GameSaveManager.Instance.SaveAllData();
        }
    }

    private void EquipSkin()
    {
        SkinManager.Instance.ChangeSkin(skinSprite);
        isEquiped = true;
        GameSaveManager.Instance.SaveAllData();
    }
}