using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardAnimation : MovDeco
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        appearSpeed = 0.8f;
        disappearSpeed = 1f;
        disappear = false;
    }

    public override void Appear(Vector3 targetPos)
    {
        StartCoroutine(ClimbToTarget(targetPos));
    }

    private IEnumerator ClimbToTarget(Vector3 targetPos)
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
        }
        else
        {
            UnityEngine.Debug.LogWarning("ERROR targetPos BORD LANDING");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(disappear)
        {
            if (transform.position.y >= 0f)
            {
                transform.position += new Vector3(0f, -1f, 0f) * disappearSpeed * Time.deltaTime;                
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public override void Disappear()
    {
        StartCoroutine(DescentToWater());
    }

    private IEnumerator DescentToWater()
    {
        while (transform.position.y >= 0f)
        {
            transform.position += new Vector3(0f, -1f, 0f) * disappearSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
