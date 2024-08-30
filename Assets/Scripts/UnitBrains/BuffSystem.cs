using Model.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Buffs
{
    public class BuffSystem
    {
        public event Action<Unit, Buff> OnBuffAdded;
        public event Action<Unit, Buff> OnBuffRemoved;

        private readonly TimeUtil _timeUtil = ServiceLocator.Get<TimeUtil>();
        public readonly Dictionary<Unit, HashSet<Buff>> unitBuffs = new();

        public void ApplyBuff(Unit unit, Buff buff)
        {
            if (!unitBuffs.ContainsKey(unit))
            {
                unitBuffs[unit] = new HashSet<Buff>();
            }
            unitBuffs[unit].Add(buff);

            OnBuffAdded?.Invoke(unit, buff);

            _timeUtil.RunDelayed(buff.Duration, () => UpdateBuffs(unit,buff));
        }

        public void UpdateBuffs(Unit unit, Buff buff)
        {
            unitBuffs[unit].Remove(buff);

            OnBuffRemoved?.Invoke(unit, buff);
        }
    }
}
