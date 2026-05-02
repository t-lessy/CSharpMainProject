using Model.Runtime;
using Model.Runtime.ReadOnly;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BuffSystem
{
    public interface IBuffSystem
    {
        void AddBuff(Unit unit, BuffType type, float duration, float modifier);
        Buff GetBuff(Unit unit, BuffType type);
        void RemoveBuff(Unit unit, BuffType type);
        bool HasAnyBuff(Unit unit);
    }

    public class BuffManager : IBuffSystem
    {
        private Dictionary<Unit, List<Buff>> _unitBuffs = new Dictionary<Unit, List<Buff>>();
        private Coroutine _updateCoroutine;

        public void Initialize(MonoBehaviour monoBehaviour)
        {
            if (_updateCoroutine != null)
                monoBehaviour.StopCoroutine(_updateCoroutine);

            _updateCoroutine = monoBehaviour.StartCoroutine(UpdateBuffs());
        }

        public void AddBuff(Unit unit, BuffType type, float duration, float modifier)
        {
            var buff = new Buff(type, duration, modifier);

            if (!_unitBuffs.ContainsKey(unit))
                _unitBuffs[unit] = new List<Buff>();

            RemoveBuff(unit, type);
            _unitBuffs[unit].Add(buff);
        }

        public Buff GetBuff(Unit unit, BuffType type)
        {
            if (!_unitBuffs.ContainsKey(unit))
                return null;

            return _unitBuffs[unit].Find(b => b.Type == type);
        }

        public void RemoveBuff(Unit unit, BuffType type)
        {
            if (!_unitBuffs.ContainsKey(unit))
                return;

            var buffToRemove = _unitBuffs[unit].Find(b => b.Type == type);
            if (buffToRemove != null)
                _unitBuffs[unit].Remove(buffToRemove);
        }

        public bool HasAnyBuff(Unit unit)
        {
            return _unitBuffs.ContainsKey(unit) &&
                   _unitBuffs[unit].Count > 0;
        }

        private IEnumerator UpdateBuffs()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f); // Обновляем каждые 0.1 секунды

                var unitsToCleanup = new List<Unit>();

                foreach (var unitPair in _unitBuffs)
                {
                    var unit = unitPair.Key;
                    var buffs = unitPair.Value;
                    var expiredBuffs = new List<Buff>();

                    foreach (var buff in buffs)
                    {
                        float oldDuration = buff.Duration;
                        buff.DecreaseDuration(0.1f);
                        Debug.Log($"Бафф {buff.Type} для {unit.Config.Name}: длительность {oldDuration:F2} → {buff.Duration:F2}");

                        if (buff.IsExpired())
                        {
                            Debug.LogWarning($"Бафф {buff.Type} истёк для {unit.Config.Name}!");
                            expiredBuffs.Add(buff);
                        }
                    }

                    foreach (var expired in expiredBuffs)
                        buffs.Remove(expired);

                    if (buffs.Count == 0)
                        unitsToCleanup.Add(unit);
                }

                foreach (var unit in unitsToCleanup)
                    _unitBuffs.Remove(unit);
            }
        }
    }
}
