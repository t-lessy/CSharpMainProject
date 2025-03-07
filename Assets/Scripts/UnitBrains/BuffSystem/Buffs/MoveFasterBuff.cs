using Model.Runtime;
using UnitBrains.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UnitBrains.BuffSystem
{
    public class MoveFasterBuff : Buff<Unit>
    {
        public MoveFasterBuff(float duration) : base(duration)
        {
        }

        public override string Name { get; } = "MoveFasterBuff";
        public override void Add(Unit unit)
        {
            if (CanApply(unit))
            {
                unit.MoveDelay = 0.1f;
                Debug.Log($"Applied buff '{Name}' to unit '{unit.Config.Name}'.");
            }
        }

        public override void Remove(Unit unit)
        {
            if (IsExpired)
            {
                unit.MoveDelay = unit.Config.MoveDelay;
                Debug.Log($"buff {Name} remove from unit '{unit.Config.Name}'.");
            }
        }

        public override bool CanApply(Unit unit) 
            => unit.Brain != null && unit.Brain.GetType() == typeof(SecondUnitBrain);
        
    }
}