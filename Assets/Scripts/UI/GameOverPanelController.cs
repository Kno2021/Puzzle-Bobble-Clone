using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitGameButton;

    private SceneController _sceneController;

    private void Awake()
    {
        _sceneController = SceneController.Instance;
        _restartButton.onClick.AddListener(_sceneController.ReloadScene);
        _quitGameButton.onClick.AddListener(_sceneController.QuitGame);
        _mainMenuButton.onClick.AddListener(_sceneController.LoadMainMenu);
    }
}
