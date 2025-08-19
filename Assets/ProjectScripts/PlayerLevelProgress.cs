using Unity.VisualScripting;
using UnityEngine;

public class PlayerLevelProgress : MonoBehaviour
{
    [SerializeField] private Transform finish;

    private float startX;
    private float finishX;

    private void Start()
    {
        finish = GameObject.FindGameObjectWithTag("Finish").transform;
        startX = transform.position.x;
        finishX = finish.position.x;
    }

    private void Update()
    {
        float progress = Mathf.Clamp01((transform.position.x - startX) / (finishX - startX));
        UIManager.Instance.levelProgress.fillAmount = progress;
    }
}
