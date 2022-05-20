using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _quitGameButton;
    private SceneController _sceneController;

    private void Start()
    {
        _sceneController = SceneController.Instance;
        _startGameButton.onClick.AddListener(_sceneController.LoadNextScene);
        _quitGameButton.onClick.AddListener(_sceneController.QuitGame);
    }
}
