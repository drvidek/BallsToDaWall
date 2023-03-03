using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugInfoBall : MonoBehaviour
{
    private Ball ball;
    [SerializeField] private TextMeshProUGUI tmpRigidbodyStats;

    private void OnValidate()
    {
        ball = GetComponent<Ball>();
        tmpRigidbodyStats.text = CompileDebugString();
    }

    void Awake()
    {
        ball = GetComponent<Ball>();
    }

    // Update is called once per frame
    void Update()
    {
        tmpRigidbodyStats.text = CompileDebugString();
    }

    private string CompileDebugString()
    {
         return
            $"Velocity: {MathExt.ClipToDecimalPlace(ball.Rigidbody.velocity,2)}\n" +
            $"Direction: {MathExt.ClipToDecimalPlace(ball.Rigidbody.velocity.normalized,2)}";
    }
}
