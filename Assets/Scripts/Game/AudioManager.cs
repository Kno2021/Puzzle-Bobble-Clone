using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] Transform _backgroundMusic;
    [SerializeField] AudioSource _gameAudioSource;
    [SerializeField] AudioSource _bubbleAudioSource;
    [SerializeField] AudioSource _boardAudioSource;
    [SerializeField] AudioSource _scoreAudioSource;
    [SerializeField] AudioClip[] _initialSoundClips;
    [SerializeField] AudioClip _bubbleSnapSound;
    [SerializeField] AudioClip _bubblePopSound;
    [SerializeField] AudioClip _bubbleFallingSound;
    [SerializeField] AudioClip _multiPopScoreSound;
    [SerializeField] AudioClip _fallingScoreSound;
    [SerializeField] AudioClip _boardWarningSound;
    [SerializeField] AudioClip _boardShiftSound;
    [SerializeField] AudioClip _clearedLevelSound;
    [SerializeField] AudioClip _youLoseLevelSound;

    private Transform _backgroundMusicReference;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Ball.OnHitSound += PlayBubbleCollisionSound;
        _backgroundMusicReference = Instantiate(_backgroundMusic);
        GameController.OnWin += PlayLevelClearedSounds;
        GameController.OnGameOver += PlayLevelLostSounds;
        StartCoroutine(PlayInitialClips());
    }
    private void OnDisable()
    {
        Ball.OnHitSound -= PlayBubbleCollisionSound;
        GameController.OnWin -= PlayLevelClearedSounds;
        GameController.OnGameOver -= PlayLevelLostSounds;
    }

    private void PlayBubbleCollisionSound()
    {
       // _bubbleAudioSource.PlayOneShot(_bubbleSnapSound);
    }

    public void PlayPoppingSounds()
    {
        //_bubbleAudioSource.PlayOneShot(_bubblePopSound);
        //_scoreAudioSource.PlayOneShot(_multiPopScoreSound);
    }

    public void PlayFallingSounds()
    {
        StartCoroutine(FallingSounds());
    }

    public void PlayBoardWarningSound()
    {
        //_boardAudioSource.PlayOneShot(_boardWarningSound);
    }

    public void PlayBoardShiftSound()
    {
        //_boardAudioSource.PlayOneShot(_boardShiftSound);
    }

    private IEnumerator FallingSounds()
    {
        //_bubbleAudioSource.PlayOneShot(_bubbleFallingSound);
        //yield return new WaitForSeconds(_bubbleFallingSound.length * 0.3f);
        yield return new WaitForSeconds(1f);
        //_scoreAudioSource.PlayOneShot(_fallingScoreSound);
    }

    private void PlayLevelClearedSounds()
    {
        Destroy(_backgroundMusicReference.gameObject);
        //_gameAudioSource.PlayOneShot(_clearedLevelSound);
    }

    private void PlayLevelLostSounds()
    {
        Destroy(_backgroundMusicReference.gameObject);
        //_gameAudioSource.PlayOneShot(_youLoseLevelSound);
    }

    public IEnumerator PlayInitialClips()
    {
        //_gameAudioSource.PlayOneShot(_initialSoundClips[0]);
        //yield return new WaitForSeconds(_initialSoundClips[0].length * 0.7f);
        yield return new WaitForSeconds(1f);
        //_gameAudioSource.PlayOneShot(_initialSoundClips[1]);
    }
    public float TimeBeforeStart()
    {
        //var duration = _initialSoundClips[0].length * 0.75f + _initialSoundClips[1].length * 0.15f;
        var duration = 1f;
        return duration;
    }


}
