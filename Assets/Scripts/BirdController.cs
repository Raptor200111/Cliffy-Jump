using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] private float appearSpeed = 5f;
    [SerializeField] private float disappearSpeed = 8f;
    [SerializeField] private float destroyHeight = 20f;

    private bool disappear = false;
    private Vector3 landingPosition;
    private Vector3 randomDirection;
    private void Start()
    {
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            1f,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    public void AppearAnim(Vector3 targetPos)
    {;
        StartCoroutine(FlyToTarget(targetPos));
    }

    private IEnumerator FlyToTarget(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, appearSpeed * Time.deltaTime);
            yield return null;
        }

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
        if (disappear)
        {
            if (transform.position.y <= destroyHeight)
            {
                transform.position += randomDirection * disappearSpeed * Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the sphere collider
        if (other.gameObject.tag == ("Player"))
        {
            disappear = true;
        }
    }

    public void DisappearAnim()
    {
        StartCoroutine(FlyToSky());
    }
    private IEnumerator FlyToSky()
    {
        while (transform.position.y <= destroyHeight)
        {
            transform.position += randomDirection * disappearSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
