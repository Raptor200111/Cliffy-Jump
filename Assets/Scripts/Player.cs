using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
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

    float jumpSpeed = 40.0f;
    float groundSpeed = 0.18f;
    float gravity = -130f;

    public GameObject startPoint;
    public ParticleSystem deathParticle;
    public ParticleSystem groundParticle;

    public bool godMode = false;
    public void Awake()
    {
        if (Player.Instance == null)
        {
            Player.Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        godMode = false;
        
        blockLayer = LayerMask.NameToLayer("Blocks");
        obstaclesLayer = LayerMask.NameToLayer("Obstacles");
        collectiblesLayer = LayerMask.NameToLayer("Collectibles");

        playerState = State.Dead;
        velocity = Vector3.zero;
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        UnityEngine.Physics.gravity = new Vector3(0, gravity, 0);
        _rigidbody = GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component not found on Player!");
        }
        LoadModel();
        PlayerReset();
    }

    public void LoadModel()//PlayerModelData modelData)
    {
        GameObject modelData = GameManager.Instance.Characters[PlayerPrefs.GetInt("PlayerDataIndex", 0)];
        //transform.GetChild(0).gameObject = Instantiate(modelData);
        //Destroy(transform.GetChild(0).gameObject);
        //GameObject aux = Instantiate(modelData, transform.GetChild(0));
        PlayerModelData model = GameManager.Instance.playerModels[PlayerPrefs.GetInt("PlayerDataIndex", 0)];
        GameObject aux = Instantiate(model.modelPrefab, this.transform);
        aux.SetActive(true);
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)) && !godMode)
        {
            if (playerState == State.Moving)
            {
                ChangePlayerState(State.Jumping);
            }
            else if (playerState == State.Waiting)
            {
                ChangePlayerState(State.Moving);
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            godMode = !godMode;
            if (godMode) 
            {
                Debug.Log("GodMode On");
            }
        }
    }

    void FixedUpdate()
    {
        transform.position += velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == obstaclesLayer && !godMode)
        {
            ChangePlayerState(State.Dead);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == obstaclesLayer && !godMode)
        {
            ChangePlayerState(State.Dead);
        }
        if (collision.gameObject.layer == collectiblesLayer)
        {
            AddCollectibles(collision.gameObject);
        }
        if (collision.gameObject.layer == blockLayer && playerState == State.Jumping) 
        {
            ChangePlayerState(State.Moving);
            Instantiate(groundParticle, transform.position, Quaternion.identity);
        }
    }

    public void PlayerReset()
    {
        if (startPoint == null)
        {
            GameObject pos = new GameObject();
            int lvl = (int)GameManager.Instance.stageName - 1;
            if (lvl < 0) { lvl = 0; }
            pos.transform.position = GameManager.Instance.StartPos[0];
            startPoint = pos;
        }
        velocity = Vector3.zero;
        _rigidbody.velocity = Vector3.zero;
        //transform.SetPositionAndRotation(startPoint.transform.position, Quaternion.Euler(0, 90, 0));
        transform.position = startPoint.transform.position;
        transform.LookAt(startPoint.transform.position + new Vector3(1f, 0f, 0f));
        this.GetComponent<TrailRenderer>().Clear();
    }

    public void PlayerStart()
    {
        PlayerReset();
        _animator.SetTrigger("Reset");
        ChangePlayerState(State.Moving);
    }

    public void PlayerStop(char c)
    {
        if (c == 's' && !godMode)
        {
            ChangePlayerState(State.Waiting);
        }
        else if (c == 'f')
            ChangePlayerState(State.Waiting);
    }

    public void PlayerAutoJump()
    {
        if (godMode)
        {
            ChangePlayerState(State.Jumping);
        }
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
                _animator.SetTrigger("Dead");
                _animator.SetBool("Jumping", false);
                Instantiate(deathParticle, transform.position, Quaternion.identity);
                velocity = Vector3.zero;
                _rigidbody.velocity = Vector3.zero;
                transform.SetPositionAndRotation(startPoint.transform.position, Quaternion.Euler(0, 90, 0));
                this.GetComponent<TrailRenderer>().Clear();
                WorldManager.Instance.PlayerDeath();
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
            transform.forward = fow;
            velocity = fow * groundSpeed;
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

    public void ResTart()
    {
        Start();
    }
}
