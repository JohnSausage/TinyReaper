using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _orbDisplay;
    [SerializeField] private Player _player;
    private Vector3 _playerStartPosition;

    [SerializeField] private List<CollectableOrb> _collectableOrbs;

    private int _collectedOrbs;

    private void Awake()
    {
        _collectableOrbs = new List<CollectableOrb>();
    }

    private void Start()
    {
        _collectableOrbs = FindObjectsOfType<CollectableOrb>().ToList();

        _collectableOrbs.ForEach(x => x.AOnGetCollected += OnOrbCollection);

        _playerStartPosition = _player.transform.position;
    }

    public void ResetDemo()
    {
        _collectedOrbs = 0;
        _orbDisplay.text = _collectedOrbs.ToString();

        _collectableOrbs.ForEach(x => x.EnableOrb());

        _player.transform.position = _playerStartPosition;
    }

    private void OnOrbCollection(CollectableOrb orb)
    {
        _collectedOrbs++;
        _orbDisplay.text = _collectedOrbs.ToString();
        orb.DisableOrb();

    }
}
