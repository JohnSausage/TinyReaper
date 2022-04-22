using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private float _movespeed = 2f;
    [Space]
    [SerializeField] private LayerMask _transportingLayer;
    [Space]
    [SerializeField] private GameObject _movingPlatform;
    [SerializeField] private Transform _start;
    [SerializeField] private Transform _end;

    private Vector2 _movement;
    private bool _movingToEndDirection;
    private RaycastHit2D[] _results = new RaycastHit2D[5];
    private BoxCollider2D _platformCol;

    private LayerMask _defaultLayerMask;
    private void Start()
    {
        SetPlatformPosition(_start.position);
        _movingToEndDirection = true;
        _platformCol = _movingPlatform.GetComponent<BoxCollider2D>();

        _defaultLayerMask = _transportingLayer;
    }

    private void FixedUpdate()
    {
        Vector2 currentPos = _movingPlatform.transform.position;

        Vector2 nextPos;
        Vector2 currentGoal;

        if (_movingToEndDirection)
        {
            currentGoal = _end.position;
        }
        else
        {
            currentGoal = _start.position;
        }


        Vector2 plannedMovement = (currentGoal - currentPos).normalized * _movespeed / 60f;

        if (Vector2.Distance(currentGoal, currentPos) < _movespeed / 60f)
        {
            nextPos = currentGoal;
            _movingToEndDirection = !_movingToEndDirection;
        }
        else
        {
            nextPos = currentPos + plannedMovement;
        }

        SetPlatformPosition(nextPos);

        TransportObjects();
    }

    private void SetPlatformPosition(Vector2 position)
    {
        _movement = position - (Vector2)_movingPlatform.transform.position;

        _movingPlatform.transform.Translate(_movement);
    }

    private void TransportObjects()
    {
        float distance = 0.2f;

        int check = Physics2D.BoxCastNonAlloc(_platformCol.bounds.center, _platformCol.bounds.size, 0, Vector2.up, _results, distance, _transportingLayer);

        LayerMask temp = _movingPlatform.layer;
        _movingPlatform.layer = 0;

        for (int i = 0; i < check; i++)
        {
            _results[i].transform.GetComponentInParent<ICanBeTransported>()?.Transport(_movement);
        }

        _movingPlatform.layer = temp;
    }

    private void OnDrawGizmos()
    {
        if (_platformCol == null) _platformCol = _movingPlatform.GetComponent<BoxCollider2D>();
        if (_platformCol == null) return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(_start.position, _platformCol.size * _movingPlatform.transform.localScale);
        Gizmos.DrawWireCube(_end.position, _platformCol.size * _movingPlatform.transform.localScale);

        Gizmos.DrawLine(_start.position, _end.position);
    }

    public void ActivateLayer(bool activate = true)
    {
        if (activate)
        {
            _transportingLayer = _defaultLayerMask;
        }
        else
        {
            _transportingLayer = 0;
        }
    }
}

public interface ICanBeTransported
{
    public void Transport(Vector2 movement);
}