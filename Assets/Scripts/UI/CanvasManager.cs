using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private RectTransform _pauseMenu;
    [SerializeField] private RectTransform _youWinPanel;
    [SerializeField] private RectTransform _gameOverPanel;
    [SerializeField] private RectTransform _startPanel;
    private string _readyText = "READY!!!";
    private string _goText = "GO!!!";

    private void Start()
    {
        PauseManager.OnPause += ActivatePauseMenu;
        GameController.OnWin += EnableWinPanel;
        GameController.OnGameOver += EnableGameOverPanel;
        StartCoroutine(StartPanel());
    }

    private void OnDisable()
    {
        PauseManager.OnPause -= ActivatePauseMenu;
        GameController.OnWin -= EnableWinPanel;
        GameController.OnGameOver -= EnableGameOverPanel;
    }

    private IEnumerator StartPanel()
    {
        _startPanel.gameObject.SetActive(true);
        var text = _startPanel.GetComponentInChildren<TextMeshProUGUI>();
        text.text = _readyText;
        yield return new WaitForSeconds(1.5f);
        text.text = _goText;
        yield return new WaitForSeconds(0.5f);

        var canvasGroup = _startPanel.GetComponent<CanvasGroup>();
        for (float i = 0; i < 0.5f; i+=Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, i / 0.5f);
            yield return null;
        }

        canvasGroup.alpha = 0;
        _startPanel.gameObject.SetActive(false);
    }

    private void EnableWinPanel()
    {
        _youWinPanel.gameObject.SetActive(true);
    }

    private void EnableGameOverPanel()
    {
        _gameOverPanel.gameObject.SetActive(true);
    }

    private void ActivatePauseMenu(bool pause)
    {
        _pauseMenu.gameObject.SetActive(pause);
    }
}
