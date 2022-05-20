using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeObject")]
public class NodeObject : ScriptableObject
{
    [SerializeField] Ball[] _playingBalls;
    public int QuantityOfColors { get { return _playingBalls.Length; } }

    public Ball GetRandomBall(int maxNumberOfColors)
    {
        int rnd = Random.Range(0, maxNumberOfColors);
        var ball = Instantiate(_playingBalls[rnd]);
        ball.SetBallType(rnd);
        return ball;
    }

    public Ball GetIndexBall(int index)
    {
        var ball = Instantiate(_playingBalls[index]);
        ball.GetComponent<Collider2D>().enabled = false;
        ball.SetBallType(index);
        ball.Playable = false;
        return ball;
    }

 
    
}
