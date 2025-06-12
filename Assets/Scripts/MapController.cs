using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject WallPre;
    public GameObject IcePre;
    public GameObject FruitPre;
    public GameObject EnemyPre;
    public GameObject PlayerPre;

    private int X, Y;
    private HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();
    private GameObject playerInstance;
    private List<GameObject> enemyInstances = new List<GameObject>();

    private void Awake()
    {
        IniteMap(9, 5);
    }

    public void IniteMap(int x, int y)
    {
        X = x; Y = y;

 
        CreateWall();
        CreatePlayer();
        CreateEnemies(3); 


        CreateIce(30); 
        CreateFruits(10); 
    }

    private void CreateWall()
    {
        // Coboundary
        for (int x = -(X + 2); x <= X - 2; x++)
        {
            Vector2 wallPos = new Vector2(x + 2, Y);
            InstantiateAndTrack(WallPre, wallPos);

            Vector2 wall1Pos = new Vector2(x + 2, -Y);
            InstantiateAndTrack(WallPre, wall1Pos);
        }

        // Left and right boundaries
        for (int y = -Y; y <= Y - 1; y++)
        {
            Vector2 wallPos = new Vector2(-X, y);
            InstantiateAndTrack(WallPre, wallPos);

            Vector2 wall1Pos = new Vector2(X, y);
            InstantiateAndTrack(WallPre, wall1Pos);
        }
    }

    private void CreatePlayer()
    {
        Vector2 playerPos = new Vector2(-X + 1, Y - 1); 
        playerInstance = InstantiateAndTrack(PlayerPre, playerPos);
        Debug.Log($"Player created at {playerPos}");
    }

    private void CreateEnemies(int count)
    {
        if (count < 3)
        {
            Debug.LogWarning("Not enough enemy positions defined for count: " + count);
            return;
        }

  
        Vector2 enemyPos1 = new Vector2(-X + 1, -Y + 1);
        GameObject enemy1 = InstantiateAndTrack(EnemyPre, enemyPos1);
        enemyInstances.Add(enemy1);
        Debug.Log($"Enemy 1 created at {enemyPos1}");

      
        Vector2 enemyPos2 = new Vector2(X - 1, -Y + 1);
        GameObject enemy2 = InstantiateAndTrack(EnemyPre, enemyPos2);
        enemyInstances.Add(enemy2);
        Debug.Log($"Enemy 2 created at {enemyPos2}");

       
        Vector2 enemyPos3 = new Vector2(X - 1, Y - 1);
        GameObject enemy3 = InstantiateAndTrack(EnemyPre, enemyPos3);
        enemyInstances.Add(enemy3);
        Debug.Log($"Enemy 3 created at {enemyPos3}");
    }

    private void CreateIce(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPos = FindEmptyPosition();
            if (randomPos != Vector2.negativeInfinity)
            {
                InstantiateAndTrack(IcePre, randomPos);
            }
        }
    }

    private void CreateFruits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPos = FindEmptyPosition();
            if (randomPos != Vector2.negativeInfinity)
            {
                InstantiateAndTrack(FruitPre, randomPos);
            }
        }
    }


    private GameObject InstantiateAndTrack(GameObject prefab, Vector2 position)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.transform.position = position;
        occupiedPositions.Add(position);
        return obj;
    }

    // Find empty position
    private Vector2 FindEmptyPosition(int maxAttempts = 100)
    {
        Vector2 randomPos;
        int attempts = 0;

        do
        {
            randomPos = new Vector2(
                Random.Range(-X + 1, X),
                Random.Range(-Y + 1, Y)
            );
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogWarning("Could not find empty position");
                return Vector2.negativeInfinity;
            }
        } while (occupiedPositions.Contains(randomPos));

        return randomPos;
    }

    public GameObject GetPlayer()
    {
        return playerInstance;
    }

    public List<GameObject> GetEnemies()
    {
        return enemyInstances;
    }
}