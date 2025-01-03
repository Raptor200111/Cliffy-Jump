using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeController : MovDeco
{

    private GameManager gameManager;
    private Transform playerTransform;
    [SerializeField] private GameObject eyeball;
    private float rotationSpeed = 1.5f;
    private bool rotatingRight = true;
    private float rotationAngle = 60f;

    private bool idle = false;
    private Vector3 startPosition;
    private float frequency = 0.5f;
    private float amplitude = 1f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        appearSpeed = 1.5f;
        disappearSpeed = 1f;
        playerTransform = GameManager.Instance.Player.transform;
    }

    public override void Appear(Vector3 targetPos)
    {
        startPosition = targetPos;
        StartCoroutine(AscendToTarget(targetPos));
    }

    private IEnumerator AscendToTarget(Vector3 targetPos)
    {
        //Debug.Log("AscendToTarget targetpos " + targetPos + "transformpos " + this.transform.position);

        while (Mathf.Abs(transform.position.y - targetPos.y) > 0.1f)
        {
            float newY = Mathf.MoveTowards(transform.position.y, targetPos.y, appearSpeed * Time.deltaTime);
            Vector3 move = new Vector3(transform.position.x, newY, transform.position.z);
            //            Debug.Log("target y: " + targetPos.y + " current y: " + newY);
            if (move.y > 7.5f)
            {
                move.y = 7f;
                transform.position = move;
                break;
            }
            transform.position = move;
            yield return null;
        }
        idle = true;

        if (targetPos != Vector3.one)
        {
            transform.position = targetPos;
        }
        else
        {
            UnityEngine.Debug.LogWarning("ERROR targetPos BORD LANDING");
        }
    }

    private void RotateEye()
    {
        float yRotation = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
        eyeball.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }


    // Update is called once per frame
    void Update()
    {
        if ( !disappear)
        {
            RotateEye();
            if (idle) {
                float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
                transform.position = startPosition + new Vector3(0, yOffset, 0);
            }
        }
        else
        {
            idle = false;
            eyeball.transform.LookAt(playerTransform);
        }        
    }

    public override void Disappear()
    {
        //_animator.SetBool("Idle", false);
        StartCoroutine(DescentToWater());
    }

    private IEnumerator DescentToWater()
    {
        while (transform.position.y >= -3f)
        {
            transform.position += new Vector3(0f, -1f, 0f) * disappearSpeed * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
