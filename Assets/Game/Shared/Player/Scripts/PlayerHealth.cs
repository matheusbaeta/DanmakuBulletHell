using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHits = 3;
    [SerializeField] private float invincibilityTime = 0.8f;

    private int currentHits;
    private bool invincible;

    public bool IsInvincible => invincible;

    private void Awake()
    {
        currentHits = maxHits;
    }

    public void TakeHit()
    {
        if (invincible) return;

        currentHits--;

        if (currentHits <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        invincible = false;
    }

    public bool IsDead() => currentHits <= 0;

    private void Die()
    {
        SceneManager.LoadScene("GameOver");
    }
}
