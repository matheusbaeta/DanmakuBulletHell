using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHits = 3;
    private int currentHits;

    private void Awake()
    {
        this.currentHits = maxHits;
    }

    public void TakeHit()
    {
        this.currentHits--;
        if (currentHits <= 0) Die();
    }

    public void Die()
    {
        // SceneManager.LoadScene("GameOver");
    }

    public void IncreaseLife()
    {
        this. currentHits++;
        if (currentHits > maxHits) currentHits = maxHits;
    }

    public bool IsDead()
    {
        return currentHits <= 0; 
    }
}
