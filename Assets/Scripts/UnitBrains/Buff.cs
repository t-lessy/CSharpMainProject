using Model.Runtime;

namespace Buffs
{
    public abstract class Buff
    {
        public float Duration { get; private set; }
        public float MoveSpeedModifier { get; private set; }
        public float AttackSpeedModifier { get; private set; }

        protected Buff()
        {

        }

        public virtual void Expire()
        {

        }
    }

    public class MovementBuff : Buff
    {
        public float Duration { get; private set; }
        public float MoveSpeedModifier { get; private set; }

         public  MovementBuff(float duration, float moveSpeedModifier)
        {
            Duration = duration;
            MoveSpeedModifier = moveSpeedModifier;
        }

        public override void Expire()
        {
            MoveSpeedModifier = 0f;
        }
    }

    public class AttackSpeedBuff : Buff
    {
        protected float Duration { get; private set; }

        public float AttackSpeedModifier { get; private set; }

        public AttackSpeedBuff(float duration, float attackSpeedModifier)
        {
            Duration = duration;
            AttackSpeedModifier = attackSpeedModifier;
        }

        public override void Expire()
        {
           
        }
    }
}