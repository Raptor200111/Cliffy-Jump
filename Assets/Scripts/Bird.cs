using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private float descendSpeed = 5f;
    [SerializeField] private float ascendSpeed = 8f;
    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private float destroyHeight = 20f;

    private Transform playerTransform;
    private bool hasLanded = false;
    private bool isRuningAway = false;
    private Vector3 landingPosition;
    private Vector3 randomDirection;
    private GameManager gameManager;
    [SerializeField] Vector3 posBackUp;
    private void Start()
    {
        gameManager = GameManager.Instance;
        playerTransform = gameManager.GetPlayerTransform();
        // Start position high above the circuit
        transform.position = new Vector3(transform.position.x, 15f, transform.position.z);

        landingPosition = Vector3.one;
        posBackUp = landingPosition;
    }

    public void SetLandingPosition(Vector3 targetPos)
    {
        landingPosition = targetPos;
        posBackUp = targetPos;
        StartCoroutine(FlyToTarget(targetPos));
    }

    private IEnumerator FlyToTarget(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, descendSpeed * Time.deltaTime);
            yield return null;
        }

        hasLanded = true;
        // Snap the bird to the target position when close enough
        if (targetPos != Vector3.one)
        {
            transform.position = targetPos;
            landingPosition = targetPos;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ERROR targetPos BORD LANDING");
        }
        
    }

    private void Update()
    {
        if (hasLanded)
        {
            // Check for player proximity
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= detectionRange)
            {
                FlyAway();                
            }
        }
    }
    private IEnumerator FlyToSky(Vector3 randomDirection)
    {
        while (transform.position.y <= destroyHeight)
        {
            transform.position += randomDirection * ascendSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    public void FlyAway()
    {
        // Generate random direction for ascent
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            1f,
            Random.Range(-1f, 1f)
        ).normalized;

        if (!isRuningAway)
        {
            StartCoroutine(FlyToSky(randomDirection));
            isRuningAway = true;

        }
    }

    private void OnDrawGizmos()
    {
        // Visualize detection range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
