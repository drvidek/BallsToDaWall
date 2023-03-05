using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private static List<Ball> _allBalls = new();
    private static Vector3 _homePosition;
    [SerializeField] private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;
    [SerializeField] private float _redirectForce;
    public float RedirectForce => _redirectForce;
    [SerializeField] private Transform _player;
    [SerializeField] private bool _useCustomGravity;
    [SerializeField] private float _gravity, _shootForce;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private ParticleSystem _psysCatch;
    private float _zDir;

    private bool _wasKinematic;
    private bool _wasReset;
    private bool _hitTarget;

    private UltimateXR.Manipulation.IUxrGrabbable _grabbable;

    private void Awake()
    {
        if (_homePosition == Vector3.zero)
            _homePosition = transform.position;

        if (!_allBalls.Contains(this))
            _allBalls.Add(this);

        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
        GameManager.Singleton.onStateChange += OnStateChange;
        _grabbable = GetComponent<UltimateXR.Manipulation.IUxrGrabbable>();
    }

    private void OnStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.menu:
                EndRound();
                break;
            case GameState.play:
                StartRound();
                break;
            default:
                break;
        }
    }

    private void StartRound()
    {
        ResetBall();
    }

    private void EndRound()
    {
        DisableBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.IsPlaying)
            return;

        _psysCatch.transform.LookAt(transform.position + Vector3.up);

        _trail.emitting = !_rigidbody.isKinematic;

        if (_rigidbody.isKinematic)
        {
            if (Input.GetButtonDown("Shoot Ball"))
            {
                _rigidbody.isKinematic = false;
                _rigidbody.AddForce((Vector3.forward + Vector3.up) * _shootForce, ForceMode.VelocityChange);
                return;
            }

            if (!_wasKinematic)
            {
                if (!_wasReset)
                {
                    _psysCatch.Play(_hitTarget);

                    if (_hitTarget)
                        ScoreKeeper.main.AdjustMultiplier(1);
                }
                _wasKinematic = true;
                _wasReset = false;
                _hitTarget = false;
            }
            return;
        }

        if (transform.position.z < -1)
            ScoreKeeper.main.SetMultiplier(1);

        if (Input.GetButtonDown("Reset Ball"))
        {
            ResetBall();
        }

    }

    private void FixedUpdate()
    {
        if (_rigidbody.isKinematic)
            return;

        _wasKinematic = false;
        float lastZDir = _zDir;
        _zDir = Mathf.Sign(_rigidbody.velocity.z);

        if (lastZDir > _zDir && _zDir * lastZDir != 0)
            DirectBallToPlayer();

        _rigidbody.useGravity = !_useCustomGravity;

        if (_useCustomGravity)
        {
            _rigidbody.AddForce(Vector3.down * _gravity);
        }
    }

    private void ResetBall()
    {
        if (_allBalls.Count > 1)
        {
            RemoveBall();
            return;
        }
        
        transform.position = _homePosition;
        //_rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        _wasReset = true;

        ScoreKeeper.main.SetMultiplier(1);

        _trail.emitting = false;
        _psysCatch.Play();
    }

    private void DisableBall()
    {
        if (_allBalls.Count > 1)
        {
            RemoveBall();
            return;
        }
        _trail.emitting = false;
        _rigidbody.isKinematic = true;
        _grabbable.ReleaseGrabs(false);
        transform.position = Vector3.up * 100f;
    }

    void RemoveBall()
    {
        GameManager.Singleton.onStateChange -= OnStateChange;
        _allBalls.Remove(this);
        Destroy(this.gameObject);
    }

    public void DirectBallToPlayer()
    {
        float power = _rigidbody.velocity.magnitude;
        Vector3 directionToPlayer = MathExt.Direction(transform.position, _player.position) * power * _redirectForce;
        //directionToPlayer.y = Mathf.Max(_correctionForce, directionToPlayer.y);
        _rigidbody.velocity = directionToPlayer;
        //_rigidbody.AddForce(directionToPlayer, ForceMode.Impulse);
    }

    public void HitTarget()
    {
        _hitTarget = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Killplane")
        {
            ResetBall();
        }
    }
}
