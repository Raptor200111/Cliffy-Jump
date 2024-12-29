using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnScript : MonoBehaviour
{
    public int rotation = 0;
    bool changed = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.GetComponent<Player>().ChangeDirection(this.transform.position, rotation);
            changed = false;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        Vector2 from = new Vector2(transform.position.x, transform.position.z);
        Vector2 to = new Vector2(other.transform.position.x, other.transform.position.z);

        float distance = Vector2.Distance(from, to);
        //float distance2 = Vector2.Distance(other.transform.position, transform.position);

        if (other.tag == "Player" && !changed && distance <= 0.2f )
        {
            changed = true;
            other.GetComponent<Player>().ChangeDirection(this.transform.position, rotation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //other.GetComponent<Player>().ChangeDirection(this.transform.position, rotation);
            changed = false;
        }
    }
}
