using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoveStates;

public class Player : Character
{
    [SerializeField] private string _stateName;

    private PlayerInput _playerInput;

    private bool oldJump;
    private bool oldAttack;
    private Vector2 oldDirection;

    private MoveState jab;
    private MoveState nair;

    protected override void Start()
    {
        base.Start();

        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions.Enable();

        MoveStateVars.InitStates(this);

        currentState = MoveStateVars.Idle;
        currentState.Enter();
        nextState = MoveStateVars.Idle;

        PauseManager.OnPause += OnPause;


        jab = new AS_PlayerJab(this);
        nair = new AS_PlayerNair(this);
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

            //Debug.Log(_stateName);
        }

        currentState.Execute();

        base.FixedUpdate();

    }

    private void GetInputs()
    {
        _movementInputs.Direction = _playerInput.actions["Move"].ReadValue<Vector2>();
        _movementInputs.Jump = _playerInput.actions["Jump"].inProgress;
        _movementInputs.Shield = _playerInput.actions["Shield"].inProgress;
        _movementInputs.Attack = _playerInput.actions["Attack"].inProgress;

        _movementInputs.JumpEvent = false;

        if (_movementInputs.Jump == true && oldJump == false)
        {
            _movementInputs.JumpEvent = true;
        }

        _movementInputs.AttackEvent = false;

        if (_movementInputs.Attack == true && oldAttack == false)
        {
            _movementInputs.AttackEvent = true;
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
        oldAttack = _movementInputs.Attack;
        oldDirection = _movementInputs.Direction;
    }

    private void OnPause(bool pause)
    {
        if(pause)
        {
            _playerInput.actions.Disable();
            _playerInput.enabled = false;
        }
        else
        {
            _playerInput.enabled = true;
            _playerInput.actions.Enable();
        }
    }

    public override void Attack(Vector2 direction)
    {
        base.Attack(direction);

        if (_movementController.IsGrounded)
        {
            SetNextState(jab);
        }
        else
        {
            SetNextState(nair);
        }
    }
}