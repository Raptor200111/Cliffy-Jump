using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject currentModelInstance;

    enum State {Waiting, Moving, Jumping, Dead};
    State playerState;
    Vector3 velocity;
    Collider _collider;
    Rigidbody _rigidbody;
    Animator _animator;

    int blockLayer;
    int obstaclesLayer;
    int collectiblesLayer;

    float jumpSpeed = 30.0f;
    float groundSpeed = 0.18f;
    float gravity = -130f;

    public GameObject startPoint;
    public ParticleSystem deathParticle;
    public ParticleSystem groundParticle;


    void Start()
    {
        transform.position = startPoint.transform.position;

        blockLayer = LayerMask.NameToLayer("Blocks");
        obstaclesLayer = LayerMask.NameToLayer("Obstacles");
        collectiblesLayer = LayerMask.NameToLayer("Collectibles");

        playerState = State.Waiting;
        velocity = Vector3.zero;
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        UnityEngine.Physics.gravity = new Vector3(0, gravity, 0);

        LoadModel();
    }

    public void LoadModel()//PlayerModelData modelData)
    {
        GameObject modelData = WorldManager.Instance.GetModelData();
        //transform.GetChild(0).gameObject = Instantiate(modelData);
        //Destroy(transform.GetChild(0).gameObject);
        Instantiate(modelData, transform.GetChild(0));
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == obstaclesLayer)
        {
            //UnityEngine.Debug.Log("Death");
            //ChangePlayerState(State.Waiting);
            ChangePlayerState(State.Dead);
            Instantiate(deathParticle, transform.position, Quaternion.identity);
        }
        else if (collision.gameObject.layer == collectiblesLayer)
        {
            AddCollectibles(collision.gameObject);
        }
        else if (collision.gameObject.layer == blockLayer && playerState == State.Jumping) 
        {
            ChangePlayerState(State.Moving);
            Instantiate(groundParticle, transform.position, Quaternion.identity);
        }
    }

    public void PlayerStart()
    {
        transform.position = startPoint.transform.position;
        ChangePlayerState(State.Moving);
        this.GetComponent<TrailRenderer>().Clear();
    }

    public void PlayerStop()
    {
        ChangePlayerState(State.Waiting);
    }

    public void PlayerDeathAnimationComplete()
    {
        WorldManager.Instance.PlayerDeath();
    }

    void ChangePlayerState(State newState)
    {
        switch (newState)
        {
            case State.Waiting:
                velocity = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
                _animator.SetBool("Jumping", false);
                break;
            case State.Moving:
                _rigidbody.velocity = Vector3.zero;
                velocity = transform.forward * groundSpeed;
                _animator.SetBool("Jumping", false);
                break;
            case State.Jumping:
                _rigidbody.velocity = new Vector3(0, jumpSpeed, 0);
                _animator.SetBool("Jumping", true);
                break;
            case State.Dead:
                velocity = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
                _animator.SetTrigger("Dead");
                _animator.SetBool("Jumping", false);
                break;

        }
        playerState = newState;
    }

    public void ChangeDirection(Vector3 pos, Vector3 fow)
    {
        if (playerState == State.Moving) 
        {
            pos.y = transform.position.y;
            transform.localPosition = pos;
            //transform.forward = fow;
            //transform.LookAt(transform.position + fow);
            velocity = 0.2f * fow;
        }
    }

    private void AddCollectibles(GameObject collectible)
    {
        if (collectible.CompareTag("Star"))
        {
            GameManager.Instance.AddStar(collectible);            
        }
        else if (collectible.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoin(collectible);
        }
        else
        {
            //add live
        }
    }
}
