using UnityEngine;

namespace MoveStates
{

    public class MoveState
    {
        protected Character chr;

        public MoveState(Character chr)
        {
            this.chr = chr;
        }

        public virtual void Enter()
        {

        }

        public virtual void Execute()
        {

        }

        public virtual void Exit()
        {

        }

        protected void Anim(string animName)
        {
            chr.Animate(animName);
        }

        protected void Next(MoveState nextState)
        {
            chr.SetNextState(nextState);
        }

        protected void InputX(float inputX)
        {
            chr.SetInputDirection(new Vector2(inputX, 0));
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
        }

        public override void Execute()
        {
            base.Execute();

            if(chr.DirectionalInput.x != 0)
            {
                chr.AnimDir = chr.DirectionalInput.x;
                Next(chr.walk);
            }
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

            if (chr.DirectionalInput.x == 0)
            {
                Next(chr.idle);
            }
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
    }

    public class MS_Jump : MoveState
    {
        public MS_Jump(Character chr) : base(chr)
        {
        }
    }
}