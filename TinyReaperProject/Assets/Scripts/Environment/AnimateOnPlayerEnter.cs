using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnPlayerEnter : MonoBehaviour
{
    [SerializeField] private LayerMask _triggerLayer;
    [SerializeField] private string _animName;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == _triggerLayer)
        {
            _anim.Play(_animName);
        }
    }
}
