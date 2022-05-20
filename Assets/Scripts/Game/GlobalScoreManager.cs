using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalScoreManager : MonoBehaviour
{
    public static GlobalScoreManager Instance;

    public uint GlobalScore { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetScore(int score)
    {
        GlobalScore += (uint)score;
    }

    public void ResetScore()
    {
        GlobalScore = 0;
    }

}
