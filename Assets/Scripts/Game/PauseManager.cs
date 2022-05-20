using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PauseManager : MonoBehaviour, I_Input
{
    public static PauseManager Instance;
    private Input_Actions _inputAction;
    private bool _isGamePaused = false;
    private bool _isGameFinished;
    public static Action<bool> OnPause;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameController.OnGameOver += UnpauseAndLockIfGameFinished;
        GameController.OnWin += UnpauseAndLockIfGameFinished;
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= UnpauseAndLockIfGameFinished;
        GameController.OnWin -= UnpauseAndLockIfGameFinished;
        DisableControl();
    }

    public void SetPauseMenu(PauseMenuManager pauseMenuManager)
    {
        pauseMenuManager.SetPauseDelegate(Unpause);
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_isGameFinished) return;
        if (context.performed && !_isGamePaused)
        {
            _isGamePaused = true;
            OnPause?.Invoke(_isGamePaused);
            Time.timeScale = 0;

        }
        else if (context.performed && _isGamePaused)
        {
            _isGamePaused = false;
            OnPause?.Invoke(_isGamePaused);
            Time.timeScale = 1;
        }
    }

    public void SetInputActions(Input_Actions inputActions)
    {
        if (_inputAction == null)
        {
            _inputAction = inputActions;
            EnableControl();
        }
    }

    private void EnableControl()
    {
        _inputAction.PauseManager.Pause.performed += Pause_performed;
        _inputAction.Enable();
    }

    private void DisableControl()
    {
        _inputAction.PauseManager.Pause.performed -= Pause_performed;
        _inputAction.Disable();
    }

    private void UnpauseAndLockIfGameFinished()
    {
        if (_isGamePaused)
        {
            _isGamePaused = false;
            OnPause?.Invoke(_isGamePaused);
            Time.timeScale = 1;
        }
        _isGameFinished = true;
    }

    private void Unpause()
    {
        if (_isGamePaused)
        {
            _isGamePaused = false;
            OnPause?.Invoke(_isGamePaused);
            Time.timeScale = 1;
        }
    }

    
}

