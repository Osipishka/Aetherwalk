using UnityEngine;

public class Diamond : MonoBehaviour
{
    [SerializeField] private int coins;
    [SerializeField] private ParticleSystem collectParticles;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collectParticles != null)
            {
                Instantiate(collectParticles, transform.position, Quaternion.identity);
            }

            AudioManager.Instance.PlayDiamondInteraction();
            SkinManager.Instance.PossibleProfit(coins);
            SkinManager.Instance.AddDiamond();
            Destroy(gameObject);
        }
    }
}