using UnityEngine;

public class PlayerTask : MonoBehaviour
{
    private int fruitsCollected = 0;
    private const int totalFruits = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fruit"))
        {
            CollectFruit(other.gameObject);
        }
    }

    private void CollectFruit(GameObject fruit)
    {
        fruitsCollected++;
        Destroy(fruit);

        if (fruitsCollected >= totalFruits)
        {
            FindObjectOfType<GameController>().Victory("Congratulations! You've collected all fruits!");
        }
    }
}