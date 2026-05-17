namespace Effects
{
    public abstract class Buff
    {
        public float duration;
        public float modifier;

    	public abstract float moveDelayModifier { get; }
    	public abstract float attackDelayModifier { get; }
	}

	public class MoveDebuff : Buff {
    	public override float moveDelayModifier => modifier;
    	public override float attackDelayModifier => 1f;
	}

	public class AttackSpeedBuff : Buff {
    	public override float moveDelayModifier => 1f;
    	public override float attackDelayModifier => modifier;
	}
}