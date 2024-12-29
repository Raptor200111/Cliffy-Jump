using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardAnimation : MonoBehaviour
{
    private float appearSpeed = 0.5f;
    private float disappearSpeed = 1f;
    private bool disappear = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void AppearAnim(Vector3 targetPos)
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
