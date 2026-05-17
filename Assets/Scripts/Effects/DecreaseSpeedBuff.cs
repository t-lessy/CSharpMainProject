namespace Effects
{
    public class DecreaseSpeedBuff : Buff
    {
        public override float moveDelayModifier => modifier;
    	public override float attackDelayModifier => 1f;
    }
}