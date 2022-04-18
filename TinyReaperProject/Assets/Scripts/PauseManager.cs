using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private Player _player;
    private MenuActions _menuActions;

    public bool GamePaused { get; private set; }

    public static event Action<bool> OnPause;

    private void Start()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<Player>();
        }

        _menuActions = new MenuActions();
        _menuActions.Enable();

        _menuActions.MenuNavigation.Pause.performed += OnPauseButton;
    }

    private void OnPauseButton(InputAction.CallbackContext context)
    {
        if(GamePaused == false)
        {
            GamePaused = true;
            OnPause?.Invoke(true);
        }
        else
        {
            GamePaused = false;
            OnPause?.Invoke(false);
        }
    }
}
