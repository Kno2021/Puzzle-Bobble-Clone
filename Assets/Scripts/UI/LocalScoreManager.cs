using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreValueText;
    [SerializeField] private int _scorePerBall = 10;
    [SerializeField] private int _scorePerFallingBall = 20;

    private GlobalScoreManager _globalScoreManager;
    private int _scoreCounter = 0;
    //private bool _gamePaused;
    private uint _globalScoreReference;

    private void Awake()
    {
        GameController.OnScore += ProcessAndSetScore;
       // PauseManager.OnPause += GamePaused;
    }

    private void Start()
    {
        GameController.OnWin += PassScoreToGlobalManager;
        _globalScoreManager = GlobalScoreManager.Instance;
        _globalScoreReference = _globalScoreManager.GlobalScore;
        _scoreValueText.text = _globalScoreReference.ToString();
    }

    private void OnDisable()
    {
        GameController.OnScore -= ProcessAndSetScore;
        //PauseManager.OnPause -= GamePaused;
        GameController.OnWin -= PassScoreToGlobalManager;
    }

    private void ProcessAndSetScore(int sameTypeBallCount, int fallingBallCount)
    {
        int tempFallscore = 0;
        if (fallingBallCount > 0)
        {
            //limit the number of bubbles, otherwise it gets bigger than int max size;
            if (fallingBallCount > 17) fallingBallCount = 17;
            tempFallscore =_scorePerFallingBall;
            for (int i = 2; i <= fallingBallCount; i++)
            {
                tempFallscore *= 2;
            }
        }

        int totalScore = tempFallscore + sameTypeBallCount * _scorePerBall;
        _scoreCounter += totalScore;
        _scoreValueText.text = (_globalScoreReference + _scoreCounter).ToString();
    }

    //private void GamePaused(bool paused)
    //{
    //    _gamePaused = paused;
    //}

    private void PassScoreToGlobalManager()
    {
        _globalScoreManager.SetScore(_scoreCounter);
    }
}
