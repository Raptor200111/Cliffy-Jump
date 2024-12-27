using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private float descendSpeed = 5f;
    [SerializeField] private float ascendSpeed = 8f;
    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private float destroyHeight = 20f;

    private Transform player;
    private bool hasLanded = false;
    private Vector3 landingPosition;
    private Vector3 randomDirection;
    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.player.playableCharacter.transform;
        // Start position high above the circuit
        transform.position = new Vector3(transform.position.x, 15f, transform.position.z);

        // Calculate landing position by raycasting down
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            landingPosition = hit.point;
        }
    }

    private void Update()
    {
        if (!hasLanded)
        {
            // Descend towards landing position
            transform.position = Vector3.MoveTowards(transform.position, landingPosition, descendSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, landingPosition) < 0.1f)
            {
                hasLanded = true;
            }
        }
        else
        {
            // Check for player proximity
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                // Generate random direction for ascent
                randomDirection = new Vector3(
                    Random.Range(-1f, 1f),
                    1f,
                    Random.Range(-1f, 1f)
                ).normalized;

                // Move upward in random direction
                transform.position += randomDirection * ascendSpeed * Time.deltaTime;

                // Destroy when high enough
                if (transform.position.y > destroyHeight)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize detection range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
