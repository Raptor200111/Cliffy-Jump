using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BirdController : MovDeco
{

    [SerializeField] private float destroyHeight = 20f;

    private Vector3 landingPosition;
    private Vector3 randomDirection;
    protected override void Start()
    {
        base.Start();
        appearSpeed = 5f;
        disappearSpeed = 8f;
        disappear = false;
        randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            1f,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    public override void Appear(Vector3 targetPos)
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

    public override void Disappear()
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
