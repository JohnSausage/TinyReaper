using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoveStates;

public class Player : Character
{
    private PlayerInput _playerInput;
    private PlayerInputActions _playerInputActions;

    private bool oldJump;

    protected override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInput>();
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        idle = new MS_Idle(this);
        duck = new MS_Duck(this);
        walk = new MS_Walk(this);
        run = new MS_Run(this);
        dash = new MS_Dash(this);
        skid = new MS_Skid(this);
        jumpSquat = new MS_JumpSquat(this);
        airJumpSquat = new MS_AirJumpSquat(this);
        jump = new MS_Jump(this);
        land = new MS_Land(this);

        currentState = idle;
        currentState.Enter();
        nextState = idle;
    }

    protected override void FixedUpdate()
    {
        GetInputs();

        _movementController.DirectionalInput = _movementInputs.Direction;

        _movementInputs.JumpEvent = false;
        if (_movementInputs.Jump == true && oldJump == false)
        {
            _movementInputs.JumpEvent = true;
        }

        if(currentState != nextState)
        {
            currentState.Exit();
            currentState = nextState;
            currentState.Enter();
        }

        currentState.Execute();

        base.FixedUpdate();

        oldJump = _movementInputs.Jump;
    }

    private void GetInputs()
    {
        _movementInputs.Direction = _playerInputActions.PlayerMovement.Move.ReadValue<Vector2>();
        _movementInputs.Jump = _playerInputActions.PlayerMovement.Jump.inProgress;
    }
}