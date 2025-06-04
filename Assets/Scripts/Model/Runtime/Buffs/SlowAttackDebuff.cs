namespace Assets.Scripts.Model.Runtime.Buffs
{
    // Замедление атаки
    public class SlowAttackDebuff : Buff
    {
        public SlowAttackDebuff(float duration, float multiplier)
            : base(duration, multiplier) { }
    }
}