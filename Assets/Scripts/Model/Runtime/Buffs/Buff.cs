namespace Assets.Scripts.Model.Runtime.Buffs
{
    public abstract class Buff
    {
        public float Duration { get; private set; }
        public float Modifier { get; }

        protected Buff(float duration, float modifier)
        {
            Duration = duration;
            Modifier = modifier;
        }

        
        public bool Tick(float deltaTime)
        {
            Duration -= deltaTime;
            return Duration <= 0f;
        }
    }
}