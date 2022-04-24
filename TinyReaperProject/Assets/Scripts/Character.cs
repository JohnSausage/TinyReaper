using System;
using System.Collections.Generic;
using UnityEngine;
using MoveStates;

public class Character : MonoBehaviour, ICanTakeDamage, ICanBeTransported
{
    protected MoveState currentState;
    protected MoveState nextState;
    public MoveState LastState { get; protected set; }


    [SerializeField] protected MoveStateVars _moveStateVars;

    protected Animator _anim;
    protected MovementController _movementController;
    public MovementController Controller { get => _movementController; }

    [SerializeField] protected MovementInputs _movementInputs;

    public Vector2 DirectionalInput { get => _movementInputs.Direction; }


    //Events

    public event Action<MoveState> AOnLand;
    public event Action<Damage> AGetHit;
    public event Action<Damage> ATakeDamage;


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
    public Damage CurrentDamage { get; set; }

    protected virtual void Start()
    {
        _movementInputs = new MovementInputs();
        _movementController = GetComponent<MovementController>();
        _anim = GetComponentInChildren<Animator>();
        CurrentDamage = new Damage();
    }

    protected virtual void FixedUpdate()
    {
        _movementController.UpdatePositionFixed();
    }

    public void Animate(string animName)
    {
        _anim.Play(animName);
    }

    public bool AnimOver()
    {
        return (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f);
    }

    public void SetNextState(MoveState nextState)
    {
        this.nextState = nextState;
    }

    public void SetInputVelocity(Vector2 inputDirection)
    {
        _movementController.InputVelocity = inputDirection;
    }

    public void SetJumpVelocity(float jumpVelocity)
    {
        _movementController.JumpVelocity = jumpVelocity;
    }

    public void TriggerOnEnterStateEvents(MoveState moveState)
    {
        AOnLand?.Invoke(moveState);
    }

    public void GetHit(Damage damage)
    {
        if (CurrentDamage.DamageID == damage.DamageID) return;

        AGetHit?.Invoke(damage);

        TakeDamage(damage);
    }

    public void TakeDamage(Damage damage)
    {
        CurrentDamage = damage.Clone();
        ATakeDamage?.Invoke(CurrentDamage);
    }

    public void Transport(Vector2 movement)
    {
        _movementController.TransportOnPlatform(movement);
    }

    public virtual void Attack(Vector2 direction)
    {

    }
}
