using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxScaleX = 0.1f;
    [SerializeField] private float parallaxScaleY = 0.1f;

    private Transform _parallaxCamera;
    private Vector2 _startPos;
    private Vector2 _currentCameraPos;
    private Vector2 _localStartPos;

    void Start()
    {
        _parallaxCamera = Camera.main.transform;

        _startPos = GetComponentInChildren<Renderer>().bounds.center;
        _localStartPos = transform.localPosition;
    }

    void FixedUpdate()
    {
        _currentCameraPos = (Vector2)_parallaxCamera.position - _startPos;

        transform.localPosition = new Vector3(_currentCameraPos.x * parallaxScaleX, _currentCameraPos.y * parallaxScaleY, transform.localPosition.z);
        transform.localPosition += (Vector3)_localStartPos;
    }
}
