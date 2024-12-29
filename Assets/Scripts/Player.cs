using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class Player : MonoBehaviour
{
    enum State {Waiting, Moving, Jumping};
    State playerState;
    Vector3 velocity;
    Collider Collider;
    Rigidbody Rigidbody;

    int blockLayer;
    int obstaclesLayer;
    int collecteblesLayer;

    public float jumpSpeed = 200.0f;
    public float groundSpeed = 0.12f;
    public float gravity = -100f;

    public GameObject startPoint;

    void Start()
    {
        transform.position = startPoint.transform.position;

        blockLayer = LayerMask.NameToLayer("Blocks");
        obstaclesLayer = LayerMask.NameToLayer("Obstacles");
        collecteblesLayer = LayerMask.NameToLayer("Collectebles");

        playerState = State.Waiting;
        velocity = Vector3.zero;
        Collider = GetComponent<Collider>();
        Rigidbody = GetComponent<Rigidbody>();

        UnityEngine.Physics.gravity = new Vector3(0, gravity, 0);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) 
        {
            if (playerState == State.Waiting)
            {
                ChangePlayerState(State.Moving);
            }
            else if (playerState == State.Moving)
            {
                ChangePlayerState(State.Waiting);
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && playerState == State.Moving)
        {
            ChangePlayerState(State.Jumping);
        }
    }

    void FixedUpdate()
    {
        transform.position += velocity;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 0.625f);
        
        foreach (RaycastHit hit in hits)
        {
            if ( hit.collider.gameObject.layer == blockLayer)
            {
                Rigidbody.velocity = Vector3.zero;
                ChangePlayerState(State.Waiting);
                UnityEngine.Debug.Log("Front Smash");
                break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == obstaclesLayer)
        {
            UnityEngine.Debug.Log("Death");
            ChangePlayerState(State.Waiting);
        }
        else if (collision.gameObject.layer == collecteblesLayer)
        {

        }
        else if (collision.gameObject.layer == blockLayer && playerState == State.Jumping) 
        {
            ChangePlayerState(State.Moving);
        }
    }

    public void PlayerStart()
    {
        transform.position = startPoint.transform.position;
        ChangePlayerState(State.Moving);
    }

    public void PlayerStop()
    {
        ChangePlayerState(State.Waiting);
    }

    void ChangePlayerState(State newState)
    {
        switch (newState)
        {
            case State.Waiting:
                velocity = Vector3.zero;
                Rigidbody.velocity = Vector3.zero;
                break;
            case State.Moving:
                Rigidbody.velocity = Vector3.zero;
                velocity = transform.forward * groundSpeed;
                break;
            case State.Jumping:
                Rigidbody.velocity = new Vector3(0, jumpSpeed, 0);
                break;
        }

        playerState = newState;
    }

    public void ChangeDirection(Vector3 pos, int rot)
    {
        if (playerState == State.Moving) 
        {
            pos.y = transform.position.y;
            Quaternion quat = Quaternion.Euler(0, rot, 0);
            this.transform.SetPositionAndRotation(pos, quat);
            velocity = 0.2f * transform.forward;
        }
    }
}
