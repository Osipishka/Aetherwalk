using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Coins")]
    [SerializeField] internal TMP_Text gameCoinsCountText;
    [SerializeField] internal TMP_Text shopCoinsCountText;
    [SerializeField] internal TMP_Text winCoinsCountText;

    [Header("Level UI")]
    [SerializeField] internal TMP_Text currentLevelText;
    [SerializeField] internal TMP_Text nextLevelText;
    [SerializeField] internal Image levelProgress;

    [Header("Diamond")]
    [SerializeField] internal Image[] diamondImages;
    [SerializeField] internal Animator[] diamondAnimators;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        LoadUIProgress();
    }

    private void LoadUIProgress()
    {
        SkinManager.Instance.UpdateCoinsUI();
    }
}
