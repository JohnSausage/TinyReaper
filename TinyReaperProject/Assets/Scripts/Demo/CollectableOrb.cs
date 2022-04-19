using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectableOrb : MonoBehaviour
{
    public event Action<CollectableOrb> AOnGetCollected;
    
    public void GetCollected()
    {
        AOnGetCollected?.Invoke(this);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetCollected();
        }
    }

    public void DisableOrb()
    {
        GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void EnableOrb()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }
}
