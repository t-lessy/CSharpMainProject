namespace Assets.Scripts.Model.Runtime.Buffs
{
    // Замедление передвижения
    public class SlowMovementDebuff : Buff
    {
        public SlowMovementDebuff(float duration, float multiplier)
            : base(duration, multiplier) { }
    }
}