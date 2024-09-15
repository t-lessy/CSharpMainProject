using Model.Runtime;
using System.Collections.Generic;
using Utilities;

public class BuffController
{

    private Dictionary<Unit, List<AbstractBuff>> UnitsEffectStatus = new Dictionary<Unit, List<AbstractBuff>>();

    public BuffController()
    {
        ServiceLocator.Get<TimeUtil>().AddFixedUpdateAction(updateEffects);
    }

    internal void addEffect(Unit unit, AbstractBuff effect)
    {
        List<AbstractBuff> unitsEffect;

        if (UnitsEffectStatus.ContainsKey(unit))
        {
            unitsEffect = UnitsEffectStatus[unit];
        }
        else
        {
            unitsEffect = new List<AbstractBuff>();
            UnitsEffectStatus.Add(unit, unitsEffect);
        }

        unitsEffect.Add(effect);
    }

    internal void RemoveEffect(AbstractBuff effect, Unit unit)
    {

        if (UnitsEffectStatus.ContainsKey(unit))
        {
            List<AbstractBuff> unitsEffect = UnitsEffectStatus[unit];
            unitsEffect.Remove(effect);
        }

    }

    public void updateEffects(float deltaTime)
    {
        foreach (KeyValuePair<Unit, List<AbstractBuff>> pare in UnitsEffectStatus)
        {
            foreach (AbstractBuff effect in pare.Value.ToArray())
            {
                effect.duration -= deltaTime;
                if (effect.duration <= 0)
                    RemoveEffect(effect, pare.Key);
            }
        }
    }

    internal float getAttackDelayMod(Unit unit)
    {
        float mod = 1f;

        List<AbstractBuff> effectsList;
        if (UnitsEffectStatus.TryGetValue(unit, out effectsList))
        {
            foreach (AbstractBuff effect in effectsList)
            {
                mod *= effect.AttackDelayMod;
            }
        }
        return mod;
    }
}