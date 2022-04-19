using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;

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

        ClosePausePanel();
    }

    private void OnPauseButton(InputAction.CallbackContext context)
    {
        if(GamePaused == false)
        {
            GamePaused = true;
            OnPause?.Invoke(true);
            Time.timeScale = 0f;

            OpenPausePanel();
        }
        else
        {
            GamePaused = false;
            OnPause?.Invoke(false);
            Time.timeScale = 1f;

            ClosePausePanel();
        }
    }

    private void OpenPausePanel()
    {
        _pausePanel.SetActive(true);
    }

    private void ClosePausePanel()
    {
        _pausePanel.SetActive(false);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
