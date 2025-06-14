namespace Assets.Scripts.Model.Runtime.Buffs
{
    // Ускорение передвижения
    public class HasteMovementBuff : Buff
    {
        public float Multiplier => Modifier;
        public HasteMovementBuff(float duration, float multiplier)
            : base(duration, multiplier) { }
    }
}