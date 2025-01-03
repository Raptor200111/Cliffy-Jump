using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private bool isAnimated = true;

    [SerializeField] private bool isRotating = true;
    [SerializeField] private bool isScaling = true;

    private Vector3 rotationAngle;
    [SerializeField] protected float rotationSpeed = 10;


    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;

    private bool scalingUp;
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private float scaleRate = 0.5f;
    private float scaleTimer;
    protected Animator _animator;

    // Use this for initialization
    protected virtual void Start()
    {

        rotationAngle = new Vector3(0f, 10f, 0f);
        rotationSpeed = 10f;


        //startScale = Vector3.one;
        //endScale = Vector3.one;

        scalingUp = true;
        scaleSpeed = 1;
        scaleRate = 0.5f;
        scaleTimer = 0f;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isAnimated)
        {
            if (isRotating)
            {
                transform.Rotate(rotationAngle * rotationSpeed * Time.deltaTime);
            }

            if (isScaling)
            {
                scaleTimer += Time.deltaTime;

                if (scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, endScale, scaleSpeed * Time.deltaTime);
                }
                else if (!scalingUp)
                {
                    transform.localScale = Vector3.Lerp(transform.localScale, startScale, scaleSpeed * Time.deltaTime);
                }

                if (scaleTimer >= scaleRate)
                {
                    if (scalingUp) { scalingUp = false; }
                    else if (!scalingUp) { scalingUp = true; }
                    scaleTimer = 0;
                }
            }
        }
    }

    public virtual void Appear()
    {
        // Default appear animation (if any)
    }

    public virtual void Disappear()
    {
        Destroy(gameObject);
        //_animator.SetTrigger("Die");                    
    }

    public virtual void OnDieComplete()
    {
        Destroy(gameObject);
    }


}
