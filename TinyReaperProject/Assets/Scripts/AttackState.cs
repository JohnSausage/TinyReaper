namespace MoveStates
{
    public class AttackState : MoveState
    {
        public AttackState(Character chr) : base(chr)
        {
        }
    }

    public class AS_PlayerJab : AttackState
    {
        public AS_PlayerJab(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("jab");
        }

        public override void Execute()
        {
            base.Execute();

            SetVelocityX(0);

            CheckIfAerial();

            if(AnimOver())
            {
                Next(Vars.Idle);
            }
        }
    }

    public class AS_PlayerNair : AttackState
    {
        public AS_PlayerNair(Character chr) : base(chr)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Anim("nair");
        }

        public override void Execute()
        {
            base.Execute();

            AerialMovement();

            if (AnimOver())
            {
                Next(Vars.Jump);
            }

            CheckForFastFallRequest();
            CheckFallThroughPlatform();
            CheckIfLanding();
        }
    }
}
