using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float rayDistance = 0.65f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float turnCooldown = 0.5f;
    [SerializeField] private float collisionOffset = 0.2f;
    [SerializeField] private float cornerStuckCheckDistance = 0.3f;
    [SerializeField] private float idleDuration = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

    private Rigidbody2D rig;
    private Vector2 dirVector;
    private float lastTurnTime;
    private int consecutiveTurns = 0;
    private Vector2 lastPosition;
    private float stuckTimer = 0f;

    // FSM States
    private enum EnemyState { Moving, Turning, Idle, Stuck }
    private EnemyState currentState;
    private float stateTimer;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        currentState = EnemyState.Idle;
        stateTimer = idleDuration;
        lastPosition = transform.position;
    }

    private void Update()
    {
        StateUpdate();
    }

    private void StateUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Moving:
                UpdateMovingState();
                break;
            case EnemyState.Turning:
                UpdateTurningState();
                break;
            case EnemyState.Idle:
                UpdateIdleState();
                break;
            case EnemyState.Stuck:
                UpdateStuckState();
                break;
        }
    }

    private void UpdateMovingState()
    {
        // Check for obstacles
        RaycastHit2D hit = Physics2D.BoxCast(
            (Vector2)transform.position + dirVector * collisionOffset,
            Vector2.one * 0.8f,
            0f,
            dirVector,
            rayDistance,
            obstacleLayer);

        if (hit.collider != null && (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Ice")))
        {
            ChangeState(EnemyState.Turning);
            return;
        }

        // Check if stuck
        if (Vector2.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > 0.5f)
            {
                ChangeState(EnemyState.Stuck);
                return;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        lastPosition = transform.position;
        rig.velocity = dirVector * speed;
    }

    private void UpdateTurningState()
    {
        if (Time.time < lastTurnTime + turnCooldown) return;

        consecutiveTurns++;
        if (consecutiveTurns > 2)
        {
            ChangeState(EnemyState.Stuck);
            return;
        }

        rig.velocity = Vector2.zero;
        SnapToGrid();

        Vector2[] testDirections = GetPriorityDirections(dirVector);
        foreach (Vector2 dir in testDirections)
        {
            if (!CheckObstacle(dir))
            {
                dirVector = dir;
                lastTurnTime = Time.time;
                ChangeState(EnemyState.Moving);
                return;
            }
        }

        // If no valid direction found
        ChangeState(EnemyState.Stuck);
    }

    private void UpdateIdleState()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            SetRandomDirection();
            ChangeState(EnemyState.Moving);
        }
    }

    private void UpdateStuckState()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f)
        {
            ForceTurnAwayFromCorner();
            ChangeState(EnemyState.Moving);
        }
    }

    private void ChangeState(EnemyState newState)
    {
        // Exit current state
        switch (currentState)
        {
            case EnemyState.Moving:
                rig.velocity = Vector2.zero;
                break;
        }

        // Enter new state
        switch (newState)
        {
            case EnemyState.Idle:
                stateTimer = idleDuration;
                break;

            case EnemyState.Stuck:
                stateTimer = 0.5f; // Time to try to get unstuck
                break;

            case EnemyState.Turning:
                break;

            case EnemyState.Moving:
                consecutiveTurns = 0;
                stuckTimer = 0f;
                break;
        }

        currentState = newState;
    }

    private void ForceTurnAwayFromCorner()
    {
        Vector2[] testDirections = {
            new Vector2(-dirVector.y, dirVector.x),
            new Vector2(dirVector.y, -dirVector.x),
            -dirVector,
            GetRandomDirection()
        };

        foreach (Vector2 dir in testDirections)
        {
            if (!CheckObstacle(dir))
            {
                dirVector = dir;
                return;
            }
        }

        // If completely stuck, go to idle
        ChangeState(EnemyState.Idle);
    }

    private void SetRandomDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        dirVector = directions[Random.Range(0, directions.Length)];
    }

    private Vector2[] GetPriorityDirections(Vector2 currentDir)
    {
        if (currentDir == Vector2.up || currentDir == Vector2.down)
        {
            return new Vector2[] {
                new Vector2(1f, 0f),
                new Vector2(-1f, 0f),
                -currentDir
            };
        }
        return new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(0f, -1f),
            -currentDir
        };
    }

    private Vector2 GetRandomDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        return directions[Random.Range(0, directions.Length)];
    }

    private bool CheckObstacle(Vector2 direction)
    {
        return Physics2D.BoxCast(
            (Vector2)transform.position + direction * collisionOffset,
            Vector2.one * 0.8f,
            0f,
            direction,
            rayDistance,
            obstacleLayer);
    }

    private void SnapToGrid()
    {
        Vector2 snappedPos = new Vector2(
            Mathf.Round(transform.position.x * 2) / 2f,
            Mathf.Round(transform.position.y * 2) / 2f);

        if (!Physics2D.OverlapPoint(snappedPos, obstacleLayer))
        {
            transform.position = snappedPos;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugRays) return;

        // Current movement direction
        Gizmos.color = Color.green;
        DrawBoxCastGizmo((Vector2)transform.position + dirVector * collisionOffset,
                        Vector2.one * 0.8f,
                        dirVector,
                        rayDistance);

        // Obstacle detection directions
        Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.7f);
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (Vector2 dir in directions)
        {
            DrawBoxCastGizmo((Vector2)transform.position + dir * collisionOffset,
                            Vector2.one * 0.8f,
                            dir,
                            rayDistance);
        }

        // Stuck detection area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.1f);

        // State indicator
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, currentState.ToString(), style);
    }

    private void DrawBoxCastGizmo(Vector2 origin, Vector2 size, Vector2 direction, float distance)
    {
        Gizmos.DrawWireCube(origin + direction * distance, size);
        Gizmos.DrawLine(origin - new Vector2(size.x / 2, size.y / 2),
                       origin - new Vector2(size.x / 2, size.y / 2) + direction * distance);
        Gizmos.DrawLine(origin + new Vector2(size.x / 2, -size.y / 2),
                       origin + new Vector2(size.x / 2, -size.y / 2) + direction * distance);
    }
}