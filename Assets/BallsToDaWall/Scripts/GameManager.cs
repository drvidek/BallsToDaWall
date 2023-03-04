using System;
using UnityEngine;
using TMPro;

public enum GameState { menu, play }

public class GameManager : MonoBehaviour
{
    private static GameState _gameState;
    public static GameState gameState => _gameState;
    public static bool IsPlaying => _gameState == GameState.play;
    [System.Serializable] public struct DisplayInfo { public TextMeshProUGUI textMesh; public Animator animator; }
    [SerializeField] private DisplayInfo _scoreDisplay;
    [SerializeField] private DisplayInfo _multiplierDisplay;
    [SerializeField] private DisplayInfo _timerDisplay;
    [SerializeField] private Alarm _roundAlarm;
    [SerializeField] private GameObject[] _lasers;
    [SerializeField] private GameObject _startButton;
    public Action<GameState> onStateChange;

    public bool _startRound;

    #region Singleton + Awake
    private static GameManager _singleton;
    public static GameManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
            {
                _singleton = value;
            }
            else if (_singleton != value)
            {
                Debug.LogWarning("GameManager instance already exists, destroy duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }
    #endregion

    private void Start()
    {
        HighscoreKeeper.LoadEntries();
        UpdateEntryDisplay();

        ScoreKeeper.main.onMultiplierChange += DrawMultiplier;
        ScoreKeeper.main.onScoreChange += DrawScore;
        ResetDrawScoreAndMultiplier();
        _timerDisplay.textMesh.gameObject.SetActive(false);

        Alarm.SetFromInspector(_roundAlarm);
        _roundAlarm.onComplete = EndRound;

        EndRound();
    }

    public void StartRound()
    {
        ScoreKeeper.main.Reset();
        ResetDrawScoreAndMultiplier();
        ChangeGameState(GameState.play);
        _roundAlarm.ResetAndPlay();
        _scoreDisplay.animator.SetTrigger("RoundStart");
        _timerDisplay.textMesh.gameObject.SetActive(true);
        _startButton.SetActive(false);
        foreach (var item in _lasers)
        {
            item.SetActive(false);
        }
    }

    private void Update()
    {
        if (_startRound)
        {
            StartRound();
            _startRound = false;
        }
        switch (gameState)
        {
            case GameState.menu:

                break;
            case GameState.play:
                _timerDisplay.textMesh.text = Mathf.Ceil(_roundAlarm.TimeRemaining).ToString();
                _timerDisplay.animator.SetBool("FinalSeconds",_roundAlarm.TimeRemaining <= 3 && _roundAlarm.TimeRemaining > 1);
                break;
            default:
                break;
        }
    }

    private void EndRound()
    {
        ChangeGameState(GameState.menu);
        _roundAlarm.Stop();
        _scoreDisplay.animator.SetTrigger("RoundEnd");
        _timerDisplay.textMesh.gameObject.SetActive(false);
        _startButton.SetActive(true);
        foreach (var item in _lasers)
        {
            item.SetActive(true);
        }
    }

    private void DrawScore(float dir)
    {
        _scoreDisplay.textMesh.text = "Score:\n" + ScoreKeeper.main.Score.ToString();
    }

    private void DrawMultiplier(float dir)
    {
        _multiplierDisplay.textMesh.text = "Combo\nx" + (ScoreKeeper.main.Multiplier - 1).ToString();
        if (dir != 0)
            _multiplierDisplay.animator.SetTrigger(dir > 0 ? "ComboUp" : "ComboDown");
    }

    private void ResetDrawScoreAndMultiplier()
    {
        DrawScore(0);
        DrawMultiplier(0);
    }

    public void ChangeGameState(GameState state)
    {
        _gameState = state;
        onStateChange?.Invoke(state);
    }

    [System.Serializable] private struct HighScoreDisplay { public TextMeshProUGUI nameList; public TextMeshProUGUI scoreList; }
    [SerializeField] private HighScoreDisplay _highScoreDisplay;
    [SerializeField] private TMP_InputField inputField;
    private int newCount;

    public void AddScore()
    {
        if (HighscoreKeeper.ValidateNewEntry("New" + newCount.ToString(), float.Parse(inputField.text)))
            UpdateEntryDisplay();
        newCount++;
    }

    public void SaveHighScores()
    {
        HighscoreKeeper.SaveEntries();
    }

    public void LoadHighScores()
    {
        HighscoreKeeper.LoadEntries();
        UpdateEntryDisplay();
    }

    private void UpdateEntryDisplay()
    {
        _highScoreDisplay.scoreList.text = HighscoreKeeper.ScoreList;
        _highScoreDisplay.nameList.text = HighscoreKeeper.NameList;
    }
}

