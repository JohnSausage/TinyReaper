using UnityEngine;

namespace MoveStates
{

    [System.Serializable]
    public class MoveStateVars
    {
        public float walkspeed = 2;
        public float runspeed = 5;
        public float fullHopVelocity = 12;
        public float shortHopVelocity = 8;
        public float airHopVelocity = 10;

        public int airJumps = 1;
        public int AirJumpCounter { get; set; }
    }

    [System.Serializable]
    public class MoveState
    {
        protected Character _chr;
        protected int _timer;

        protected MoveStateVars Vars { get => _chr.MoveStateVars; }
        protected MovementInputs Inputs { get => _chr.MovementInputs; }
        protected MovementController Ctr { get => _chr.Controller; }

        public MoveState(Character chr)
        {
            this._chr = chr;
        }

        public virtual void Enter()
        {
            _timer = 0;
        }

        public virtual void Execute()
        {
            _timer++;
        }

        public virtual void Exit()
        {

        }

        protected void Anim(string animName)
        {
            _chr.Animate(animName);
        }

        protected void Next(MoveState nextState)
        {
            _chr.SetNextState(nextState);
        }

        protected void InputX(float inputX)
        {
            _chr.SetInputDirection(new Vector2(inputX, 0));
        }

        protected void CheckForJump()
        {
            if (_chr.MovementInputs.JumpEvent == true)
            {
                Next(_chr.jumpSquat);
            }
        }

        protected void CheckIfAerial()
        {
            if(Ctr.IsGrounded == false)
            {
                Next(_chr.jump);
            }
        }
    }

    public class MS_Idle : MoveState
    {
        public MS_Idle(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("idle");

            Vars.AirJumpCounter = 0;
        }

        public override void Execute()
        {
            base.Execute();

            if (_chr.DirectionalInput.y < -0.5f)
            {
                Next(_chr.duck);
            }

            if (Inputs.Direction.x != 0)
            {
                _chr.AnimDir = _chr.DirectionalInput.x;
                Next(_chr.walk);
            }

            CheckIfAerial();
            CheckForJump();
        }
    }

    public class MS_Duck : MoveState
    {
        public MS_Duck(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("duck");
        }

        public override void Execute()
        {
            base.Execute();

            if (_chr.DirectionalInput.y > -0.5f)
            {
                Next(_chr.idle);
            }

            if (_chr.DirectionalInput.x != 0)
            {
                _chr.AnimDir = _chr.DirectionalInput.x;
                Next(_chr.walk);
            }

            CheckIfAerial();
            CheckForJump();
        }
    }

    public class MS_Walk : MoveState
    {
        public MS_Walk(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("run");
        }

        public override void Execute()
        {
            base.Execute();

            if (_chr.DirectionalInput.x == 0)
            {
                Next(_chr.idle);
            }

            CheckIfAerial();
            CheckForJump();
        }
    }

    public class MS_Run : MoveState
    {
        public MS_Run(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("run");
        }

        public override void Execute()
        {
            base.Execute();

            CheckIfAerial();
            CheckForJump();
        }
    }

    public class MS_Dash : MoveState
    {
        public MS_Dash(Character chr) : base(chr)
        {
        }
    }

    public class MS_Skid : MoveState
    {
        public MS_Skid(Character chr) : base(chr)
        {
        }
    }

    public class MS_JumpSquat : MoveState
    {
        public MS_JumpSquat(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("duck");
        }

        public override void Execute()
        {
            base.Execute();

            Inputs.Direction = Vector2.zero;

            if (_timer > 3)
            {
                Next(_chr.jump);
                if (_chr.MovementInputs.Jump == true)
                {
                    _chr.SetJump(_chr.MoveStateVars.fullHopVelocity);
                }
                else
                {
                    _chr.SetJump(_chr.MoveStateVars.shortHopVelocity);
                }
            }
        }
    }

    public class MS_AirJumpSquat : MoveState
    {
        public MS_AirJumpSquat(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("duck");
        }

        public override void Execute()
        {
            base.Execute();

            Inputs.Direction = Vector2.zero;

            if (_timer > 3)
            {
                Next(_chr.jump);

                _chr.SetJump(_chr.MoveStateVars.airHopVelocity);
            }
        }
    }

    public class MS_Jump : MoveState
    {
        public MS_Jump(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("idle");
        }

        public override void Execute()
        {
            base.Execute();

            if (Inputs.JumpEvent && Vars.AirJumpCounter < Vars.airJumps)
            {
                Vars.AirJumpCounter++;
                Next(_chr.airJumpSquat);
            }

            if (_chr.Controller.IsGrounded == true)
            {
                Next(_chr.land);
            }
        }
    }

    public class MS_Land : MoveState
    {
        public MS_Land(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Vars.AirJumpCounter = 0;

            Anim("duck");
        }

        public override void Execute()
        {
            base.Execute();

            if (_timer > 3)
            {
                Next(_chr.idle);

                if(Inputs.Jump)
                {
                    Next(_chr.jumpSquat);
                }
            }

            Inputs.Direction = Vector2.zero;
        }
    }
}