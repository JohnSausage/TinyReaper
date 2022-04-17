using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoveStates;

public class Player : Character
{
    [SerializeField] private string _stateName;

    private PlayerInput _playerInput;
    private PlayerInputActions _playerInputActions;

    private bool oldJump;
    private Vector2 oldDirection;

    protected override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInput>();
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        MoveStateVars.InitStates(this);

        currentState = MoveStateVars.Idle;
        currentState.Enter();
        nextState = MoveStateVars.Idle;
    }

    protected override void FixedUpdate()
    {
        GetInputs();


        if (currentState != nextState)
        {
            currentState.Exit();
            LastState = currentState;
            currentState = nextState;
            currentState.Enter();

            _stateName = currentState.ToString();

            Debug.Log(_stateName);
        }

        currentState.Execute();

        base.FixedUpdate();

    }

    private void GetInputs()
    {
        _movementInputs.Direction = _playerInputActions.PlayerMovement.Move.ReadValue<Vector2>();
        _movementInputs.Jump = _playerInputActions.PlayerMovement.Jump.inProgress;
        _movementInputs.Shield = _playerInputActions.PlayerMovement.Shield.inProgress;

        _movementInputs.JumpEvent = false;

        if (_movementInputs.Jump == true && oldJump == false)
        {
            _movementInputs.JumpEvent = true;
        }

        Vector2 direction = _movementInputs.Direction;
        Vector2 strongDirection = Vector2.zero;

        if ((Mathf.Abs(direction.x - oldDirection.x) > 0.25f) && Mathf.Abs(direction.x) > 0.8f)
        {
            strongDirection.x = Mathf.Sign(direction.x);
        }

        if ((Mathf.Abs(direction.y - oldDirection.y) > 0.25f) && Mathf.Abs(direction.y) > 0.8f)
        {
            strongDirection.y = Mathf.Sign(direction.y);
        }

        _movementInputs.StrongDirection = strongDirection;

        oldJump = _movementInputs.Jump;
        oldDirection = _movementInputs.Direction;
    }
}