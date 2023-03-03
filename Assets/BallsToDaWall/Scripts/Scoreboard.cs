using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    [System.Serializable] public struct DisplayInfo { public TextMeshProUGUI textMesh; public Animator animator; }
    [SerializeField] private DisplayInfo _scoreDisplay;
    [SerializeField] private DisplayInfo _multiplierDisplay;

    private void Start()
    {
        HighscoreKeeper.LoadEntries();
        UpdateEntryDisplay();

        ScoreKeeper.main.onMultiplierChange += DrawMultiplier;
        ScoreKeeper.main.onScoreChange += DrawScore;
        DrawScore(0);
        DrawMultiplier(0);
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

