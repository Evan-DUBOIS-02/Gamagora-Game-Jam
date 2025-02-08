using Unity.VisualScripting;
using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAimAndShoot _playerAimAndShoot;
    [SerializeField] private Transform _flareSpawnPoint;

    [Header("Trajectory Line Smoothness/Length")]
    [SerializeField] private int _segmentCount = 50;
    [SerializeField] private float _curveLength = 3.5f;

    private Vector2[] _segments;
    private LineRenderer _lineRenderer;

    private BulletBehavior _bulletBehavior;

    private float _projectileSpeed;
    private float _projectileGravityFromRB;

    private const float TIME_CURVE_ADDITION = 0.5f;

    private void Start()
    {
        //initialize segments
        _segments = new Vector2[_segmentCount];

        //grab line renderer component and set it's number of points
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _segmentCount;

        //grab the projectile speed from the player's bullet behavior
        _bulletBehavior = _playerAimAndShoot.flare.GetComponent<BulletBehavior>();
        _projectileSpeed = _bulletBehavior.physicsFlareSpeed;
        _projectileGravityFromRB = _bulletBehavior.physicsFlareGravity;
    }

    private void Update()
    {
        //set the starting position of the line renderer
        Vector2 startPos = _flareSpawnPoint.position;
        _segments[0] = startPos;
        _lineRenderer.SetPosition(0, startPos);

        //set the starting velocity based on the bullet physics
        Vector2 startVelocity = transform.right * _projectileSpeed;

        for(int i = 1; i < _segmentCount; i++)
        {
            //compute the time offset assuming we're using a RB for physics
            float timeOffset = (i * Time.fixedDeltaTime * _curveLength);

            //compute the gravity offset assuming we're using a RB
            Vector2 gravityOffset = TIME_CURVE_ADDITION * Physics2D.gravity * _projectileGravityFromRB * Mathf.Pow(timeOffset, 2);

            //set the position of the point in the line renderer
            _segments[i] = _segments[0] + startVelocity * timeOffset + gravityOffset;
            _lineRenderer.SetPosition(i, _segments[i]);
        }
    }

}
