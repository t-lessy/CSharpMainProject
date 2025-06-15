namespace Assets.Scripts.Model.Runtime.Buffs
{
    // Ускорение атаки
    public class HasteAttackBuff : Buff
    {
        public float Multiplier => Modifier;
        public HasteAttackBuff(float duration, float multiplier)
            : base(duration, multiplier) { }
    }
}