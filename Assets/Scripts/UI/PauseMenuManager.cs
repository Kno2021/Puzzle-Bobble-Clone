using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitGameButton;

    UnityAction _pauseDelegate;

    private PauseManager _pauseManager;
    private SceneController _sceneController;
    private void Awake()
    {
        _sceneController = SceneController.Instance;
        _pauseManager = PauseManager.Instance;
        _pauseManager.SetPauseMenu(this);
    }

    private void Start()
    {
        _resumeButton.onClick.AddListener(_pauseDelegate);
        //_restartButton.onClick.AddListener(_pauseDelegate);
        _restartButton.onClick.AddListener(_sceneController.ReloadScene);
        _quitGameButton.onClick.AddListener(_sceneController.QuitGame);
        _mainMenuButton.onClick.AddListener(_sceneController.LoadMainMenu);
    }

    public void SetPauseDelegate(UnityAction action)
    {
        _pauseDelegate = action;
    }
}

