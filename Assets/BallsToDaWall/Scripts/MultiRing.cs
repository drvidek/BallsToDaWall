using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiRing : MonoBehaviour
{
    [SerializeField] private int _multiballCount;

    [SerializeField] private AnimationCurve[] _allMovementCurves;
    [SerializeField] private AnimationCurve _currentMovementCurveX, _currentMovementCurveY;
    [SerializeField] private Bounds2D _movementBounds;
    [SerializeField] private float _lifetime;
    [SerializeField] private Alarm _spawningAlarm;

    private float _timePassed;

    public float MoveWidth => _movementBounds.max.x - _movementBounds.min.x;
    public float MoveHeight => _movementBounds.max.y - _movementBounds.min.y;

    private bool _currentlySpawning = false;
    private float _spawningDirection;
    private Vector3 _homePosition;

    private void Awake()
    {
        _homePosition = transform.position;
        transform.localScale = Vector3.zero;
        Hide();
    }

    private void Start()
    {
        GameManager.Singleton.onStateChange += OnStateChange;
        Alarm.SetFromInspector(_spawningAlarm);
        _spawningAlarm.onComplete = StartSpawn;
    }

    private void Hide()
    {
        transform.position = Vector3.up * 50f;
    }

    private void OnStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.menu:
                _spawningAlarm.Stop();
                StartDespawn();
                break;
            case GameState.play:
                _spawningAlarm.ResetAndPlay();
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (!_currentlySpawning)
            return;

        transform.localScale += Vector3.one * Time.deltaTime * _spawningDirection;

        if (transform.localScale.x >= 1 || transform.localScale.x <= 0)
        {
            EndSpawn();
        }
    }

    private void FixedUpdate()
    {
        if (_currentlySpawning || transform.localScale == Vector3.zero)
            return;

        _timePassed += Time.fixedDeltaTime / _lifetime;
        SetPositionFromCurves();

        if (_timePassed >= 1)
        {
            StartDespawn();
        }
    }

    private void StartSpawn()
    {
        _currentlySpawning = true;
        _spawningDirection = 1;
        RollMovementCurves();
        _timePassed = 0;
        SetPositionFromCurves();

    }

    private void RollMovementCurves()
    {
        MathExt.Roll(_allMovementCurves.Length, out int num);
        _currentMovementCurveX = _allMovementCurves[num];

        MathExt.Roll(_allMovementCurves.Length, out num);
        _currentMovementCurveY = _allMovementCurves[num];
    }

    private void SetPositionFromCurves()
    {
        Vector3 position = (Vector3)_movementBounds.min + new Vector3(_currentMovementCurveX.Evaluate(_timePassed) * MoveWidth, _currentMovementCurveY.Evaluate(_timePassed) * MoveHeight);
        position.z = _homePosition.z;
        transform.position = position;
    }

    private void EndSpawn()
    {
        transform.localScale = _spawningDirection > 0 ? Vector3.one : Vector3.zero;
        if (transform.localScale.x == 0)
            Hide();
        _currentlySpawning = false;
    }

    private void StartDespawn()
    {
        _currentlySpawning = true;
        _spawningDirection = -1;
        if (GameManager.IsPlaying)
            _spawningAlarm.ResetAndPlay();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Ball>(out Ball ball))
        {
            //if the ball is returning to the player don't multiply
            if (ball.Rigidbody.velocity.z < 0)
                return;

            Vector3 newPosition = ball.transform.position;
            float radius = ball.transform.localScale.x / 2;
            float angleDifference = 360 / _multiballCount;

            for (int i = 0; i < _multiballCount; i++)
            {
                newPosition += Quaternion.AngleAxis(angleDifference * i, Vector3.forward) * Vector3.up * radius;
                Ball newBall = Instantiate(ball.gameObject, newPosition, Quaternion.identity).GetComponent<Ball>();
                Vector3 newVelocity = ball.Rigidbody.velocity;
                newVelocity.x *= Random.Range(0.7f, 1.3f);
                newVelocity.y *= Random.Range(0.7f, 1.3f);
                newBall.Rigidbody.velocity = newVelocity * Random.Range(0.8f, 1.3f);
            }
        }
    }
}
