using System;
using System.Collections;
using System.Collections.Generic;
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
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        UnityEngine.Physics.gravity = new Vector3(0, gravity, 0);
    }

    public void LoadModel(PlayerModelData modelData)
    {
        // Clean up old model
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }

        // Instantiate the new model
        if (modelData != null && modelData.modelPrefab != null)
        {
            currentModelInstance = Instantiate(modelData.modelPrefab, this.transform);
            
            /*if (_animator != null)
            {
                _animator.runtimeAnimatorController = modelData.animatorController;
            }*/
        }
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

        _animator.SetFloat("Yvelocity", _rigidbody.velocity.y);
    }

    void FixedUpdate()
    {
        transform.position += velocity;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 0.625f);
        
        foreach (RaycastHit hit in hits)
        {
            if ( hit.collider.gameObject.layer == blockLayer)
            {
                _rigidbody.velocity = Vector3.zero;
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
            //UnityEngine.Debug.Log("Death");
            //ChangePlayerState(State.Waiting);
            ChangePlayerState(State.Dead);
        }
        else if (collision.gameObject.layer == collecteblesLayer)
        {
            AddCollectibles(collision.gameObject);
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
                _animator.SetBool("Running", false);
                break;
            case State.Moving:
                _rigidbody.velocity = Vector3.zero;
                velocity = transform.forward * groundSpeed;
                _animator.SetBool("Running", true);
                break;
            case State.Jumping:
                _rigidbody.velocity = new Vector3(0, jumpSpeed, 0);
                _animator.SetTrigger("Jump");
                break;
            case State.Dead:
                velocity = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
                _animator.SetTrigger("Dead");
                break;

        }
        _animator.SetFloat("Yvelocity", _rigidbody.velocity.y);
        playerState = newState;
    }

    public void ChangeDirection(Vector3 pos, Vector3 fow)
    {
        if (playerState == State.Moving) 
        {
            pos.y = transform.position.y;
            transform.localPosition = pos;
            transform.forward = fow;
            velocity = 0.2f * fow;
        }
    }

    private void AddCollectibles(GameObject collectible)
    {
        if (collectible.CompareTag("Star"))
        {
            WorldManager.Instance.AddStar();            
        }
        else if (collectible.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoin();
        }
        else
        {
            //add live
        }
    }
}
