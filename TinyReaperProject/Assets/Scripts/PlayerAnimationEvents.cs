using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoveStates;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [Space]

    [SerializeField] private Sound _step;
    [SerializeField] private Sound _land;

    private void Start()
    {
        Character chr = GetComponentInParent<Character>();
        chr.AOnLand += Land;
    }

    private void PlayClip(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    private void Step()
    {
        _step.PlayFrom(_audioSource);
    }

    private void Land(MoveState moveState)
    {
        if(moveState.GetType() == typeof(MS_Land))
        {
            _land.PlayFrom(_audioSource);
        }
    }
}

[System.Serializable]
public class Sound
{
    [SerializeField] 
    private AudioClip _clip;
    
    [SerializeField] 
    [Range(0f, 1f)] 
    private float _volume = 1f;

    public AudioClip Clip { get => _clip; }
    public float Volume { get => _volume; set => _volume = value; }

    public void PlayFrom(AudioSource audioSource)
    {
        audioSource.PlayOneShot(Clip, Volume);
    }
}