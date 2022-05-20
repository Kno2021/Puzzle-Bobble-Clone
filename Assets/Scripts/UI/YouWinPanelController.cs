using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class YouWinPanelController : MonoBehaviour
{
    [SerializeField] private Button _nextStageButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitGameButton;

    private SceneController _sceneController;

    private void Awake()
    {
        _sceneController = SceneController.Instance;
        _nextStageButton.onClick.AddListener(_sceneController.LoadNextScene);
        _mainMenuButton.onClick.AddListener(_sceneController.LoadMainMenu);
        _quitGameButton.onClick.AddListener(_sceneController.QuitGame);
    }
}
