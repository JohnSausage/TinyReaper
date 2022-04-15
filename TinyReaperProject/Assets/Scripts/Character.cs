using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoveStates;

public class Character : MonoBehaviour
{
    protected MoveState currentState;
    protected MoveState nextState;

    public MS_Idle idle;
    public MS_Duck duck;
    public MS_Walk walk;
    public MS_Run run;
    public MS_Dash dash;
    public MS_Skid skid;
    public MS_JumpSquat jumpSquat;
    public MS_AirJumpSquat airJumpSquat;
    public MS_Jump jump;
    public MS_Land land;

    [SerializeField] protected MoveStateVars _moveStateVars;

    protected Animator _anim;
    protected MovementController _movementController;
    public MovementController Controller { get => _movementController; }

    [SerializeField] protected MovementInputs _movementInputs;

    public Vector2 DirectionalInput { get => _movementInputs.Direction; }

    public float AnimDir
    {
        get => _anim.transform.rotation.y == 0 ? 1 : -1;
        set
        {
            if (value >= 0)
            {
                _anim.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            else
            {
                _anim.transform.rotation = new Quaternion(0, 1, 0, 0);
            }
        }
    }

    public MovementInputs MovementInputs { get => _movementInputs; }
    public MoveStateVars MoveStateVars { get => _moveStateVars; }

    protected virtual void Start()
    {
        _movementInputs = new MovementInputs();
        _movementController = GetComponent<MovementController>();
        _anim = GetComponentInChildren<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        _movementController.UpdatePositionFixed();
    }

    public void Animate(string animName)
    {
        _anim.Play(animName);
    }

    public void SetNextState(MoveState nextState)
    {
        this.nextState = nextState;
    }

    public void SetInputDirection(Vector2 inputDirection)
    {
        _movementController.DirectionalInput = inputDirection;
    }

    public void SetJump(float jumpVelocity)
    {
        _movementController.JumpVelocity = jumpVelocity;
    }
}
