using Model.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.Model.Runtime
{
    public class BuffsController : IDisposable
    {
        private readonly Dictionary<Unit, List<Buff>> _buffs = new();
        private TimeUtil _timeUtil;
        private bool _disposed = false;

        public BuffsController()
        {
            _timeUtil = ServiceLocator.Get<TimeUtil>();
            _timeUtil.AddFixedUpdateAction(Update);
        }

        public void AddBuff(Unit unit, Buff newBuff)
        {
            if (unit.IsDead)
                return;

            try
            {
                if (!_buffs.TryGetValue(unit, out var unitBuffs))
                {
                    unitBuffs = new List<Buff>();
                    _buffs[unit] = unitBuffs;
                }

                var existingBuffType = unitBuffs.FirstOrDefault(b => b.GetType() == newBuff.GetType());
                if (existingBuffType != null)
                {
                    existingBuffType.Duration = Mathf.Max(existingBuffType.Duration, newBuff.Duration);
                }
                else
                {
                    unitBuffs.Add(newBuff);
                }
            }
            catch (ArgumentException ex)
            {
                Debug.LogError($"Недопустимые параметры усиления: {ex.Message}");
            }
        }

        public void RemoveBuffs(Unit unit)
        {
            _buffs.Remove(unit);
        }

        public void ClearAllBuffs()
        {
            _buffs.Clear();
        }

        public float GetMoveSpeedModifier(Unit unit)
        {
            if (!_buffs.TryGetValue(unit, out var unitBuffs))
                return 1f;

            float modifier = 1f;
            foreach (var buff in unitBuffs)
            {
                modifier *= buff.MoveSpeedModifier;
            }
            return modifier;
        }

        public float GetAttackSpeedModifier(Unit unit)
        {
            if (!_buffs.TryGetValue(unit, out var unitBuffs))
                return 1f;

            float modifier = 1f;
            foreach (var buff in unitBuffs)
            {
                modifier *= buff.AttackSpeedModifier;
            }
            return modifier;
        }

        public void Update(float deltaTime)
        {
            if (_disposed) return;

            var units = _buffs.Keys.ToList();
            foreach (var unit in units)
            {
                if (unit.IsDead)
                {
                    RemoveBuffs(unit);
                    continue;
                }

                var unitBuffs = _buffs[unit];
                for (int i = unitBuffs.Count - 1; i >= 0; i--)
                {
                    var buff = unitBuffs[i];
                    buff.Duration -= deltaTime;
                    if (buff.Duration <= 0)
                    {
                        unitBuffs.RemoveAt(i);
                    }
                }

                if (unitBuffs.Count == 0)
                {
                    _buffs.Remove(unit);
                }
            }
        }

        public bool HasBuff<T>(Unit unit) where T : Buff
        {
            if (_buffs.TryGetValue(unit, out var unitBuffs))
            {
                return unitBuffs.Any(buff => buff is T);
            }
            return false;
        }
                
        public void Dispose()
        {
            if (_disposed) return;
            _timeUtil?.RemoveFixedUpdateAction(Update);
            _disposed = true;
        }

        public class BuffCarBuff : Buff
        {
            public BuffCarBuff(float duration, float moveSpeedModifier) : base(duration)
            {
                ValidateMoveSpeedModifier(moveSpeedModifier, nameof(BuffCarBuff));
                MoveSpeedModifier = moveSpeedModifier;
            }
        }
    }
}