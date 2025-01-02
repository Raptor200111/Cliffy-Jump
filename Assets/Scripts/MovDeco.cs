using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovDeco : MonoBehaviour
{
    [SerializeField] protected float appearSpeed;
    [SerializeField] protected float disappearSpeed;
    protected bool disappear = false;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    public abstract void Appear(Vector3 targetPos);
    public abstract void Disappear();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            disappear = true;
        }
    }
}
