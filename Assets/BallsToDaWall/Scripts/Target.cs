using System.Collections;
using UnityEngine;
using TMPro;

public class Target : MonoBehaviour
{
    [System.Serializable]
    private struct SpawningBounds
    {
        public Vector2 min;
        public Vector2 max;
    }
    [SerializeField] private float _pointWorth;
    [SerializeField] private SpawningBounds _spawningBounds;
    [SerializeField] private Collider _collider;

    private Vector3 _homePosition;

    [SerializeField] private ParticleSystem _psysHit;
    [SerializeField] private GameObject _scorePopup;

    public float SpawnXMin => _homePosition.x + _spawningBounds.min.x;
    public float SpawnXMax => _homePosition.x + _spawningBounds.max.x;
    public float SpawnYMin => _homePosition.y + _spawningBounds.min.y;
    public float SpawnYMax => _homePosition.y + _spawningBounds.max.y;

    private void Start()
    {
        _homePosition = transform.position;
        var main = _psysHit.main;
        main.startColor = GetComponentInChildren<Renderer>().sharedMaterial.GetColor("_EmissionColor");
        Reposition();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Ball>(out Ball ball))
        {
            StartCoroutine(OnHit());
            ball.HitTarget();
        }
    }

    private void Reposition()
    {
        float x = Random.Range(SpawnXMin, SpawnXMax);
        float y = Random.Range(SpawnYMin, SpawnYMax);
        transform.position = new Vector3(x, y, transform.position.z);
    }

    IEnumerator OnHit()
    {
        _psysHit.Play();
        ScoreKeeper.main.AdjustScore(_pointWorth);
        ScorePopup popup = Instantiate(_scorePopup, transform.position, Quaternion.identity).GetComponent<ScorePopup>();
        popup.SetScoreDisplay(_pointWorth * ScoreKeeper.main.Multiplier);
        yield return null;
        Reposition();
    }
}
