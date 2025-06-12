using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public GameObject icePre;
    private LayerMask iceLayer;
    private LayerMask wallLayer;

    private void Awake()
    {
        iceLayer = LayerMask.GetMask("Ice");
        wallLayer = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        CreateIce();
        RemoveIce();
    }

    //Create ice cube
    private void CreateIce()
    {
        Vector2 checkDirection = GetInputDirection();
        if (checkDirection != Vector2.zero)
        {
            Vector2 targetPosition = CalculateTargetPosition(checkDirection);
            if (!Physics2D.OverlapPoint(targetPosition, wallLayer))
            {
                Instantiate(icePre, targetPosition, Quaternion.identity);
            }
        }
    }

    //Remove ice cube
    private void RemoveIce()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Vector2 direction in new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right })
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, iceLayer);
                if (hit.collider != null && hit.collider.CompareTag("Ice"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    private Vector2 GetInputDirection()
    {
        if (Input.GetKeyDown(KeyCode.W)) return Vector2.up;
        if (Input.GetKeyDown(KeyCode.S)) return Vector2.down;
        if (Input.GetKeyDown(KeyCode.A)) return Vector2.left;
        if (Input.GetKeyDown(KeyCode.D)) return Vector2.right;
        return Vector2.zero;
    }

    private Vector2 CalculateTargetPosition(Vector2 direction)
    {
        return new Vector2(
            Mathf.Round(transform.position.x + direction.x),
            Mathf.Round(transform.position.y + direction.y)
        );
    }
}