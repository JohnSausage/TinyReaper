using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(CircleCollider2D))]
public class Hurtbox : MonoBehaviour
{
    private CircleCollider2D _col;

    private void Start()
    {
        _col = GetComponent<CircleCollider2D>();
    }

    private void OnDrawGizmos()
    {
        if(_col == null) _col = GetComponent<CircleCollider2D>();
        if (_col == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_col.bounds.center, _col.radius);
    }
}
