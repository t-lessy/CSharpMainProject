using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Runtime
{
    public class BuffSystem
    {
        private readonly Dictionary<Unit, List<Buff>> _unitBuffs = new();

        public void AddBuff(Unit unit, Buff buff)
        {
            if (!_unitBuffs.ContainsKey(unit))
            {
                _unitBuffs[unit] = new List<Buff>();
            }

            _unitBuffs[unit].Add(buff);
            Debug.Log($"[BuffSystem] Added buff to {unit.Config.Name}: {buff.GetDescription()}");
        }

        public void RemoveUnit(Unit unit)
        {
            if (_unitBuffs.ContainsKey(unit))
            {
                _unitBuffs.Remove(unit);
            }
        }

        public float GetMovementSpeedModifier(Unit unit)
        {
            if (!_unitBuffs.ContainsKey(unit) || _unitBuffs[unit].Count == 0)
                return 1f;

            float totalModifier = 1f;

            foreach (var buff in _unitBuffs[unit])
            {
                if (buff is MovementSpeedBuff or MovementSlowDebuff)
                {
                    totalModifier *= buff.Modifier;
                }
            }

            return totalModifier;
        }

        public float GetAttackSpeedModifier(Unit unit)
        {
            if (!_unitBuffs.ContainsKey(unit) || _unitBuffs[unit].Count == 0)
                return 1f;

            float totalModifier = 1f;

            foreach (var buff in _unitBuffs[unit])
            {
                if (buff is AttackSpeedBuff or AttackSlowDebuff)
                {
                    totalModifier *= buff.Modifier;
                }
            }

            return totalModifier;
        }

        public void Update(float deltaTime)
        {
            var unitsToClean = new List<Unit>();

            foreach (var kvp in _unitBuffs)
            {
                var unit = kvp.Key;
                var buffs = kvp.Value;

                var buffsToRemove = new List<Buff>();

                foreach (var buff in buffs)
                {
                    buff.Duration -= deltaTime;

                    if (buff.Duration <= 0)
                    {
                        buffsToRemove.Add(buff);
                    }
                }

                foreach (var buff in buffsToRemove)
                {
                    buffs.Remove(buff);
                    Debug.Log($"[BuffSystem] Removed buff from {unit.Config.Name}: {buff.GetDescription()}");
                }

                if (buffs.Count == 0)
                {
                    unitsToClean.Add(unit);
                }
            }

            foreach (var unit in unitsToClean)
            {
                _unitBuffs.Remove(unit);
            }
        }

        public void Clear()
        {
            _unitBuffs.Clear();
        }
    }
}