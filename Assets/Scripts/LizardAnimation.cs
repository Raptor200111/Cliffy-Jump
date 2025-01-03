using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardAnimation : MovDeco
{

    TrailRenderer _trailRenderer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        appearSpeed = 1f;
        disappearSpeed = 2f;
        _trailRenderer = GetComponent<TrailRenderer>();
        disappear = false;
        _trailRenderer.enabled = true;
    }

    public override void Appear(Vector3 targetPos)
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
            Start();
        }
        _animator.SetBool("Idle", false);
        StartCoroutine(ClimbToTarget(targetPos));
    }

    private IEnumerator ClimbToTarget(Vector3 targetPos)
    {
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, appearSpeed * Time.deltaTime);
            yield return null;
        }

        _animator.SetBool("Idle", true);
        _trailRenderer.enabled = false;
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
        _animator.SetBool("Idle", false);
        _trailRenderer.enabled = true;
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
