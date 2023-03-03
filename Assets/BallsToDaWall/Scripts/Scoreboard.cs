using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    [System.Serializable] public struct DisplayInfo { public TextMeshProUGUI textMesh; public Animator animator; }
    [SerializeField] private DisplayInfo _scoreDisplay;
    [SerializeField] private DisplayInfo _multiplierDisplay;

    private void Start()
    {
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
}
