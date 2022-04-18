using UnityEngine;

namespace MoveStates
{

    [System.Serializable]
    public class MoveStateVars
    {
        public float walkspeed = 8;
        public float runspeed = 15;
        public float fullHopVelocity = 12;
        public float shortHopVelocity = 8;
        public float airHopVelocity = 10;
        public float maxAirspeed = 20;
        public float aerialAccel = 2;
        public float aerialDeaccel = 1;

        [Space]

        public float dashSpeed = 20;
        public int dashTimeF = 5;
        public int skidTimeF = 3;
        public int airJumps = 1;
        public float rollSpeed = 8;
        public int airDodgeTimeF = 30;
        public float airDodgeSpeed = 20;
        public bool airDodged;
        public float wallJumpVelocity = 20;
        public int wallJumpTimeF = 12;

        [Space]

        public int fallThroughPlatformTimerF = 15;
        public int FallThroughPlatformTimer { get; set; }

        public int fastFallRequestTimeF = 15;
        public int FastFallRequestTimer { get; set; }

        public int AirJumpCounter { get; set; }
        public float DashDirection { get; set; }
        public float SkidDirection { get; set; }
        public float JumpSquatDirection { get; set; }
        public float RollDirection { get; set; }
        public float WallJumpDirection { get; set; }

        [Space]

        //States
        [SerializeField] private MS_Idle idle;
        [SerializeField] private MS_Duck duck;
        [SerializeField] private MS_Walk walk;
        [SerializeField] private MS_Run run;
        [SerializeField] private MS_Dash dash;
        [SerializeField] private MS_Skid skid;
        [SerializeField] private MS_JumpSquat jumpSquat;
        [SerializeField] private MS_AirJumpSquat airJumpSquat;
        [SerializeField] private MS_Jump jump;
        [SerializeField] private MS_Land land;
        [SerializeField] private MS_Shield shield;
        [SerializeField] private MS_Roll roll;
        [SerializeField] private MS_AirDodge airDodge;
        [SerializeField] private MS_OnWall onWall;
        [SerializeField] private MS_WallJumpSquat wallJumpSquat;

        public MS_Idle Idle { get => idle; }
        public MS_Duck Duck { get => duck; }
        public MS_Walk Walk { get => walk; }
        public MS_Run Run { get => run; }
        public MS_Dash Dash { get => dash; }
        public MS_Skid Skid { get => skid; }
        public MS_JumpSquat JumpSquat { get => jumpSquat; }
        public MS_AirJumpSquat AirJumpSquat { get => airJumpSquat; }
        public MS_Jump Jump { get => jump; }
        public MS_Land Land { get => land; }
        public MS_Shield Shield { get => shield; }
        public MS_Roll Roll { get => roll; }
        public MS_AirDodge AirDodge { get => airDodge; }
        public MS_OnWall OnWall { get => onWall; }
        public MS_WallJumpSquat WallJumpSquat { get => wallJumpSquat; }

        public void InitStates(Character chr)
        {
            idle = new MS_Idle(chr);
            duck = new MS_Duck(chr);
            walk = new MS_Walk(chr);
            run = new MS_Run(chr);
            dash = new MS_Dash(chr);
            skid = new MS_Skid(chr);
            jumpSquat = new MS_JumpSquat(chr);
            airJumpSquat = new MS_AirJumpSquat(chr);
            jump = new MS_Jump(chr);
            land = new MS_Land(chr);
            shield = new MS_Shield(chr);
            roll = new MS_Roll(chr);
            airDodge = new MS_AirDodge(chr);
            onWall = new MS_OnWall(chr);
            wallJumpSquat = new MS_WallJumpSquat(chr);
        }
    }

    [System.Serializable]
    public class MoveState
    {
        protected Character _chr;
        protected int _timerF;

        protected MoveStateVars Vars { get => _chr.MoveStateVars; }
        protected MovementInputs Inputs { get => _chr.MovementInputs; }
        protected MovementController Ctr { get => _chr.Controller; }

