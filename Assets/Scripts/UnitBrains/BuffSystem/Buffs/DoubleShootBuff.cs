using Model.Runtime;
using UnitBrains.Player;
using UnityEngine;

namespace UnitBrains.BuffSystem
{
    public class DoubleShootBuff : Buff<Unit>
    {

        public override string Name { get; } = "DoubleShootBuff";
        private int _doubleShootIndex = 2;
        private int _defaultShootIndex = 1;
        
        public DoubleShootBuff(float duration) : base(duration)
        {
        }
        
        public override void Add(Unit unit)
        {
            if(CanApply(unit))
            {
                unit.Brain.DoubleShootIndex = _doubleShootIndex;
                Debug.Log($"Applied buff '{Name}' to unit '{unit.Config.Name}'.");
            }
        }

        public override void Remove(Unit unit)
        {
            if (IsExpired)
            {
                unit.Brain.DoubleShootIndex = _defaultShootIndex;
                Debug.Log($"buff {Name} remove from unit '{unit.Config.Name}'.");
            }
        }

        public override bool CanApply(Unit unit) 
            => unit.Brain != null && unit.Brain.GetType() == typeof(DefaultPlayerUnitBrain);
    }
}