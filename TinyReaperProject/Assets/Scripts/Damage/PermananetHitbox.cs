using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PermananetHitbox : MonoBehaviour
{
    [SerializeField] private Damage _damage;
    [SerializeField] private ContactFilter2D _hitLayer;

    private Collider2D _col;
    private RaycastHit2D[] _results = new RaycastHit2D[3];
    private RaycastHit2D[] _oldResults = new RaycastHit2D[3];
    private int _oldNrOfHits;

    private void Start()
    {
        _col = GetComponent<Collider2D>();
        _damage.GenerateID();
    }

    private void FixedUpdate()
    {
        _results = new RaycastHit2D[3];
        int nrOfHits = _col.Cast(direction: Vector2.up, results: _results, contactFilter: _hitLayer, distance: 0.1f);

        for (int i = 0; i < nrOfHits; i++)
        {
            if (_oldResults.Contains(_results[i]))
            {
                continue;
            }

            _results[i].transform.GetComponentInParent<ICanTakeDamage>()?.GetHit(_damage);
        }

        if (_oldNrOfHits > 0 && nrOfHits == 0)
        {
            _damage.GenerateID();
        }

        _oldNrOfHits = nrOfHits;
        _oldResults = _results;
    }
}

[System.Serializable]
public class Damage
{
    public float amount;
    public float stagger;
    public Knockback knockback;

    public int DamageID;// { get; set; }
    public void GenerateID()
    {
        DamageID = UnityEngine.Random.Range(0, 1000000);
    }

    public Damage Clone()
    {
        return (Damage)MemberwiseClone();
    }
}

[System.Serializable]
public class Knockback
{
    public Vector2 direction;
    public float magnitude;
}

public interface ICanTakeDamage
{
    Damage CurrentDamage { get; set; }
    void GetHit(Damage damage);
    void TakeDamage(Damage damage);
}