        public MoveState(Character chr)
        {
            this._chr = chr;
        }

        public virtual void Enter()
        {
            _timerF = 0;
        }

        public virtual void Execute()
        {
            _timerF++;
            if (Vars.FallThroughPlatformTimer > 0)
            {
                Vars.FallThroughPlatformTimer--;
            }
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

        protected void SetVelocityX(float inputX)
        {
            _chr.SetInputVelocity(new Vector2(inputX, 0));
        }

        protected void CheckForJump()
        {
            if (_chr.MovementInputs.JumpEvent == true)
            {
                Next(Vars.JumpSquat);
            }
        }

        protected void CheckIfAerial()
        {
            if (Ctr.IsGrounded == false)
            {
                Next(Vars.Jump);
            }
        }

        protected void CheckIfLanding()
        {
            if (_chr.Controller.IsGrounded == true)
            {
                Next(Vars.Land);
            }
        }

        protected void CheckForDash()
        {
            if (Inputs.StrongDirection.x != 0)
            {
                Vars.DashDirection = Inputs.StrongDirection.x;
                Next(Vars.Dash);
                _chr.AnimDir = Vars.DashDirection;
            }
        }

        protected void CheckFallThroughPlatform()
        {
            if (Ctr.IsGrounded)
            {
                if (Inputs.StrongDirection.y < -0.90f)
                {
                    Vars.FallThroughPlatformTimer = Vars.fallThroughPlatformTimerF;
                }
            }
            else
            {
                if (Inputs.Direction.y < -0.90f)
                {
                    Vars.FallThroughPlatformTimer = Vars.fallThroughPlatformTimerF;
                }
            }

            if (Inputs.Direction.y >= 0f)
            {
                Vars.FallThroughPlatformTimer = 0;
            }

            if (Vars.FallThroughPlatformTimer > 0)
            {
                Vars.FallThroughPlatformTimer--;
                Ctr.FallThroughPlatforms = true;
            }
            else
            {
                Ctr.FallThroughPlatforms = false;
            }
        }

        protected void CheckForFastFallRequest()
        {
            if (Inputs.StrongDirection.y < -0.9f)
            {
                Vars.FastFallRequestTimer = Vars.fastFallRequestTimeF;
            }

            if (Vars.FastFallRequestTimer > 0)
            {
                Vars.FastFallRequestTimer--;
                Ctr.fastFall = true;
            }
            else
            {
                Ctr.fastFall = false;
            }
        }

        protected bool AnimOver()
        {
            return _chr.AnimOver();
        }

        protected void CheckForShield()
        {
            if (Inputs.Shield)
            {
                Next(Vars.Shield);
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

            Vars.airDodged = false;
            Vars.AirJumpCounter = 0;
            Vars.SkidDirection = 0;
            Vars.FallThroughPlatformTimer = 0;
            Vars.FastFallRequestTimer = 0;
        }

        public override void Execute()
        {
            base.Execute();

            SetVelocityX(0f);

            if (_timerF > 3)
            {
                if (_chr.DirectionalInput.y < -0.5f)
                {
                    Next(Vars.Duck);
                }

                if (Mathf.Abs(Inputs.Direction.x) > 0.45f)
                {
                    _chr.AnimDir = _chr.DirectionalInput.x;
                    Next(Vars.Walk);
                }

                CheckForDash();
            }

            CheckForShield();
            CheckFallThroughPlatform();
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

            Vars.SkidDirection = 0;
        }

        public override void Execute()
        {
            base.Execute();

            if (_chr.DirectionalInput.y > -0.5f)
            {
                Next(Vars.Idle);
            }

            if (Mathf.Abs(Inputs.Direction.x) > 0.45f)
            {
                _chr.AnimDir = _chr.DirectionalInput.x;
                Next(Vars.Walk);
            }

            CheckForShield();
            CheckForDash();
            CheckFallThroughPlatform();
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

            Vars.SkidDirection = 0;
        }

        public override void Execute()
        {
            base.Execute();

            SetVelocityX(Inputs.Direction.x * Vars.walkspeed);
            _chr.AnimDir = Inputs.Direction.x;

            if (Mathf.Abs(Inputs.Direction.x) < 0.25f)
            {
                Next(Vars.Idle);
            }

            if (Mathf.Abs(Inputs.Direction.x) > 0.75f)
            {
                Next(Vars.Run);
            }

            CheckForShield();
            CheckFallThroughPlatform();
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

            SetVelocityX(Inputs.Direction.x * Vars.runspeed);

            if (Mathf.Abs(Inputs.Direction.x) < 0.25f)
            {
                Next(Vars.Skid);
            }

            if (Mathf.Sign(Inputs.Direction.x) != Mathf.Sign(_chr.AnimDir) && Mathf.Abs(Inputs.Direction.x) > 0.5f)
            {
                Next(Vars.Skid);
            }

            CheckForShield();
            CheckFallThroughPlatform();
            CheckIfAerial();
            CheckForJump();
        }
    }

    public class MS_Dash : MoveState
    {
        public MS_Dash(Character chr) : base(chr)
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

            if (_timerF < 3)
            {
                SetVelocityX(0);
            }
            else
            {
                SetVelocityX(Mathf.Sign(Vars.DashDirection) * Vars.dashSpeed);
            }

            if (_timerF > Vars.dashTimeF)
            {
                if (Mathf.Abs(Inputs.Direction.x) < 0.25f)
                {
                    Next(Vars.Skid);
                }
                else
                {
                    Next(Vars.Run);
                }
            }

            CheckIfAerial();
            CheckForJump();
        }
    }

