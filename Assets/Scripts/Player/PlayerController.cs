using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour, I_Input
{
    [SerializeField] Transform _throwPosition;
    [SerializeField] float _targetRotationSpeed = 10f;
    [SerializeField] float throwForce = 20f;

    //References
    private PlayerInput _inputClass;
    private Ball _currentBall;
    private Transform _board;
    private AudioSource _audioSource;

    private bool _hasShot;
    private bool _canShoot;
    private bool _isPlayerLocked;
    private bool _isGamePaused;
    

    public delegate void OnShoot();
    public event OnShoot onShoot;

    private void Awake()
    {
        _inputClass = GetComponent<PlayerInput>();
        _audioSource = GetComponent<AudioSource>();
        GameController.OnGameOver += LockPlayer;
        GameController.OnWin += LockPlayer;
        PauseManager.OnPause += PausePlayer;
    }
    private void OnDisable()
    {
        GameController.OnGameOver -= LockPlayer;
        GameController.OnWin -= LockPlayer;
        PauseManager.OnPause -= PausePlayer;
    }

    private void Update()
    {
        if (_isPlayerLocked || _isGamePaused) return;
        if (_inputClass.FirePressed && !_hasShot && _canShoot)
        {
            _hasShot = true;
            ShootBall();
        }
        else if (!_inputClass.FirePressed)
        {
            _hasShot = false;
        }
    }

    private void FixedUpdate()
    {
        if (_isPlayerLocked || _isGamePaused) return;
        ShootDirectionProcessing();
    }

    public void Initialize(Transform board, Ball ball)
    {
        _board = board;
        _currentBall = ball;
        _currentBall.transform.position = _throwPosition.position;
    }

    public void SetCurrentBall(Ball ball)
    {
        _currentBall = ball;
        _currentBall.transform.position = _throwPosition.position;
    }

    private void ShootBall()
    {
        if (_isPlayerLocked || _isGamePaused) return;
        if (!_currentBall) return;
        _currentBall.Unlock();
        _currentBall.transform.parent = _board;
        _currentBall.GetComponent<Collider2D>().enabled = true;
        _currentBall.Playable = true;
        _currentBall.SetInitialVelocity(_throwPosition.up * throwForce);
        _currentBall = null;
        onShoot?.Invoke();
        _audioSource.Play();
    }

    void ShootDirectionProcessing()
    {
        var x = _inputClass.RotationKeyValues.x;
        if (x == 0) return;
        _throwPosition.Rotate(new Vector3(0, 0, -x * _targetRotationSpeed * Time.fixedDeltaTime), Space.Self);
        var angles = _throwPosition.rotation.eulerAngles;
        angles.z = angles.z > 180 ? angles.z - 360 : angles.z;
        angles.z = Mathf.Clamp(angles.z, -85, 85);
        _throwPosition.localRotation = Quaternion.Euler(angles);

        //do raycast
    }

    public void CanShoot(bool canShoot)
    {
        _canShoot = canShoot;
    }

    private void LockPlayer()
    {
        _isPlayerLocked = true;
    }

    private void PausePlayer(bool pause)
    {
        _isGamePaused = pause;
    }


    public void SetInputActions(Input_Actions inputActions)
    {
        _inputClass.SetInputActions(inputActions);
    }
}
