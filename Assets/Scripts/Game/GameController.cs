using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameController : MonoBehaviour
{
    private class LevelArea
    {
        public static int MapRows { get; private set; } = 13;
        public static int MapColumns { get; private set; } = 15; 
    }
    [Header("Game Settings")]
    [SerializeField] NodeObject _nodeObject;
    [SerializeField] GameObject boardObject;
    [SerializeField] Transform _nextBallTransform;
    [SerializeField] private bool _invertLayout = false;
    [Range(1, 20)]
    [SerializeField] private int _gameRows = 6;
    [Range(1, 20)]
    [SerializeField] private int _gameColumns = 7;
    [Range(2, 9)]
    [SerializeField] private int _numberOfElementsForBreak = 3;
    [Range(2, 6)]
    [SerializeField] private int _numberOfColorsInLevel = 6;
    [Range(5, 30)]
    [SerializeField] private float _stepTime = 10f;
    private float _stepCounter;

    [SerializeField] private PlayerController _playerPrefab;

    //references
    private PlayerController _playerReference;
    private GameObject _boardReference;
    private AudioManager _audioManager;
    private Ball _currentBall;
    private Ball _nextBall;

    private const float _cellSize = 1;
    //time before checking if there is a break
    private const float _timeBeforeBreaking = 0.03f;
    private int _ballCounter;

    private bool _hasPlayedWarningSound;
    private bool _shakeEventSent;
    private bool _gamePaused;
    private bool _gameLocked;
    private bool _updatingBoardStep;
    //bool to detect when the grid is updating due to collision
    private bool _updatingGridWhenCollision;

    private Input_Actions _inputActionReference;
    private Node[][] _board;

    private int[] _numberOfElementsPerColorArray;

    private Dictionary<int, bool> RowIndexAndSizeDictionary = new();

    public static event Action OnWin;
    public static event Action OnGameOver;
    public static event Action OnShake;
    public static event Action<int, int> OnScore;

    private int _scoreSameTypeBallCounter;
    private int _scoreFallingBallCounter;


    private void Awake()
    {
        CreateIpuntAction();
        PauseManagerInit();
        ClampGameGridDimensions();
        PopulateLineDictionary();
        DetermineBoardArraySize();
        InstantiateBoard();
        CreateBoard();
        EventSubscription();
    }

    private void Start()
    {
        CreateStartingBubbles();
        InstantiatePlayer();
        StartCoroutine(Initialize());
    }

    private void OnDisable()
    {
        _playerReference.onShoot -= UpdateBubbles;
        Ball.OnUpdate -= UpdatePosition;
        PauseManager.OnPause -= PauseGame;
    }

    private void Update()
    {
        if (_gamePaused || _gameLocked) return;
        BoardState();
    }

    private void BoardState()
    {
        _stepCounter += Time.deltaTime;

        if (_stepCounter >= _stepTime - 4.0f && !_shakeEventSent)
        {
            if (!_updatingGridWhenCollision)
            {
                _shakeEventSent = true;
                OnShake?.Invoke();
            }

        }
        if (_stepCounter >= _stepTime - 2.0f && !_hasPlayedWarningSound)
        {
            _hasPlayedWarningSound = true;
            _audioManager.PlayBoardWarningSound();
        }

        if (_stepCounter >= _stepTime)
        {
            if (_updatingGridWhenCollision) return;

            _stepCounter = 0;
            //shift board down
            if (_gameLocked) return;
            ProcessBoardStep();
            _hasPlayedWarningSound = false;
            _shakeEventSent = false;
        }
    }


    private void ClampGameGridDimensions()
    {
        if (_gameRows > LevelArea.MapRows - 1) 
        {
            _gameRows = LevelArea.MapRows - 1; 
        }
        if (_gameColumns > LevelArea.MapColumns)
        {
            _gameColumns = LevelArea.MapColumns;
        }
    }

    private void EventSubscription()
    {
        PauseManager.OnPause += PauseGame;
    }
    private void CreateIpuntAction()
    {
        _inputActionReference = new Input_Actions();
    }
    private void PauseManagerInit()
    {
        var manager = PauseManager.Instance;
        manager.SetInputActions(_inputActionReference);
    }

    private IEnumerator Initialize()
    {
        _audioManager = AudioManager.Instance;
        _playerReference.onShoot += UpdateBubbles;
        Ball.OnUpdate += UpdatePosition;
        //yield return new WaitForSeconds(_audioManager.TimeBeforeStart());
        yield return new WaitForSeconds(2f);
        _playerReference.CanShoot(true);
    }
    private void InstantiateBoard()
    {
        _boardReference = Instantiate(boardObject);
        _numberOfElementsPerColorArray = new int[_nodeObject.QuantityOfColors];
    }
    private void InstantiatePlayer()
    {
        _playerReference = Instantiate(_playerPrefab);
        _playerReference.Initialize(_boardReference.transform, _currentBall);
        _playerReference.SetInputActions(_inputActionReference);

    }

    private void CreateStartingBubbles()
    {
        int a;
        int rnd1;
        do
        {
            rnd1 = UnityEngine.Random.Range(0, _numberOfElementsPerColorArray.Length);
            a = _numberOfElementsPerColorArray[rnd1];
        } while (a == 0);

        _currentBall = _nodeObject.GetIndexBall(rnd1);
        _currentBall.Initialize(true);

        int b;
        int rnd2;
        do
        {
            rnd2 = UnityEngine.Random.Range(0, _numberOfElementsPerColorArray.Length);
            b = _numberOfElementsPerColorArray[rnd2];
        } while (b == 0);
        _nextBall = _nodeObject.GetIndexBall(rnd2);
        _nextBall.Initialize(true);
        _nextBall.transform.position = _nextBallTransform.position;
    }

    private void ProcessBoardStep()
    {
        _updatingBoardStep = true;
        //shift board 1 unit down
        _boardReference.transform.position += new Vector3(0, -1, 0);
        //check game over
        foreach (var node in _board[_board.Length - 2])
        {
            if (node.NodeType != -1)
            {
                //game over
                OnGameOver?.Invoke();
                _gameLocked = true;
                return;
            }
        }

        //delete las row from node board
        var lastRowIndex = _board.Length - 1;
        Node[][] tempBoard = new Node[lastRowIndex][];
        for (int i = 0; i < lastRowIndex; i++)
        {
            tempBoard[i] = new Node[_board[i].Length];
            for (int j = 0; j < tempBoard[i].Length; j++)
            {
                tempBoard[i][j] = _board[i][j];
            }
        }

        RowIndexAndSizeDictionary.Remove(lastRowIndex);
   
        _board = tempBoard;
        UpdateBoardConnections();
        _updatingBoardStep = false;
        _audioManager.PlayBoardShiftSound();
    }

    private void UpdateBubbles()
    {
        StartCoroutine(UpdatingBubbles());
    }

    IEnumerator UpdatingBubbles()
    {
        _playerReference.CanShoot(false);
        _currentBall = _nextBall;
        yield return new WaitForSeconds(0.15f);
        yield return new WaitWhile(() => _gamePaused == true); 

        int a;
        int rnd;
        do
        {
            rnd = UnityEngine.Random.Range(0, _numberOfElementsPerColorArray.Length);
            a = _numberOfElementsPerColorArray[rnd];
        } while (a == 0);
        
        _nextBall = _nodeObject.GetIndexBall(rnd);
        _nextBall.Initialize(true);
        _nextBall.transform.position = _nextBallTransform.position;
        _playerReference.SetCurrentBall(_currentBall);
        yield break;
    }



    private void PopulateLineDictionary()
    {
        var x = Mathf.FloorToInt(((LevelArea.MapColumns - 1) - (_gameColumns - 1) * _cellSize) / 2) * 2 + _gameColumns;
        var isShortLine = x != LevelArea.MapColumns;
        RowIndexAndSizeDictionary.Add(0, isShortLine);

        for (int i = 1; i < LevelArea.MapRows; i++)
        {
            isShortLine = !isShortLine;
            RowIndexAndSizeDictionary.Add(i, isShortLine);
        }
    }

    void DetermineBoardArraySize()
    {
        _board = new Node[LevelArea.MapRows][];
        for (int i = 0; i < LevelArea.MapRows; i++)
        {
            var numberOfElements = RowIndexAndSizeDictionary[i] ? LevelArea.MapColumns - 1 : LevelArea.MapColumns;
            _board[i] = new Node[numberOfElements];
        }
    }

    private void CreateBoard()
    {
        for (int i = 0; i < _board.Length; i++)
        {
            var length = _board[i].Length;
            var initialNodeOffset = RowIndexAndSizeDictionary[i] ? 0.5f : 0;
            var ballInitPos = ((LevelArea.MapColumns - 1) - (_gameColumns - 1) * _cellSize) / 2;//_cellSize
            var ballInitIndex = Mathf.FloorToInt(ballInitPos);

            for (int j = 0; j < length; j++)
            {
                var index = new NodeIndex
                {
                    i_index = i,
                    j_index = j
                };

                var pos = new Vector3(initialNodeOffset + j, -i, 0);

                _board[i][j] = new Node(index, pos);
                var a = 0;
                var b = 0;

                if (i > 0 && RowIndexAndSizeDictionary[i] && i % 2 != 0)
                {
                    a = 1;
                    if (_invertLayout)
                    {
                        a = 0;
                        b = -1;
                    }
                }
                else if (i > 0 && !RowIndexAndSizeDictionary[i] && i % 2 != 0)
                {
                    b = 1;
                    if (_invertLayout)
                    {
                        b = 0;
                        a = -1;
                    }
                }
                var check = j >= ballInitIndex - a && j <= ballInitIndex + _gameColumns - (1 - b);

                if (i < _gameRows && check) 
                {
                    var ballObject = _nodeObject.GetRandomBall(_numberOfColorsInLevel);
                    ballObject.transform.parent = _boardReference.transform;
                    ballObject.transform.localPosition = pos;
                    ballObject.Initialize(true);
                    _board[i][j].SetNodeType(ballObject);
                    ballObject.SetNodeIndex(_board[i][j].GetNodeIndex());
                    _numberOfElementsPerColorArray[ballObject.BallType]++;
                    _ballCounter++;
                    
                }
            }
        }

        UpdateBoardConnections();
    }

    private void UpdateBoardConnections()
    {
        for (int i = 0; i < _board.Length; i++)
        {
            for (int j = 0; j < _board[i].Length; j++)
            {
                UpdateNodeConnections(_board[i][j]);
            }
        }
    }

    private void UpdateNodeConnections(Node node)
    {
        var neighbors = new List<Node>();
        var index = node.GetNodeIndex();
        
        //check above
        if (index.i_index > 0)
        {
            int leftIndex = index.j_index - (RowIndexAndSizeDictionary[index.i_index] ? 0 : 1);
            int rightIndex = index.j_index + (RowIndexAndSizeDictionary[index.i_index] ? 1 : 0);
            if(leftIndex >= 0) //was greater than. Modified to greater or equal
            {
                neighbors.Add(_board[index.i_index - 1][leftIndex]);
            }
            if (rightIndex < _board[index.i_index - 1].Length)
            {
                neighbors.Add(_board[index.i_index - 1][rightIndex]);
            }
        }

        //check sides 
        //check left
        var isFirstNode = index.j_index == 0;
        var isLastNode = index.j_index == _board[index.i_index].Length - 1;

        if (!isFirstNode)
        {
            neighbors.Add(_board[index.i_index][index.j_index - 1]);
        }

        //check right
        if (!isLastNode)
        {
            neighbors.Add(_board[index.i_index][index.j_index + 1]);
        }

        //check below
        if (index.i_index < _board.Length - 1)
        {
            int leftIndex = index.j_index - (RowIndexAndSizeDictionary[index.i_index] ? 0 : 1);
            int rightIndex = index.j_index + (RowIndexAndSizeDictionary[index.i_index] ? 1 : 0);
            if (leftIndex >= 0)
            {
                neighbors.Add(_board[index.i_index + 1][leftIndex]);
            }
            if (rightIndex < _board[index.i_index + 1].Length)
            {
                neighbors.Add(_board[index.i_index + 1][rightIndex]);
            }
        }
        node.UpdateNodeConnections(neighbors);
    }


    private void UpdatePosition(Vector3 otherBallPos, Ball thisBall)
    {
        _playerReference.CanShoot(false);
        _updatingGridWhenCollision = true;

        //collided node position more a relative unit vector of thisBall locaPosition. More precision.
        var pos = otherBallPos + (thisBall.transform.localPosition - otherBallPos).normalized; 

        var yPos_Index = Mathf.Clamp(-Mathf.RoundToInt(pos.y), 0, _board.Length - 1);
        int xPosIndex;
        float offset;
        int nodeType;
        int maxIterations = 10;
        int counter = 0;

        while (_updatingBoardStep)
        {
            //Debug.Log("updating board detected after ball collision");
            continue;
        }

        do
        {
            offset = RowIndexAndSizeDictionary[yPos_Index] ? 0.0f : 0.5f;
            xPosIndex = Mathf.Clamp(Mathf.FloorToInt(pos.x + offset), 0, RowIndexAndSizeDictionary[yPos_Index] ? 13 : 14); //modify this
            nodeType = _board[yPos_Index][xPosIndex].NodeType;
            counter++;

            if (nodeType != -1)
            {
                //if not empty shift down
                yPos_Index++;
                if (yPos_Index > _board.Length - 1)
                {
                    _gameLocked = true;
                    OnGameOver?.Invoke();
                    return;
                }
            }

        } while (nodeType != -1 && counter <= maxIterations);

        //if the line is short, displace the nodeIndex 0.5f units to get the localPos;
        var offset2 = RowIndexAndSizeDictionary[yPos_Index] ? 0.5f : 0.0f;
        thisBall.transform.localPosition = new Vector3(xPosIndex + offset2, -yPos_Index, 0);

        _board[yPos_Index][xPosIndex].SetNodeType(thisBall);
        thisBall.SetNodeIndex(_board[yPos_Index][xPosIndex].GetNodeIndex());

        _numberOfElementsPerColorArray[thisBall.BallType]++;
        _ballCounter++;
        //check number of elementes of same type
        StartCoroutine(DetectSameTypeAndDestroy(_board[yPos_Index][xPosIndex]));
    }

    private IEnumerator DetectSameTypeAndDestroy(Node node)
    {
        List<Node> nodeList = CheckSameTypeNode(node, new List<Node>());
        
        if (nodeList.Count >= _numberOfElementsForBreak)
        {
            
            Ball[] ballArray = new Ball[nodeList.Count];

            for (int i = 0; i < ballArray.Length; i++)
            {
                ballArray[i] = nodeList[i].ResetNodeType();
            }

            yield return new WaitForSeconds(_timeBeforeBreaking);
            _audioManager.PlayPoppingSounds();

            foreach (var ball in ballArray)
            {
                DestroyBallObject(ball);
            }

            CheckForFallingBalls();
            ProcessScore();
            CheckWinCondition();
            
        }
        else
        {
            //check game over
            foreach (var bubble in _board[_board.Length - 1])
            {
                if (bubble.NodeType != -1)
                {
                    //game over
                    OnGameOver?.Invoke();
                    _gameLocked = true;
                    yield break;
                }
            }
            _playerReference.CanShoot(true);
            _updatingGridWhenCollision = false;
        }

        
    }

    void DestroyBallObject(Ball ball)
    {
        if (_numberOfElementsPerColorArray[ball.BallType] < 0)
            Debug.LogError("THIS SHOULD NOT HAPPEN");

        if (ball.gameObject)
        {
            ball.PopBubble();
            Destroy(ball.gameObject);
            if (_numberOfElementsPerColorArray[ball.BallType] > 0)
                _numberOfElementsPerColorArray[ball.BallType]--;
            _ballCounter--;
            _scoreSameTypeBallCounter++;
        }
    }

    private void CheckWinCondition()
    {
        if (_ballCounter < 1)
        {
            OnWin?.Invoke();
            //Debug.LogWarning("YOU WIN");
            _gameLocked = true;
        }
    }



    private void ProcessScore()
    {
        OnScore?.Invoke(_scoreSameTypeBallCounter, _scoreFallingBallCounter);
        _scoreSameTypeBallCounter = 0;
        _scoreFallingBallCounter = 0;
    }

    private void CheckForFallingBalls()
    {
        //_checkingForFallingBalls = true;
        //stablish every ball, after row [0], as shouldFall = true;
        for (int i = 1; i < _board.Length; i++)
        {
            for (int j = 0; j < _board[i].Length; j++)
            {
                if (_board[i][j].NodeType == -1) continue;
                _board[i][j].Ball.ShouldFall = true;
            }
        }

        //check if balls are connected to a roof ball
        for (int i = 0; i < _board[0].Length; i++)
        {
            if (_board[0][i].NodeType != -1)
            {
                _board[0][i].Ball.ShouldFall = false;
                CheckFallingNeighbors(_board[0][i]);
            }
        }


        //check in board the balls that should fall.
        List<Node> fallingBalls = new();
        for (int i = 1; i < _board.Length; i++)
        {
            for (int j = 0; j < _board[i].Length; j++)
            {
                if (_board[i][j].NodeType != -1 && _board[i][j].Ball.ShouldFall == true)
                    fallingBalls.Add(_board[i][j]);
            }
        }

        //Destroy every ball
        if (fallingBalls.Count > 0)
        {
            _audioManager.PlayFallingSounds();
        }

        foreach (var node in fallingBalls)
        {
            ResetNodeAndReleaseBall(node);
        }

         _playerReference.CanShoot(true);
        _updatingGridWhenCollision = false;
    }

    private void CheckFallingNeighbors(Node node)
    {
        var neighbors = node.GetNeighbors();
        foreach (var neighborNode in neighbors)
        {
            if (neighborNode.NodeType == -1) continue;
            if (neighborNode.Ball.ShouldFall == true)
            {
                neighborNode.Ball.ShouldFall = false;
                CheckFallingNeighbors(neighborNode);
            }
        }
    }

    void ResetNodeAndReleaseBall(Node node)
    {
        //do vfx
        var ball = node.ResetNodeType();

        if (_numberOfElementsPerColorArray[ball.BallType] < 0)
            Debug.LogError("THIS SHOULD NOT HAPPEN");
        if (ball.gameObject != null)
        {
            ball.ReleaseFromGrid();
            if (_numberOfElementsPerColorArray[ball.BallType] > 0)
                _numberOfElementsPerColorArray[ball.BallType]--;
            _ballCounter--;
            _scoreFallingBallCounter++;
        }
    }
    
    //really cool recursion
    private List<Node> CheckSameTypeNode(Node node, List<Node> list)
    {
        var neighbors = node.GetNeighbors();

        foreach (var neighbor in neighbors)
        {
            if (neighbor.NodeType == -1) continue;
            if (neighbor.NodeType != node.NodeType) continue;
            if (neighbor.NodeType == node.NodeType && !list.Contains(neighbor))
            {
                list.Add(neighbor);
                CheckSameTypeNode(neighbor, list);
            }
        }

        return list;
    }

    private void PauseGame(bool pause)
    {
        _gamePaused = pause;
    }
}
