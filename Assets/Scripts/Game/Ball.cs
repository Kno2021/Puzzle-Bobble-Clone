using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [SerializeField] private ParticleSystem _popVFX;
    private float _destroyTimeWhenFalling = 1.0f;
    private float _fallingSpeed = 15f;
    private float _timeCounter;

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private bool _isStatic = false;

    private NodeIndex _nodeIndex;
    private int _ballType;
    public bool IsStatic { get { return _isStatic; } }

    private Animator _animator; 

    public delegate void OnUpdatePosition(Vector3 otherPosition, Ball thisBall);
    public static event OnUpdatePosition OnUpdate;
    public int BallType { get { return _ballType; } }
    public bool ShouldFall { get; set; } = false;
    public bool Playable { get; set; } = true;


    public static event Action OnHitSound;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        GameController.OnShake += ShakeAnimation;
        GameController.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        GameController.OnShake -= ShakeAnimation;
        GameController.OnGameOver -= GameOver;
    }

    private void Update()
    {
        if (!_isStatic && ShouldFall)
        {
            _timeCounter += Time.deltaTime;
            if (_timeCounter >= _destroyTimeWhenFalling)
            {
                Destroy(gameObject);
            }
        }
    }
    private void FixedUpdate()
    {
        if(!_isStatic)
            _rb.velocity = _velocity;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isStatic) return;
        if (collision.gameObject.CompareTag("SideWall"))
        {
            _velocity = Bounce();
        }
        if (collision.gameObject.CompareTag("Ball"))
        {
            Lock();
            OnUpdate?.Invoke(collision.transform.localPosition, this);
            CheckForAnimationStatus(collision.gameObject);

        }
        if (collision.gameObject.CompareTag("TopWall"))
        {
            Lock();
            OnUpdate?.Invoke(transform.localPosition, this);
            CheckNeighborBubbleAnimationState();

        }
        OnHitSound?.Invoke();
    }

    private void CheckNeighborBubbleAnimationState()
    {
        var bubbles = transform.parent.GetComponentsInChildren<Ball>();
        Ball tempBubble = null;
        foreach (var bubble in bubbles)
        {
            var condition = bubble.IsStatic && bubble.Playable && (bubble != this);
            if (condition)
            {
                tempBubble = bubble;
                break;
            }
        }

        if (tempBubble != null)
            CheckForAnimationStatus(tempBubble.gameObject);
    }

    private void CheckForAnimationStatus(GameObject ballObject)
    {
        var animator = ballObject.GetComponent<Animator>();

        var clipInfoArray = animator.GetCurrentAnimatorClipInfo(0);

        if (clipInfoArray.Length > 0)
        {
            var hash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            var normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            //add half a frame of correction to sync animation;
            normalizedTime += 0.5f / 60;
            if (normalizedTime > 1)
                normalizedTime -= 0.5f / 60;
            _animator.Play(hash, 0, normalizedTime);
        }
    }

    private Vector2 Bounce()
    {
        var velocity = new Vector2(-_velocity.x, _velocity.y);
        return velocity;
    }

    public void Initialize(bool isStatic)
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (isStatic)
        {
            _rb.bodyType = RigidbodyType2D.Static;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
        _isStatic = isStatic;
    }

    private void Lock()
    {
        _isStatic = true;
        _rb.bodyType = RigidbodyType2D.Static;
    }

    public void Unlock()
    {
        _isStatic = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void SetInitialVelocity(Vector2 initialVelocity)
    {
        _velocity = initialVelocity;
    }

    public void SetNodeIndex(NodeIndex index)
    {
        _nodeIndex = index;
    }

    public void SetBallType(int type)
    {
        _ballType = type;
    }

    public void ReleaseFromGrid()
    {
        Unlock();
        GetComponent<Collider2D>().enabled = false;
        _velocity = _fallingSpeed * Vector2.down;
        _animator.enabled = false;
    }

    private void ShakeAnimation()
    {
        if (!_isStatic || !Playable) return;
        _animator.SetTrigger("Shake");
    }

    public void PopBubble()
    {
        Instantiate(_popVFX, transform.position, Quaternion.identity, transform.parent);
    }

    private void GameOver()
    {
        Lock();
        Playable = false;
        _animator.enabled = false;
    }
}


