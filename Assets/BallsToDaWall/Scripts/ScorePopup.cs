using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    private Alarm _killAlarm;
    [SerializeField] private float _killTime;
    [SerializeField] private TextMeshProUGUI _scoreDisplay;
    [SerializeField] private float _travelDistance;
    Vector3 _startPosition;
    Vector3 _finalPosition;

    public void Start()
    {
        _killAlarm = Alarm.Get(_killTime);
        _killAlarm.onComplete = () => Destroy(this.gameObject);
        _startPosition = transform.position;
        _finalPosition = transform.position;
        _finalPosition.z -= _travelDistance;
    }

    public void Update()
    {
        float size = Mathf.Min(1, _killAlarm.PercentRemaining * 2)/10;
        _scoreDisplay.rectTransform.localScale = new Vector3(size, size, 1);
        transform.position = Vector3.Lerp(_startPosition, _finalPosition, _killAlarm.PercentComplete);
    }

    public void SetScoreDisplay(float score)
    {
        _scoreDisplay.text = $"+{score}";
        transform.localScale *= 1 + (score / 10);
    }
}