    public class MS_Skid : MoveState
    {
        private bool _directionChanged;
        public MS_Skid(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Vars.SkidDirection = _chr.AnimDir;
            _directionChanged = false;
        }

        public override void Execute()
        {
            base.Execute();

            SetVelocityX(Mathf.Sign(Vars.SkidDirection) * Vars.walkspeed);

            if (_directionChanged == false)
            {
                if (Mathf.Sign(Inputs.Direction.x) != Mathf.Sign(Vars.SkidDirection) && Mathf.Abs(Inputs.Direction.x) > 0.5f)
                {
                    _chr.AnimDir = -_chr.AnimDir;
                    _directionChanged = true;
                }
            }

            if (_timerF > Vars.skidTimeF)
            {
                Next(Vars.Idle);
            }

            CheckFallThroughPlatform();
            CheckIfAerial();
            CheckForJump();
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

            Vars.JumpSquatDirection = Inputs.Direction.x;

            if (_chr.LastState == Vars.Skid)
            {
                Vars.JumpSquatDirection = Vars.SkidDirection * 0.25f;
            }

            if (_chr.LastState == Vars.Run)
            {
                Vars.JumpSquatDirection = _chr.AnimDir;
            }
        }

        public override void Execute()
        {
            base.Execute();

            SetVelocityX(Vars.JumpSquatDirection * Vars.walkspeed);

            if (_timerF > 3)
            {
                Next(Vars.Jump);
                if (_chr.MovementInputs.Jump == true)
                {
                    SetVelocityX(Vars.JumpSquatDirection * Vars.runspeed);
                    _chr.SetJumpVelocity(_chr.MoveStateVars.fullHopVelocity);
                }
                else
                {
                    SetVelocityX(Vars.JumpSquatDirection * Vars.runspeed);
                    _chr.SetJumpVelocity(_chr.MoveStateVars.shortHopVelocity);
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

            Vars.JumpSquatDirection = Inputs.Direction.x;
        }

        public override void Execute()
        {
            base.Execute();

            Inputs.Direction = Vector2.zero;

            if (_timerF > 3)
            {
                Next(Vars.Jump);

                _chr.SetJumpVelocity(_chr.MoveStateVars.airHopVelocity);
            }
        }
    }

    public class MS_Jump : MoveState
    {
        private int _onWallTimer;
        private int _onWallTimeF = 5;

        public MS_Jump(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _onWallTimer = 0;
        }

        public override void Execute()
        {
            base.Execute();

            if (Ctr.OutputVelocity.y > 0f)
            {
                Anim("jumpUp");
            }
            else
            {
                Anim("jumpDown");
            }

            if (_timerF < 3 && (_chr.LastState == Vars.JumpSquat))
            {
                if (Mathf.Abs(Inputs.Direction.x) <= 0.5f)
                {
                    SetVelocityX(Vars.JumpSquatDirection * Vars.maxAirspeed / 2f);
                }
                else
                {
                    SetVelocityX(Inputs.Direction.x * Vars.maxAirspeed / 1.5f);
                }
            }
            else if (_timerF < 3 && _chr.LastState == Vars.AirJumpSquat)
            {
                if (Mathf.Abs(Inputs.Direction.x) <= 0.5f)
                {
                    SetVelocityX(0);
                }
                else
                {
                    SetVelocityX(Inputs.Direction.x * Vars.maxAirspeed / 2f);
                }
            }
            else
            {
                float nextVelocity = Ctr.OutputVelocity.x;
                float inputX = Inputs.Direction.x;
                if (Mathf.Abs(inputX) < 0.25f)
                {
                    inputX = 0;
                }

                if (inputX == 0)
                {
                    if (Mathf.Abs(nextVelocity) <= Vars.aerialDeaccel)
                    {
                        nextVelocity = 0f;
                    }
                    else
                    {
                        nextVelocity -= (Vars.aerialDeaccel * Mathf.Sign(nextVelocity));
                    }
                }
                else
                {
                    nextVelocity += inputX * Vars.aerialAccel;
                }

                nextVelocity = Mathf.Clamp(nextVelocity, -Vars.maxAirspeed, Vars.maxAirspeed);

                SetVelocityX(nextVelocity);


                if(_chr.LastState == Vars.WallJumpSquat && _timerF < Vars.wallJumpTimeF)
                {
                    SetVelocityX(Vars.WallJumpDirection * Vars.maxAirspeed / 1.5f);
                }

                if(Inputs.JumpEvent && Ctr.OnWall)
                {
                    _chr.AnimDir = -Ctr.WallDirection;
                    Vars.WallJumpDirection = -Ctr.WallDirection;
                    Next(Vars.WallJumpSquat);
                }
                else if (Inputs.JumpEvent && Vars.AirJumpCounter < Vars.airJumps)
                {
                    Vars.AirJumpCounter++;
                    Next(Vars.AirJumpSquat);
                }
            }

            if (Ctr.OnWall == true)
            {
                _onWallTimer++;
            }
            else
            {
                _onWallTimer = 0;
            }

            if (_onWallTimer > _onWallTimeF && Ctr.OutputVelocity.y < 0)
            {
                Next(Vars.OnWall);
            }

            CheckForFastFallRequest();

            if (Inputs.Shield && Vars.airDodged == false)
            {
                Next(Vars.AirDodge);
            }

            CheckFallThroughPlatform();
            CheckIfLanding();
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
            Vars.SkidDirection = 0;
            Vars.airDodged = false;
            Vars.FallThroughPlatformTimer = 0;
            Vars.FastFallRequestTimer = 0;

            Anim("duck");
        }

        public override void Execute()
        {
            base.Execute();

            if (_timerF > 3)
            {
                if (Mathf.Abs(Inputs.Direction.x) <= 0.25f)
                {
                    Next(Vars.Idle);
                }
                else if (Mathf.Abs(Inputs.Direction.x) <= 0.75f)
                {
                    Next(Vars.Walk);
                    _chr.AnimDir = Inputs.Direction.x;
                }
                else
                {
                    Next(Vars.Run);
                    _chr.AnimDir = Inputs.Direction.x;
                }

                if (Inputs.Jump)
                {
                    Next(Vars.JumpSquat);
                }
            }

            SetVelocityX(Inputs.Direction.x * Vars.walkspeed);
        }
    }

    public class MS_Shield : MoveState
    {
        public MS_Shield(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("shield");
        }

        public override void Execute()
        {
            base.Execute();

            SetVelocityX(0);

            if (Mathf.Abs(Inputs.StrongDirection.x) > 0.5f)
            {
                Vars.RollDirection = Mathf.Sign(Inputs.Direction.x);
                Next(Vars.Roll);
            }

            if (Inputs.Shield == false)
            {
                Next(Vars.Idle);
            }

            CheckFallThroughPlatform();
            CheckIfAerial();
            CheckForJump();
        }
    }

    public class MS_Roll : MoveState
    {
        public MS_Roll(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            if (Mathf.Sign(Vars.RollDirection) == _chr.AnimDir)
            {
                Anim("roll");
            }
            else
            {
                Anim("rollBack");
            }
        }

        public override void Execute()
        {
            base.Execute();

            if (_timerF <= 10)
            {
                SetVelocityX(Mathf.Sign(Vars.RollDirection) * Vars.rollSpeed / 1.5f);
            }
            else
            {
                SetVelocityX(Mathf.Sign(Vars.RollDirection) * Vars.rollSpeed);
            }

            if (Ctr.OnLedge)
            {
                SetVelocityX(0);
            }

            if (_timerF > 5 && AnimOver())
            {
                if (Inputs.Shield)
                {
                    Next(Vars.Shield);
                }
                else
                {
                    Next(Vars.Idle);
                }
            }

            CheckIfAerial();
        }
    }

    public class MS_AirDodge : MoveState
    {
        private bool _neutralAirDodge = false;

        public MS_AirDodge(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("airDodge");

            if (Inputs.Direction.magnitude < 0.25f)
            {
                _neutralAirDodge = true;
            }
            else
            {
                _neutralAirDodge = false;
            }

            Vars.airDodged = true;
            Ctr.FallThroughPlatforms = false;
            Vars.FastFallRequestTimer = 0;
        }

        public override void Execute()
        {
            base.Execute();

            if (_neutralAirDodge == true)
            {

            }
            else
            {
                if (_timerF == 1)
                {
                    Ctr.ResetVelocity = true;
                }
                else
                {
                    Ctr.ResetVelocity = false;
                }
                if (_timerF < 3)
                {
                    SetVelocityX(0f);
                }
                else if (_timerF < 4)
                {
                    SetVelocityX(Inputs.Direction.x * Vars.airDodgeSpeed);
                    _chr.SetJumpVelocity((Inputs.Direction.y + 0.5f) * Vars.airDodgeSpeed / 1.5f);
                }
            }

            if (_timerF > Vars.airDodgeTimeF)
            {
                Next(Vars.Jump);
            }

            CheckIfLanding();
        }

        public override void Exit()
        {
            base.Exit();

            Ctr.ResetVelocity = false;
        }
    }

    public class MS_OnWall : MoveState
    {
        private float _savedDir;
        public MS_OnWall(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _savedDir = _chr.AnimDir;

            _chr.AnimDir = -Ctr.WallDirection;

            Anim("onWall");
        }

        public override void Execute()
        {
            base.Execute();

            Vars.WallJumpDirection = -Ctr.WallDirection;

            SetVelocityX(Inputs.Direction.x);

            if (Ctr.OnWall == false)
            {
                Next(Vars.Jump);
            }

            if(Inputs.Jump)
            {
                Next(Vars.WallJumpSquat);
            }

            CheckIfLanding();
        }

        public override void Exit()
        {
            base.Exit();

            _chr.AnimDir = _savedDir;
        }
    }

    public class MS_WallJumpSquat : MoveState
    {
        public MS_WallJumpSquat(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("duck");

            _chr.AnimDir = Vars.WallJumpDirection;
        }

        public override void Execute()
        {
            base.Execute();

            Inputs.Direction = Vector2.zero;

            if (_timerF > 3)
            {
                Next(Vars.Jump);

                _chr.SetJumpVelocity(_chr.MoveStateVars.wallJumpVelocity);
            }
        }
    }
}
