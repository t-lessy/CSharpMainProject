namespace Model.Runtime.Buffs
{
    public class Buff
    {
        public enum BuffType
        {
            AttackSpeed,
            MoveSpeed,
            Invulnerability
        }
        
        public BuffType Type { get; }
        public float Duration { get; set; }
        public float Value { get; }

        public Buff(BuffType type, float duration, float value)
        {
            Type = type;
            Duration = duration;
            Value = value;
        }
    }
}