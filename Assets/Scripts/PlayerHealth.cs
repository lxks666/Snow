using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int HP = 2;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvincible = false; 
    private float invincibleDuration = 2f; 

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isInvincible) return; 

        HP--;
        StartCoroutine(InjuredEffect(invincibleDuration));
        StartCoroutine(InvincibleCooldown(invincibleDuration));

        if (HP <= 0)
        {
            FindObjectOfType<GameController>().GameOver("You died!");
        }
    }

    // Injury Flash Effect
    private IEnumerator InjuredEffect(float duration)
    {
        for (int i = 0; i < duration * 2; i++)
        {
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
            yield return new WaitForSeconds(0.25f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.25f);
        }
    }

    // Coroutine in Invincible State
    private IEnumerator InvincibleCooldown(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
}